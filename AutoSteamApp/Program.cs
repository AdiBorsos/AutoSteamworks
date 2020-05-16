using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoSteamApp.Core;
using AutoSteamApp.Process_Memory;
using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;
using Keystroke.API;
using Logging;

namespace AutoSteamApp
{

    class Program
    {

        #region Fields

        #region Properties

        /// <summary>
        /// The name of the process. Ideally this should be MonsterHunterWorld as this is likely never to change.
        /// </summary>
        private const string ProcessName = "MonsterHunterWorld";

        #endregion


        #region Object References

        /// <summary>
        /// Random object used to when random mode is selected.
        /// </summary>
        private static Random rnd = new Random();

        /// <summary>
        /// Keystroke object used to abstract keyboard hooks.
        /// </summary>
        private static KeystrokeAPI api = new KeystrokeAPI();

        /// <summary>
        /// The actual Monster Hunter process.
        /// </summary>
        private static Process mhw;

        /// <summary>
        /// Cancellation token used to signal when to stop reading process memory etc.
        /// </summary>
        private static CancellationTokenSource ct = new CancellationTokenSource();

        #endregion

        #region Read-only Code Dictionaries

        /// <summary>
        /// Still learning what this does
        /// TODO: update this summary
        /// </summary>
        private static readonly Dictionary<VirtualKeyCode, int> keyOrder = new Dictionary<VirtualKeyCode, int>()
        {
            { VirtualKeyCode.VK_A, 999 },
            { VirtualKeyCode.VK_W, 999 },
            { VirtualKeyCode.VK_D, 999 },
            { VirtualKeyCode.VK_Q, 999 },
            { VirtualKeyCode.VK_Z, 999 },
        };

        /// <summary>
        /// Still learning what this does
        /// TODO: update this summary
        /// </summary>
        private static readonly Dictionary<int, List<int>> rndPatterns = new Dictionary<int, List<int>>()
        {
            { 0, new List<int> { 0, 1, 2 } },
            { 1, new List<int> { 1, 0, 2 } },
            { 2, new List<int> { 2, 0, 1 } },
            { 3, new List<int> { 0, 2, 1 } },
            { 4, new List<int> { 2, 1, 0 } },
            { 5, new List<int> { 1, 2, 0 } }
        };

        #endregion

        #endregion

        #region Methods

        static void Main(string[] args)
        {
            // Set the configuration file to the specified path
            AppDomain.CurrentDomain.SetupInformation.ConfigurationFile = ".config";
            SteamworkAutomaton automater = new SteamworkAutomaton();
            Log.Message("Press Any Key to begin.");
            Console.ReadKey();

            // Create a cancellation token for the thread
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            // Spawn a task to do the work in a separate thread
            Task t = Task.Run(() => { automater.Run(token); });

            Log.Message("Enter quit to stop");
            while (Console.ReadLine() != "quit")
            {
                // Wait for exit command
            }

            // Quit, as well as all underlying threads
            Environment.Exit(0);
        }
    }
}

//private static void DoRandomWork()
//{
//    if (mhw != null && !ct.IsCancellationRequested)
//    {
//        InputSimulator sim = new InputSimulator();
//        while (!shouldStop && !ct.IsCancellationRequested)
//        {
//            List<KeyValuePair<VirtualKeyCode, int>> orderBytes = GetRandomSequence();

//            foreach (var item in orderBytes)
//            {
//                PressKey(sim, item.Key, true);
//            }

//            PressKey(sim, (VirtualKeyCode)Settings.KeyCutsceneSkip, true);

//            PressKey(sim, VirtualKeyCode.SPACE, true);
//        }

//        api.Dispose();
//    }
//}

//private static List<KeyValuePair<VirtualKeyCode, int>> GetRandomSequence()
//{
//    List<int> orderBytes = rndPatterns[rnd.Next(0, 5)];

//    if (Settings.IsAzerty)
//    {
//        keyOrder[VirtualKeyCode.VK_Q] = orderBytes[0];   // Q
//        keyOrder[VirtualKeyCode.VK_Z] = orderBytes[1];   // Z
//        keyOrder[VirtualKeyCode.VK_D] = orderBytes[2];   // D
//    }
//    else
//    {
//        keyOrder[VirtualKeyCode.VK_A] = orderBytes[0];   // A
//        keyOrder[VirtualKeyCode.VK_W] = orderBytes[1];   // W
//        keyOrder[VirtualKeyCode.VK_D] = orderBytes[2];   // D
//    }

