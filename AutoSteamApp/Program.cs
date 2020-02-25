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

        private static readonly Dictionary<int, List<int>> rndPatterns = new Dictionary<int, List<int>>()
        {
            { 0, new List<int> { 0, 1, 2 } },
            { 1, new List<int> { 1, 0, 2 } },
            { 2, new List<int> { 2, 0, 1 } },
            { 3, new List<int> { 0, 2, 1 } },
            { 4, new List<int> { 2, 1, 0 } },
            { 5, new List<int> { 1, 2, 0 } }
        };

        private static Process mhw;
        private static CancellationTokenSource ct = new CancellationTokenSource();
        private static bool isRandomPath = false;

        static void Main(string[] args)
        {
            Console.Title = $"Currently built for version: ({Settings.SupportedGameVersion})";

            Console.WriteLine($"Currently built for version: {Settings.SupportedGameVersion}");

            Console.WriteLine($"Press '{((KeyCode)Settings.KeyCodeStart).ToString()}' to start typing");
            Console.WriteLine($"Press '{((KeyCode)Settings.KeyCodeStop).ToString()}' to end typing");

            HookKeyboardEvents();

            Startup();

            if (isRandomPath)
            {
                DoRandomWork();
            }
            else
            {
                DoWork();
            }
        }

        private static void Startup()
        {
            while (mhw == null && !ct.IsCancellationRequested)
            {
                mhw = GetMHW();
                Thread.Sleep(1000);
            }

            while (!shouldStart && !ct.IsCancellationRequested)
            {
                Thread.Sleep(1000);
            }

            if (mhw != null &&
                !mhw.MainWindowTitle.Contains(Settings.SupportedGameVersion))
            {
                var currentVersion = int.Parse(mhw.MainWindowTitle.Split('(')[1].Replace(")", ""));
                Logger.LogError($"Currently built for version: {Settings.SupportedGameVersion}. This game version ({currentVersion}) is not supported YET!");

                if (!Settings.UseRandomPatterns)
                {
                    Logger.LogError($"However, if you still want to use the application, please set UseRandomPatterns to TRUE in the AutoSteamApp.exe.config");
                    Logger.LogError($"UseRandomPatterns to TRUE will push buttons randomly, which is still better than nothing.");

                    mhw = null;
                }
                else
                {
                    Logger.LogError($"Version Not Supported and UseRandomPatters is ACTIVE - app will type randomly - to disable this see AutoSteamApp.exe.config!");
                    isRandomPath = true;
                }
            }
        }

        private static void DoRandomWork()
        {
            if (mhw != null && !ct.IsCancellationRequested)
            {
                InputSimulator sim = new InputSimulator();
                while (!shouldStop && !ct.IsCancellationRequested)
                {
                    List<int> orderBytes = rndPatterns[rnd.Next(0, 5)];

                    Dictionary<VirtualKeyCode, int> keyOrder = new Dictionary<VirtualKeyCode, int>()
                    {
                        { Settings.KeyCodesToPress[0], orderBytes[0] },
                        { Settings.KeyCodesToPress[1], orderBytes[1] },
                        { Settings.KeyCodesToPress[2], orderBytes[2] },
                    };

                    foreach (var item in keyOrder.OrderBy(x => x.Value).Take(3).ToList())
                    {
                        PressKey(sim, item.Key, true);
                    }

                    PressKey(sim, (VirtualKeyCode)Settings.KeyCutsceneSkip, true);

                    PressKey(sim, VirtualKeyCode.SPACE, true);
                }

                api.Dispose();
            }
        }

        private static void DoWork()
        {
            if (mhw != null && !ct.IsCancellationRequested)
            {
                InputSimulator sim = new InputSimulator();
                SaveData sd = new SaveData(mhw, ct);

                ulong starter = Settings.Off_Base + Settings.Off_SteamworksCombo;

                var pointerAddress = MemoryHelper.Read<ulong>(mhw, starter);
                // offset the address
                var offset_Address = pointerAddress + 0x350;
                var offset_buttonPressState = offset_Address + 8;

                while (!shouldStop && !ct.IsCancellationRequested)
                {
                    Logger.LogInfo($"Gauge Data {sd.SteamGauge}!");

                    // value of the offset address
                    var ordered = ExtractCorrectSequence(mhw, offset_Address);
                    if (ordered == null)
                    {
                        Logger.LogInfo("The Steamworks minigame is not started. Please enter the minigame and Press 'Space' so that you see the first letters on your screen.");

                        // try again..
                        continue;
                    }

                    int index = 0;
                    while (index < 3)
                    {
                        try
                        {
                            var before = MemoryHelper.Read<byte>(mhw, offset_buttonPressState);

                            var item = ordered[index];

                            byte after = before;
                            while (before == after && !ct.IsCancellationRequested)
                            {
                                PressKey(sim, item.Key);

                                after = MemoryHelper.Read<byte>(mhw, offset_buttonPressState);
                            }

                            index++;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"Trying to press button sequence: {ex.Message}");
                        }
                    }

                    if (shouldStop)
                    {
                        break;
                    }

                    var currentState = MemoryHelper.Read<byte>(mhw, offset_buttonPressState);
                    while (currentState != (int)ButtonPressingState.BeginningOfSequence && !ct.IsCancellationRequested)
                    {
                        Thread.Sleep(50);

                        try
                        {
                            PressKey(sim, (VirtualKeyCode)Settings.KeyCutsceneSkip);

                            // no more fuel
                            if (currentState == (int)ButtonPressingState.EndOfGame)
                            {
                                if (sd.NaturalFuel + sd.StoredFuel < 10)
                                {
                                    Logger.LogInfo("No more fuel, stopping bot.");
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
                        catch (Exception ex)
                        {
                            Logger.LogError($"Trying to finish up combo: {ex.Message}");
                        }
                    }
                }

                api.Dispose();
            }
        }

        private static List<KeyValuePair<VirtualKeyCode, int>> ExtractCorrectSequence(Process mhw, ulong offset_Address)
        {
            try
            {
                Thread.Sleep(rnd.Next((int)Settings.DelayBetweenCombo));

                var actualSequence = MemoryHelper.Read<int>(mhw, offset_Address);
                if (actualSequence == 0)
                {
                    // wait for init of Steamworks
                    return null;
                }

                var orderBytes = BitConverter.GetBytes(actualSequence);
                // Some shitty logic suggested by https://github.com/Geobryn which fixes the accuracy
                if (orderBytes[0] == 2 && orderBytes[1] == 0 && orderBytes[2] == 1)
                {
                    orderBytes[0] = 1;
                    orderBytes[1] = 2;
                    orderBytes[2] = 0;
                }
                else
                if (orderBytes[0] == 1 && orderBytes[1] == 2 && orderBytes[2] == 0)
                {
                    orderBytes[0] = 2;
                    orderBytes[1] = 0;
                    orderBytes[2] = 1;
                }

                Dictionary<VirtualKeyCode, int> keyOrder = new Dictionary<VirtualKeyCode, int>()
                {
                    { Settings.KeyCodesToPress[0], orderBytes[0] },
                    { Settings.KeyCodesToPress[1], orderBytes[1] },
                    { Settings.KeyCodesToPress[2], orderBytes[2] },
                };

                var ordered = keyOrder.OrderBy(x => x.Value).ToList();
                Logger.LogInfo($"Pressing {string.Join(" -> ", ordered.Take(3).Select(x => x.Key.ToString()))}");

                return ordered;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Extracting Correct Sequence: {ex.Message}");

                return null;
            }
        }

        private static void PressKey(InputSimulator sim, VirtualKeyCode key, bool delay = false)
        {
            Logger.LogInfo($"Pressing: {key}!");

            if (delay)
            {
                sim.Keyboard.KeyDown(key);
                sim.Keyboard.Sleep(100);
                sim.Keyboard.KeyUp(key);

                return;
            }

            sim.Keyboard.KeyPress(key);
        }

        private static void HookKeyboardEvents()
        {
            Task.Run(() =>
            {
                api.CreateKeyboardHook((character) =>
                {
                    if (character.KeyCode == (KeyCode)Settings.KeyCodeStart)
                    {
                        shouldStart = true;
                        Logger.LogInfo($"Captured Start!");
                    }

                    if (character.KeyCode == (KeyCode)Settings.KeyCodeStop)
                    {
                        ct.Cancel();

                        shouldStart = true;
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
