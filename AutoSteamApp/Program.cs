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
        static void Main(string[] args)
        {
            Console.WriteLine("Press 'Insert' to start typing");
            Console.WriteLine("Press 'Escape' to end typing");

            DoWork();
        }

        private const string ProcessName = "MonsterHunterWorld";
        private static volatile bool shouldStop = false;
        private static volatile bool shouldStart = false;

        private static KeystrokeAPI api = new KeystrokeAPI();

        private static void DoWork()
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

            while (!shouldStart)
            {
                Thread.Sleep(1000);
            }
            
            Process mhw = GetMHW();
            if (mhw != null)
            {
                InputSimulator sim = new InputSimulator();
                
                ulong starter = 0x140000000 + 0x4D68970;
                var readResult2 = MemoryHelper.Read<ulong>(mhw, starter);

                // offset the address
                var offsetAddress = readResult2 + 0x350;
                
                var index = 5;
                while (!shouldStop)
                {
                    sim.Keyboard.KeyDown(VirtualKeyCode.VK_L);
                    Thread.Sleep(100);
                    sim.Keyboard.KeyUp(VirtualKeyCode.VK_L);

                    // value of the offset address
                    var readResult3 = MemoryHelper.Read<int>(mhw, offsetAddress);

                    var orderBytes = BitConverter.GetBytes(readResult3);

                    int orderOfA = int.Parse(((char)(orderBytes[0] + 0x30)).ToString());   // A
                    int orderOfW = int.Parse(((char)(orderBytes[1] + 0x30)).ToString());   // W
                    int orderOfD = int.Parse(((char)(orderBytes[2] + 0x30)).ToString());   // D

                    Dictionary<VirtualKeyCode, int> data = new Dictionary<VirtualKeyCode, int>()
                {
                    { VirtualKeyCode.VK_A, -1 },
                    { VirtualKeyCode.VK_W, -1 },
                    { VirtualKeyCode.VK_D, -1 }
                };

                    data[VirtualKeyCode.VK_A] = orderOfA;
                    data[VirtualKeyCode.VK_W] = orderOfW;
                    data[VirtualKeyCode.VK_D] = orderOfD;

                    var ordered = data.OrderBy(x => x.Value).ToList();

                    Console.WriteLine($"Pressing {string.Join("->", ordered.Select(x => x.Key.ToString()))}");

                    foreach (var item in ordered)
                    {
                        var keyToPressNow = item.Key;
                        sim.Keyboard.KeyDown(keyToPressNow);
                        Thread.Sleep(500);
                        sim.Keyboard.KeyUp(keyToPressNow);


                    }

                    Thread.Sleep(500);
                    index++;
                }

                api.Dispose();
            }
        }

        private static Process GetMHW()
        {
            var processes = Process.GetProcesses();
            foreach (var p in processes)
            {
                try
                {
                    if (p != null && p.ProcessName.Equals(ProcessName) && !p.HasExited)
                    {
                        return p;
                    }
                }
                catch
                {
                    // nothing here
                }
            }

            return null;
        }
    }
}