//    return keyOrder.OrderBy(x => x.Value).Take(3).ToList();
//}

//private static void DoWork(bool isSmartRun = true)
//{
//    if (mhw != null && !ct.IsCancellationRequested)
//    {

//        // Initialize the input simulator object     
//        InputSimulator sim = new InputSimulator();

//        SaveData saveDat = new SaveData(mhw, ct);
//        short gauge = saveDat.SteamGauge;

//// Set the memory address for the steamworks combo values
//ulong starter = Settings.Off_Base + Settings.Off_SteamworksCombo;

//var pointerAddress = MemoryHelper.Read<ulong>(mhw, starter);
//// offset the address
//var offset_Address = pointerAddress + 0x350;
//var offset_buttonPressState = offset_Address + 8;

//var oldFuelValue = sd.NaturalFuel + sd.StoredFuel;
//var fuelPerRound = 10;

//while(true)
//{
//    // Logger.LogInfo($"Gauge Data {sd.SteamGauge}!");
//    Console.WriteLine("Natural Fuel: " + sd.NaturalFuel);
//    Console.WriteLine("Bonus Time: " + sd.BonusTime);
//    Console.WriteLine("Steam Gauge: " + sd.SteamGauge);
//    Console.WriteLine("Stored Fuel: " + sd.StoredFuel);
//    Console.WriteLine("===============================================");

//    while (!shouldStop)
//        Thread.Sleep(1000);

//    shouldStop = false;

//    // value of the offset address
//    //List<KeyValuePair<VirtualKeyCode, int>> ordered =
//    //    isSmartRun ?
//    //        ExtractCorrectSequence(mhw, offset_Address) :
//    //        GetRandomSequence();

//    //if (ordered == null)
//    //{
//    //    Logger.LogInfo("The Steamworks minigame is not started. Please enter the minigame and Press 'Space' so that you see the first letters on your screen.");

//    //    // try again..
//    //    continue;
//    //}

//    //int index = 0;
//    //while (index < 3)
//    //{
//    //    try
//    //    {
//    //        var before = MemoryHelper.Read<byte>(mhw, offset_buttonPressState);

//    //        var item = ordered[index];

//    //        byte after = before;
//    //        while (before == after && !ct.IsCancellationRequested)
//    //        {
//    //            PressKey(sim, item.Key);

//    //            after = MemoryHelper.Read<byte>(mhw, offset_buttonPressState);
//    //        }

//    //        index++;
//    //    }
//    //    catch (Exception ex)
//    //    {
//    //        Logger.LogError($"Error trying to press button sequence -> {ex.Message}");
//    //    }
//    //}

//    //// Small work around to avoid blocking when running x10 fuel per sequence
//    //if (oldFuelValue - sd.NaturalFuel - sd.StoredFuel == 100)
//    //{
//    //    fuelPerRound = 100;
//    //}
//    //else
//    //{
//    //    fuelPerRound = 10;
//    //}

//    //oldFuelValue = sd.NaturalFuel + sd.StoredFuel;

//    //if (shouldStop)
//    //{
//    //    break;
//    //}

//    //var currentState = MemoryHelper.Read<byte>(mhw, offset_buttonPressState);
//    //while (currentState != (int)ButtonPressingState.BeginningOfSequence && !ct.IsCancellationRequested)
//    //{
//    //    Thread.Sleep(50);

//    //    try
//    //    {
//    //        PressKey(sim, (VirtualKeyCode)Settings.KeyCutsceneSkip, true);

//    //        // no more fuel
//    //        if (currentState == (int)ButtonPressingState.EndOfGame)
//    //        {
//    //            if (sd.NaturalFuel + (sd.StoredFuel * (Settings.ShouldConsumeAllFuel ? 1 : 0)) < fuelPerRound)
//    //            {
//    //                Logger.LogInfo(
//    //                    string.Format(
//    //                        "No more {0}fuel, stopping bot.",
//    //                        Settings.ShouldConsumeAllFuel == false ? "Natural " : string.Empty));

//    //                shouldStop = true;
//    //                break;
//    //            }

//    //            if (sd.SteamGauge == 0)
//    //            {
//    //                PressKey(sim, VirtualKeyCode.SPACE, true);
//    //            }
//    //        }

//    //        if (shouldStop)
//    //        {
//    //            break;
//    //        }

