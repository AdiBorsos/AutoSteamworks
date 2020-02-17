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
            { VirtualKeyCode.VK_A, 999 },
            { VirtualKeyCode.VK_W, 999 },
            { VirtualKeyCode.VK_D, 999 },
            { VirtualKeyCode.VK_Q, 999 },
            { VirtualKeyCode.VK_Z, 999 },
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

            SaveData sd = new SaveData(mhw);

            if (mhw != null)
            {
                InputSimulator sim = new InputSimulator();

                ulong starter = 0x140000000 + 0x4D68970;
                var pointerAddress = MemoryHelper.Read<ulong>(mhw, starter);
                // offset the address
                var offset_Address = pointerAddress + 0x350;
                var offset_buttonPressState = offset_Address + 8;

                while (!shouldStop)
                {
                    Logger.LogInfo($"Gauge Data {sd.SteamGauge}!");

                    // value of the offset address
                    var actualSequence = MemoryHelper.Read<int>(mhw, offset_Address);
                    if (actualSequence == 0)
                    {
                        // wait for init of Steamworks
                        continue;
                    }

                    var orderBytes = BitConverter.GetBytes(actualSequence);
                    if (Settings.IsAzerty)
                    {
                        keyOrder[VirtualKeyCode.VK_Q] = int.Parse(((char)(orderBytes[0] + 0x30)).ToString());   // Q
                        keyOrder[VirtualKeyCode.VK_Z] = int.Parse(((char)(orderBytes[1] + 0x30)).ToString());   // Z
                        keyOrder[VirtualKeyCode.VK_D] = int.Parse(((char)(orderBytes[2] + 0x30)).ToString());   // D
                    }
                    else
                    {
                        keyOrder[VirtualKeyCode.VK_A] = int.Parse(((char)(orderBytes[0] + 0x30)).ToString());   // A
                        keyOrder[VirtualKeyCode.VK_W] = int.Parse(((char)(orderBytes[1] + 0x30)).ToString());   // W
                        keyOrder[VirtualKeyCode.VK_D] = int.Parse(((char)(orderBytes[2] + 0x30)).ToString());   // D
                    }

                    var ordered = keyOrder.OrderBy(x => x.Value).ToList();
                    Logger.LogInfo($"Pressing {string.Join(" -> ", ordered.Select(x => x.Key.ToString()))}");

                    int index = 0;
                    while (index < 3)
                    {
                        var before = MemoryHelper.Read<byte>(mhw, offset_buttonPressState);

                        var item = ordered[index];

                        PressKey(sim, item.Key);

                        byte after = before;
                        while (before == after)
                        {
                            after = MemoryHelper.Read<byte>(mhw, offset_buttonPressState);

                            PressKey(sim, item.Key);

                            if (shouldStop)
                            {
                                break;
                            }
                        }

                        index++;
                    }

                    var currentState = MemoryHelper.Read<byte>(mhw, offset_buttonPressState);
                    while (currentState != (int)ButtonPressingState.BeginningOfSequence)
                    {
                        Thread.Sleep(rnd.Next((int)Settings.DelayBetweenCombo));

                        // no more fuel
                        if (currentState == (int)ButtonPressingState.EndOfGame)
                        {
                            if (sd.NaturalFuel + sd.StoredFuel < 10)
                            {
                                shouldStop = true;
                                break;
                            }

                            if (sd.SteamGauge == 0)
                            {
                                PressKey(sim, VirtualKeyCode.SPACE);
                            }
                        }

                        if (shouldStop)
                        {
                            break;
                        }

                        currentState = MemoryHelper.Read<byte>(mhw, offset_buttonPressState);
                    }
                }

                api.Dispose();
            }
        }

        private static void PressKey(InputSimulator sim, VirtualKeyCode key)
        {
            sim.Keyboard.KeyPress(key);

            Logger.LogInfo($"Pressed: {key}!");
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
                        Logger.LogInfo($"Captured Start!");
                    }

                    if (character.KeyCode == KeyCode.Escape)
                    {
                        shouldStop = true;

                        Logger.LogInfo($"Captured Escape!");

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