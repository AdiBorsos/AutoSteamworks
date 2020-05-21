using GregsStack.InputSimulatorStandard.Native;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoSteamApp.Configuration
{
    public static class ConfigurationReader
    {

        /// <summary>
        /// Method for loading a config file
        /// </summary>
        /// <param name="configLocation"></param>
        public static void LoadConfig(string configLocation = null)
        {
            if (configLocation != null)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.File = configLocation;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }

        #region Dynamic parameters

        /// <summary>
        /// Returns whether the config file was located, and keys were able to be loaded properly
        /// </summary>
        public static bool ConfigLoadedProperly
        {
            get
            {
                // If we have already read the value once
                if (_ConfigLoadedProperly.HasValue)
                    return _ConfigLoadedProperly.Value;

                // Value is determined by the fact of any keys being loaded
                _ConfigLoadedProperly = ConfigurationManager.AppSettings.AllKeys.Length > 0;
                return _ConfigLoadedProperly.Value;
            }
        }
        static bool? _ConfigLoadedProperly = null;

        /// <summary>
        /// Returns whether or not the application is being run in debug mode.
        /// <para></para>
        /// Debug is defined by either the executing assembly, or in the config file.
        /// </summary>
        public static bool IsDebug
        {
            get
            {

                if (_IsDebug.HasValue)
                    return _IsDebug.Value;

                _IsDebug = ConfigurationDefaults.DefaultIsDebug;
#if DEBUG
                _IsDebug = true;
#endif
                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "Debug"))
                {
                    if (bool.TryParse(ConfigurationManager.AppSettings["Debug"].Trim(), out bool ConfigDebug))
                        _IsDebug |= ConfigDebug;
                }
                return _IsDebug.Value;
            }
        }
        static bool? _IsDebug = null;

        /// <summary>
        /// Returns the Version of the assembly.
        /// </summary>
        public static Version ApplicationVersion
        {
            get
            {
                if (_ApplicationVersion != null)
                    return _ApplicationVersion;

                _ApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version;
                return _ApplicationVersion;
            }
        }
        static Version _ApplicationVersion = null;

        /// <summary>
        /// Absolute path to a log file. If omitted, no logs are saved.
        /// </summary>
        public static string LogFile
        {
            get
            {
                if (!string.IsNullOrEmpty(_LogFile))
                    return _LogFile;

                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "LogFile"))
                {
                    string retVal = ConfigurationManager.AppSettings["LogFile"].Trim();
                    if (!string.IsNullOrEmpty(retVal))
                        _LogFile = retVal;
                }

                return _LogFile;
            }
        }
        static string _LogFile = null;

        /// <summary>
        /// Changes keyboard input to match an azerty keyboard
        /// </summary>
        public static bool IsAzerty
        {
            get
            {
                if (_IsAzerty.HasValue)
                    return _IsAzerty.Value;

                _IsAzerty = ConfigurationDefaults.DefaultIsAzerty;

                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "IsAzerty"))
                {
                    if (bool.TryParse(ConfigurationManager.AppSettings["IsAzerty"].Trim(), out bool azerty))
                        _IsAzerty = azerty;
                }
                return _IsAzerty.Value;
            }
        }
        static bool? _IsAzerty = null;

        /// <summary>
        /// Delay used to determine how long to wait between sequences
        /// </summary>
        public static int RandomInputDelay
        {
            get
            {
                if (_RandomInputDelay.HasValue)
                    return _RandomInputDelay.Value;

                _RandomInputDelay = ConfigurationDefaults.DefaultRandomInputDelay;

                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "DelayBetweenCombo"))
                {
                    if (int.TryParse(ConfigurationManager.AppSettings["DelayBetweenCombo"].Trim(), out int delay))
                        _RandomInputDelay = delay;
                }
                return _RandomInputDelay.Value;
            }
        }
        static int? _RandomInputDelay = null;

        /// <summary>
        /// key used to skip cutscenes
        /// </summary>
        public static VirtualKeyCode KeyCutsceneSkip
        {
            get
            {

                if (_KeyCutsceneSkip.HasValue)
                    return _KeyCutsceneSkip.Value;

                _KeyCutsceneSkip = ConfigurationDefaults.DefaultKeyCutsceneSkip;

                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "keyCutsceneSkip"))
                    if (int.TryParse(ConfigurationManager.AppSettings["keyCutsceneSkip"].Trim(), out int keycode))
                        try
                        {
                            VirtualKeyCode key = (VirtualKeyCode)keycode;
                            _KeyCutsceneSkip = key;
                        }
                        catch
                        {
                        }

                return _KeyCutsceneSkip.Value;

            }
        }
        static VirtualKeyCode? _KeyCutsceneSkip = null;

        /// <summary>
        /// Sets if the values should be random
        /// </summary>
        public static bool RandomRun
        {
            get
            {
                if (_RandomRun.HasValue)
                    return _RandomRun.Value;

                _RandomRun = ConfigurationDefaults.DefaultRandomRun;

                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "RandomRun"))
                    if (bool.TryParse(ConfigurationManager.AppSettings["RandomRun"].Trim(), out bool random))
                        _RandomRun = random;

                return _RandomRun.Value;
            }
        }
        static bool? _RandomRun = null;

        #endregion

    }
}
