using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoSteamApp.Core;
using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;
using Keystroke.API;

namespace AutoSteamApp
{
    class Program
    {
        private const string ProcessName = "MonsterHunterWorld";
        private static volatile bool shouldStop = false;
        private static volatile bool shouldStart = false;

        private static KeystrokeAPI api = new KeystrokeAPI();
        private static readonly Dictionary<VirtualKeyCode, int> keyOrder = new Dictionary<VirtualKeyCode, int>()
        {
            { VirtualKeyCode.VK_A, -1 },
            { VirtualKeyCode.VK_W, -1 },
            { VirtualKeyCode.VK_D, -1 }
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Press 'Insert' to start typing");
            Console.WriteLine("Press 'Escape' to end typing");

            DoWork();
        }

        private static void DoWork()
        {
            HookKeyboardEvents();

            Process mhw = GetMHW();
            while (!shouldStart || mhw == null)
            {
                Thread.Sleep(1000);
                mhw = GetMHW();
            }

            if (mhw != null)
            {
                InputSimulator sim = new InputSimulator();

                ulong starter = 0x140000000 + 0x4D68970;
                var pointerAddress = MemoryHelper.Read<ulong>(mhw, starter);
                // offset the address
                var offsetAddress = pointerAddress + 0x350;

                while (!shouldStop)
                {
                    // value of the offset address
                    var actualSequence = MemoryHelper.Read<int>(mhw, offsetAddress);
                    if (actualSequence == 0)
                    {
                        // wait for init of Steamworks 
                        continue;
                    }

                    var orderBytes = BitConverter.GetBytes(actualSequence);

                    keyOrder[VirtualKeyCode.VK_A] = int.Parse(((char)(orderBytes[0] + 0x30)).ToString());   // A
                    keyOrder[VirtualKeyCode.VK_W] = int.Parse(((char)(orderBytes[1] + 0x30)).ToString());   // W
                    keyOrder[VirtualKeyCode.VK_D] = int.Parse(((char)(orderBytes[2] + 0x30)).ToString());   // D

                    var ordered = keyOrder.OrderBy(x => x.Value).ToList();

                    Console.WriteLine($"Pressing {string.Join(" -> ", ordered.Select(x => x.Key.ToString()))}");

                    foreach (var item in ordered)
                    {
                        var keyToPressNow = item.Key;
                        sim.Keyboard.KeyDown(keyToPressNow);
                        Thread.Sleep((int)Settings.DelayBetweenKeys);
                        sim.Keyboard.KeyUp(keyToPressNow);
                    }

                    Thread.Sleep((int)Settings.DelayBetweenCombo);
                }

                api.Dispose();
            }
        }

        private static void HookKeyboardEvents()
        {
            Task.Run(() =>
            {
                api.CreateKeyboardHook((character) =>
                {
                    if (character.KeyCode == KeyCode.Insert)
                    {
                        shouldStart = true;
                    }

                    if (character.KeyCode == KeyCode.Escape)
                    {
                        shouldStop = true;

                        Application.Exit();
                    }
                });

                Application.Run();
            });
        }

        private static Process GetMHW()
        {
            var processes = Process.GetProcesses();
            try
            {
                return processes.FirstOrDefault(p => p != null && p.ProcessName.Equals(ProcessName) && !p.HasExited);
            }
            catch
            {
                Console.WriteLine($"Error trying to find '{ProcessName}' process.");
            }

            Console.WriteLine($"Looks like the game is not running. It should...");

            return null;
        }
    }
}
