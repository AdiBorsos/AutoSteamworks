using AutoSteamApp.Configuration;
using AutoSteamApp.ProcessMemory;
using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;
using Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AutoSteamApp.Helpers
{
    public static class StaticHelpers
    {
        /// <summary>
        /// Returns the process which contains the process name as defined in the fields
        /// </summary>
        /// <returns></returns>
        public static Process GetMHWProcess()
        {
            //Retrieve all processes with defined process name
            var processes = Process.GetProcessesByName(MHWMemoryValues.ProcessName);

            // Try to return the first one
            return processes.First(p => p != null && p.ProcessName.Equals(MHWMemoryValues.ProcessName) && !p.HasExited);
        }

        /// <summary>
        /// Logs the current configuration settings to the user
        /// </summary>
        public static void DisplayConfig()
        {
            Log.Message("Configuration Settings:");
            Log.Message("\tApplication Version: " + ConfigurationReader.ApplicationVersion);
            Log.Message("\tIs Debug: " + ConfigurationReader.IsDebug);

            if (!ConfigurationReader.ConfigLoadedProperly)
            {
                Log.Warning("Config file was unable to load.");
                return;
            }
            Log.Message("\tIs Azerty: " + ConfigurationReader.IsAzerty);
            Log.Message("\tLog File: " + (ConfigurationReader.LogFile ?? "Not supplied"));
            Log.Message("\tRandom Run: " + ConfigurationReader.RandomRun);
            Log.Message("\tInput Delay on Random Run: " + ConfigurationReader.RandomInputDelay);
            Log.Message("\tCutscene skip key: " + ConfigurationReader.KeyCutsceneSkip.ToString());
            Log.Message("\tCommon Reward Success Rate: " + ConfigurationReader.CommonSuccessRate.ToString("#.##"));
            Log.Message("\tRare Reward Success Rate: " + ConfigurationReader.RareSuccessRate.ToString("#.##"));
            Log.Message("\tMaximum Wait for Determining Slot: " + ConfigurationReader.MaxTimeSlotNumberSeconds);
            Log.Message("\tStop at Fuel Amount: " + ConfigurationReader.StopAtFuelAmount);
            Log.Message("\tOnly use Natural Fuel: " + ConfigurationReader.OnlyUseNaturalFuel);
            Log.Message("\tShould Auto-Quit: " + ConfigurationReader.ShouldAutoQuit);
            Log.Message("");
        }


        /// <summary>
        /// Sets the proper log methods depending on the configuration found in the .config file
        /// </summary>
        public static void SetLogger()
        {
            // Initialize the different log types
            Log.LogTypes LoggingTypes = Log.LogTypes.Message | Log.LogTypes.Warning;

            // If we're in debug mode, heighten the logging which takes place
            if (ConfigurationReader.IsDebug)
            {
                LoggingTypes |= Log.LogTypes.Debug | Log.LogTypes.Exception | Log.LogTypes.Error;

                // Check if we need to write logs to a file
                string logFile = ConfigurationReader.LogFile;
                if (!string.IsNullOrEmpty(logFile))
                {
                    // Try to create a stream using the log file
                    try
                    {
                        FileStream logStream = File.Create(logFile);
                        Log.SetStream(logStream, true, LoggingTypes);
                        return;
                    }
                    // In the event of failure, log to console instead
                    catch (Exception e)
                    {
                        Log.SetStream(Console.OpenStandardOutput(), false, LoggingTypes);
                        Log.Exception(e, logFile);
                        return;
                    }
                }
                // If no log file is specified, log to console instead
                Log.SetStream(Console.OpenStandardOutput(), false, LoggingTypes);
                //Log.Warning("No log file specified. All logs written to console.");
                return;
            }
            // If not in debug mode, write messages to console
            Log.SetStream(Console.OpenStandardOutput(), false, LoggingTypes);
        }

        /// <summary>
        /// Sets the console title of the app
        /// </summary>
        public static void SetConsoleTitle()
        {
            Version ver = ConfigurationReader.ApplicationVersion;
            string title = "Auto Steamworks Version:" + ver.Major + "." + ver.Minor;
            title += "        ";
            title += "Supported MHW:IB Version: " + MHWMemoryValues.SupportedGameVersion;
            Console.Title = title;
        }

        /// <summary>
        /// Converts the byte sequence to an array of Virtual Key Codes using the index of the key as the order in which to press
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static VirtualKeyCode[] KeyCodeSequenceFromBytes(byte[] sequence)
        {

            Dictionary<int, VirtualKeyCode> dict = new Dictionary<int, VirtualKeyCode>();

            if (ConfigurationReader.IsAzerty)
            {
                dict.Add(sequence[0], VirtualKeyCode.VK_Q);
                dict.Add(sequence[1], VirtualKeyCode.VK_Z);
                dict.Add(sequence[2], VirtualKeyCode.VK_D);
            }
            else
            {
                dict.Add(sequence[0], VirtualKeyCode.VK_A);
                dict.Add(sequence[1], VirtualKeyCode.VK_W);
                dict.Add(sequence[2], VirtualKeyCode.VK_D);
            }

            VirtualKeyCode[] retVal = dict.OrderBy(x => x.Key).Select(y => y.Value).ToArray();
            // Return the virtual key code values ordered by the index assigned from the byte sequence
            Log.Debug("Sequence found: [" + string.Join(", ", retVal.Select(x => x.ToString())) + "]");
            return retVal;
        }

        /// <summary>
        /// Generates a random sequence of three key codes
        /// </summary>
        /// <returns></returns>
        public static VirtualKeyCode[] GetRandomSequence(Random rng)
        {
            VirtualKeyCode[] retVal = new VirtualKeyCode[3];
            if (ConfigurationReader.IsAzerty)
            {
                retVal[0] = VirtualKeyCode.VK_Q;
                retVal[1] = VirtualKeyCode.VK_Z;
                retVal[2] = VirtualKeyCode.VK_D;
            }
            else
            {
                retVal[0] = VirtualKeyCode.VK_A;
                retVal[1] = VirtualKeyCode.VK_W;
                retVal[2] = VirtualKeyCode.VK_D;
            }
            return retVal.Shuffle(rng);
        }

        /// <summary>
        /// Simulates a key down, followed by key up event for a key
        /// </summary>
        /// <param name="sim">The input simulator to use.</param>
        /// <param name="key">The key to press.</param>
        /// <param name="delay">The delay to wait between key down and key up.</param>
        public static void PressKey(InputSimulator sim, VirtualKeyCode key, int delay = 0)
        {
            sim.Keyboard.KeyDown(key);
            sim.Keyboard.Sleep(delay);
            sim.Keyboard.KeyUp(key);
            return;
        }

    }
}