//    //        currentState = MemoryHelper.Read<byte>(mhw, offset_buttonPressState);
//    //    }
//    //    catch (Exception ex)
//    //    {
//    //        Logger.LogError($"Trying to finish up combo: {ex.Message}");
//    //    }
//    //}
//}

//api.Dispose();


//private static List<KeyValuePair<VirtualKeyCode, int>> ExtractCorrectSequence(Process mhw, ulong offset_Address)
//{
//    try
//    {
//        Thread.Sleep(rnd.Next((int)Settings.DelayBetweenCombo));

//        var actualSequence = MemoryHelper.Read<int>(mhw, offset_Address);
//        if (actualSequence == 0)
//        {
//            // wait for init of Steamworks
//            return null;
//        }

//        var orderBytes = BitConverter.GetBytes(actualSequence);
//        // Some shitty logic suggested by https://github.com/Geobryn which fixes the accuracy
//        if (orderBytes[0] == 2 && orderBytes[1] == 0 && orderBytes[2] == 1)
//        {
//            orderBytes[0] = 1;
//            orderBytes[1] = 2;
//            orderBytes[2] = 0;
//        }
//        else
//        if (orderBytes[0] == 1 && orderBytes[1] == 2 && orderBytes[2] == 0)
//        {
//            orderBytes[0] = 2;
//            orderBytes[1] = 0;
//            orderBytes[2] = 1;
//        }

//        if (Settings.IsAzerty)
//        {
//            keyOrder[VirtualKeyCode.VK_Q] = int.Parse(((char)(orderBytes[0] + 0x30)).ToString());   // Q
//            keyOrder[VirtualKeyCode.VK_Z] = int.Parse(((char)(orderBytes[1] + 0x30)).ToString());   // Z
//            keyOrder[VirtualKeyCode.VK_D] = int.Parse(((char)(orderBytes[2] + 0x30)).ToString());   // D
//        }
//        else
//        {
//            keyOrder[VirtualKeyCode.VK_A] = int.Parse(((char)(orderBytes[0] + 0x30)).ToString());   // A
//            keyOrder[VirtualKeyCode.VK_W] = int.Parse(((char)(orderBytes[1] + 0x30)).ToString());   // W
//            keyOrder[VirtualKeyCode.VK_D] = int.Parse(((char)(orderBytes[2] + 0x30)).ToString());   // D
//        }

//        var ordered = keyOrder.OrderBy(x => x.Value).ToList();
//        Logger.LogInfo($"Pressing {string.Join(" -> ", ordered.Take(3).Select(x => x.Key.ToString()))}");

//        return ordered;
//    }
//    catch (Exception ex)
//    {
//        Logger.LogError($"Extracting Correct Sequence: {ex.Message}");

//        return null;
//    }
//}

//private static void PressKey(InputSimulator sim, VirtualKeyCode key, bool delay = false)
//{
//    while (!IsCurrentActiveMHW())
//    {
//        Logger.LogInfo("MHW not active/focused. Waiting ...");
//    }

//    Logger.LogInfo($"Pressing: {key}!");

//    if (Settings.UseBackgroundKeyPress)
//    {
//        Logger.LogInfo($"You cheeky bastard. This doesn't work yet ..Please switch the flag back.");
//        //mhw.WaitForInputIdle();
//        //var keyMap = new Key((Messaging.VKeys)key);

//        //keyMap.PressBackground(mhw.MainWindowHandle);
//    }
//    else
//    {
//        if (delay)
//        {
//            sim.Keyboard.KeyDown(key);
//            sim.Keyboard.Sleep(100);
//            sim.Keyboard.KeyUp(key);

//            return;
//        }

//        sim.Keyboard.KeyPress(key);
//    }
//}

//#endregion

//#region Helpers

///// <summary>
///// Returns the process which contains the process name as defined in the fields
///// </summary>
///// <returns></returns>
//static Process GetMHW()
//{          
//    //Retrieve all processes with defined process name
//    var processes = Process.GetProcessesByName(ProcessName);
//    try
//    {
//        // Try to return the first one
//        return processes.FirstOrDefault(p => p != null && p.ProcessName.Equals(ProcessName) && !p.HasExited);
//    }
//    catch
//    {
//        // Log if not found
//        Logger.LogError($"Looks like the game is not running. It should...");
//        return null;
//    }        
//}

#endregion




