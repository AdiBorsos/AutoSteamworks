using GregsStack.InputSimulatorStandard.Native;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoSteamApp.Helpers
{
    public static class ConfigurationReader
    {

        #region Constant parameters

        /// <summary>
        /// The maximum number of seconds the save data will attempt to find a slot before throwing an exception
        /// </summary>
        public const double MaxTimeSlotNumberSeconds = 30;

        #endregion

        #region Dynamic parameters

        /// <summary>
        /// Returns whether the config file was located, and keys were able to be loaded properly
        /// </summary>
        public static bool ConfigLoadedProperly
        {
            get
            {
                return (ConfigurationManager.AppSettings.AllKeys.Length > 0);
            }
        }

        /// <summary>
        /// Returns whether or not the application is being run in debug mode.
        /// <para></para>
        /// Debug is defined by either the executing assembly, or in the config file.
        /// </summary>
        public static bool IsDebug
        {
            get
            {
                bool AssemblyDebug = false;
#if DEBUG
                AssemblyDebug = true;
#endif
                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "Debug"))
                {
                    if (bool.TryParse(ConfigurationManager.AppSettings["Debug"].Trim(), out bool ConfigDebug))
                        AssemblyDebug |= ConfigDebug;
                }
                return AssemblyDebug;
            }
        }

        /// <summary>
        /// Returns the Version of the assembly.
        /// </summary>
        public static Version ApplicationVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        /// <summary>
        /// Absolute path to a log file. If omitted, no logs are saved.
        /// </summary>
        public static string LogFile
        {
            get
            {
                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "LogFile"))
                {
                    return ConfigurationManager.AppSettings["LogFile"].Trim();
                }
                return null;
            }
        }

        /// <summary>
        /// Changes keyboard input to match an azerty keyboard
        /// </summary>
        public static bool IsAzerty
        {
            get
            {
                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "IsAzerty"))
                {
                    if (bool.TryParse(ConfigurationManager.AppSettings["IsAzerty"].Trim(), out bool azerty))
                        return azerty;
                }
                return false;
            }
        }

        /// <summary>
        /// Delay used to determine how long to wait between sequences
        /// </summary>
        public static int RandomInputDelay
        {
            get
            {
                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "DelayBetweenCombo"))
                {
                    if (int.TryParse(ConfigurationManager.AppSettings["DelayBetweenCombo"].Trim(), out int delay))
                        return delay;
                }
                return 50;
            }
        }

        /// <summary>
        /// key used to skip cutscenes
        /// </summary>
        public static VirtualKeyCode KeyCutsceneSkip
        {
            get
            {
                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "keyCutsceneSkip"))
                {
                    if (int.TryParse(ConfigurationManager.AppSettings["keyCutsceneSkip"].Trim(), out int keycode))
                    {
                        try
                        {
                            VirtualKeyCode key = (VirtualKeyCode)keycode;

                            return key;
                        }
                        catch (Exception ex)
                        {
                            return VirtualKeyCode.VK_X;
                        }
                    }
                    return VirtualKeyCode.VK_X;
                }
                else
                {
                    return VirtualKeyCode.VK_X;
                }

            }
        }

        #endregion

    }
}
