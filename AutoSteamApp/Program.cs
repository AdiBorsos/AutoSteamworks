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

        private static Random rnd = new Random();
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
                var offset_Address = pointerAddress + 0x350;
                var offset_NumberOfButtonsPressed = offset_Address + 8;

                while (!shouldStop)
                {
                    bool pressedThisCycle = false;

                    // value of the offset address
                    var actualSequence = MemoryHelper.Read<int>(mhw, offset_Address);

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
                    Logger.LogInfo($"Pressing {string.Join(" -> ", ordered.Select(x => x.Key.ToString()))}");

                    if (MemoryHelper.Read<ButtonIndex>(mhw, offset_NumberOfButtonsPressed) == ButtonIndex.NoButtonPressed)
                    {
                        var item = ordered[0];
                        var keyToPressNow = item.Key;
                        sim.Keyboard.KeyDown(keyToPressNow);
                        Thread.Sleep((int)Settings.DelayBetweenKeys);
                        sim.Keyboard.KeyUp(keyToPressNow);

                        pressedThisCycle = true;
                    }

                    if (MemoryHelper.Read<ButtonIndex>(mhw, offset_NumberOfButtonsPressed) == ButtonIndex.OneButtonPressed)
                    {
                        var item = ordered[1];
                        var keyToPressNow = item.Key;
                        sim.Keyboard.KeyDown(keyToPressNow);
                        Thread.Sleep((int)Settings.DelayBetweenKeys);
                        sim.Keyboard.KeyUp(keyToPressNow);

                        pressedThisCycle = true;
                    }

                    if (MemoryHelper.Read<ButtonIndex>(mhw, offset_NumberOfButtonsPressed) == ButtonIndex.TwoButtonsPressed)
                    {
                        var item = ordered[2];
                        var keyToPressNow = item.Key;
                        sim.Keyboard.KeyDown(keyToPressNow);
                        Thread.Sleep((int)Settings.DelayBetweenKeys);
                        sim.Keyboard.KeyUp(keyToPressNow);

                        pressedThisCycle = true;
                    }

                    Thread.Sleep(rnd.Next((int)Settings.DelayBetweenCombo));
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
                Logger.LogError($"Error trying to find '{ProcessName}' process.");
            }

            Logger.LogError($"Looks like the game is not running. It should...");

            return null;
        }
    }
}
