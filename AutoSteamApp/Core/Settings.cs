using System;
using System.Configuration;
using System.Linq;
using Keystroke.API;

namespace AutoSteamApp.Core
{
    public static class Settings
    {
        #region magic numbers
        public static string SupportedGameVersion = "421652"; // update 15.21.00

        public static ulong Off_Base = 0x140000000;
        public static ulong Off_SteamworksCombo = 0x4F859F0; // update 15.20.00

        public static ulong Off_SaveData = 0x5011710; //update 15.20.00
        public static ulong Off_DiffSlot = 0x26CC00; // start of each save slot data slotnr * off. update 15.20.00
        #endregion

        private static uint _DelayBetweenCombo = 500;
        public static uint DelayBetweenCombo
        {
            get
            {
                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "DelayBetweenCombo"))
                {
                    return uint.TryParse(ConfigurationManager.AppSettings["DelayBetweenCombo"].Trim(), out _DelayBetweenCombo) ?
                        _DelayBetweenCombo :
                        (_DelayBetweenCombo = 500);
                }

                return _DelayBetweenCombo;
            }
        }

        private static bool _isLogEnabled = false;
        public static bool EnableLog
        {
            get
            {
#if DEBUG
                return true;
#else
                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "IsLogEnabled"))
                {
                    return bool.TryParse(ConfigurationManager.AppSettings["IsLogEnabled"].Trim(), out _isLogEnabled) ?
                        _isLogEnabled :
                        (_isLogEnabled = false);
                }

                return _isLogEnabled;
#endif
            }
        }

        private static bool _isAzerty = false;
        public static bool IsAzerty
        {
            get
            {
                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "IsAzerty"))
                {
                    return bool.TryParse(ConfigurationManager.AppSettings["IsAzerty"].Trim(), out _isAzerty) ?
                        _isAzerty :
                        (_isAzerty = false);
                }

                return _isAzerty;
            }
        }

        private static bool _useBackgroundKeyPress = false;
        public static bool UseBackgroundKeyPress
        {
            get
            {
                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "UseBackgroundKeyPress"))
                {
                    return bool.TryParse(ConfigurationManager.AppSettings["UseBackgroundKeyPress"].Trim(), out _useBackgroundKeyPress) ?
                        _useBackgroundKeyPress :
                        (_useBackgroundKeyPress = false);
                }

                return _useBackgroundKeyPress;
            }
        }

        private static bool _ShouldConsumeAllFuel = false;
        public static bool ShouldConsumeAllFuel
        {
            get
            {
                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "ShouldConsumeAllFuel"))
                {
                    return bool.TryParse(ConfigurationManager.AppSettings["ShouldConsumeAllFuel"].Trim(), out _ShouldConsumeAllFuel) ?
                        _ShouldConsumeAllFuel :
                        (_ShouldConsumeAllFuel = false);
                }

                return _ShouldConsumeAllFuel;
            }
        }

        #region keycodes

        private static int _keyCodeStart = -1;
        public static int KeyCodeStart
        {
            get
            {
                if (_keyCodeStart != -1) { return _keyCodeStart; }

                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "keyCodeStart"))
                {
                    if (int.TryParse(ConfigurationManager.AppSettings["keyCodeStart"].Trim(), out _keyCodeStart)) 
                    {
                        try
                        {
                            KeyCode key = (KeyCode)_keyCodeStart;
                            
                            return _keyCodeStart;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"Invalid number for 'Start' keycode: [{_keyCodeStart}]. Will use default instead! Exception: {ex.Message}");
                        }
                    }
                    else
                    {
                        Logger.LogError($"Invalid number for 'Start' keycode: [{ConfigurationManager.AppSettings["keyCodeStart"]}]. Will use default instead!");
                    }
                }

                return (_keyCodeStart = 45);
            }
        }

        private static int _keyCodeStartRandom = -1;
        public static int KeyCodeStartRandom
        {
            get
            {
                if (_keyCodeStartRandom != -1) { return _keyCodeStartRandom; }

                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "keyCodeStartRandom"))
                {
                    if (int.TryParse(ConfigurationManager.AppSettings["keyCodeStartRandom"].Trim(), out _keyCodeStartRandom))
                    {
                        try
                        {
                            KeyCode key = (KeyCode)_keyCodeStartRandom;

                            return _keyCodeStartRandom;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"Invalid number for 'StartRandom' keycode: [{_keyCodeStartRandom}]. Will use default (F2) instead! Exception: {ex.Message}");
                        }
                    }
                    else
                    {
                        Logger.LogError($"Invalid number for 'StartRandom' keycode: [{ConfigurationManager.AppSettings["keyCodeStartNatural"]}]. Will use default (F2) instead!");
                    }
                }

                return (_keyCodeStartRandom = 112);
            }
        }



        private static int _keyCodeStop = -1;
        public static int KeyCodeStop
        {
            get
            {
                if (_keyCodeStop != -1) { return _keyCodeStop; }

                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "keyCodeStop"))
                {
                    if(int.TryParse(ConfigurationManager.AppSettings["keyCodeStop"].Trim(), out _keyCodeStop))
                    {
                        try
                        {
                            KeyCode key = (KeyCode)_keyCodeStop;

                            return _keyCodeStop;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"Invalid number for 'Stop' keycode: [{_keyCodeStop}]. Will use default instead! Exception: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Logger.LogError($"Invalid number for 'Stop' keycode: [{ConfigurationManager.AppSettings["keyCodeStop"]}]. Will use default instead!");
                }

                return (_keyCodeStop = 27);
            }
        }
        
        private static int _keyCutsceneSkip = -1;
        public static int KeyCutsceneSkip
        {
            get
            {
                if (_keyCutsceneSkip != -1) { return _keyCutsceneSkip; }

                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "keyCutsceneSkip"))
                {
                    if (int.TryParse(ConfigurationManager.AppSettings["keyCutsceneSkip"].Trim(), out _keyCutsceneSkip))
                    {
                        try
                        {
                            KeyCode key = (KeyCode)_keyCutsceneSkip;

                            return _keyCutsceneSkip;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"Invalid number for 'CutsceneSkip' keycode: [{_keyCutsceneSkip}]. Will use default instead! Exception: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Logger.LogError($"Invalid number for 'CutsceneSkip' keycode: [{ConfigurationManager.AppSettings["keyCutsceneSkip"]}]. Will use default instead!");
                }

                return (_keyCutsceneSkip = 88);
            }
        }
        #endregion
    }
}
