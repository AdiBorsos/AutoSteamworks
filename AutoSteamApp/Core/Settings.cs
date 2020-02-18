using System;
using System.Configuration;
using System.Linq;
using Keystroke.API;

namespace AutoSteamApp.Core
{
    public static class Settings
    {
        private static uint _DelayBetweenKeys = 500;
        public static uint DelayBetweenKeys
        {
            get
            {
                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "DelayBetweenKeys"))
                {
                    return uint.TryParse(ConfigurationManager.AppSettings["DelayBetweenKeys"].Trim(), out _DelayBetweenKeys) ?
                        _DelayBetweenKeys :
                        (_DelayBetweenKeys = 500);
                }

                return _DelayBetweenKeys;
            }
        }

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

                return _isAzerty;
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
    }
}