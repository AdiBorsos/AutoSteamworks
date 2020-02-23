using System;
using System.Configuration;
using System.Linq;
using BotanistWarning.Core.Utility;
using Keystroke.API;

namespace BotanistWarning
{
    public static class Settings
    {
        public static string SupportedGameVersion = "404549";

        public static ulong Off_Base = 0x140000000;
        public static ulong Off_SteamworksCombo = 0x4D6B970;

        public static ulong Off_SaveData = 0x4DF6F00;
        public static ulong Off_DiffSlot = 0x27E9F0; // start of each save slot data slotnr * off


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

        private static int _keyCodeShow = -1;
        public static int KeyCodeShow
        {
            get
            {
                if (_keyCodeShow != -1) { return _keyCodeShow; }

                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "KeyCodeShow"))
                {
                    if (int.TryParse(ConfigurationManager.AppSettings["KeyCodeShow"].Trim(), out _keyCodeShow))
                    {
                        try
                        {
                            KeyCode key = (KeyCode)_keyCodeShow;

                            return _keyCodeShow;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"Invalid number for 'Start' keycode: [{_keyCodeShow}]. Will use default instead! Exception: {ex.Message}");
                        }
                    }
                    else
                    {
                        Logger.LogError($"Invalid number for 'Start' keycode: [{ConfigurationManager.AppSettings["KeyCodeShow"]}]. Will use default instead!");
                    }
                }

                return (_keyCodeShow = 45);
            }
        }


        private static int _keyCodeHide = -1;
        public static int KeyCodeHide
        {
            get
            {
                if (_keyCodeHide != -1) { return _keyCodeHide; }

                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "KeyCodeHide"))
                {
                    if (int.TryParse(ConfigurationManager.AppSettings["KeyCodeHide"].Trim(), out _keyCodeHide))
                    {
                        try
                        {
                            KeyCode key = (KeyCode)_keyCodeHide;

                            return _keyCodeHide;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"Invalid number for 'Stop' keycode: [{_keyCodeHide}]. Will use default instead! Exception: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Logger.LogError($"Invalid number for 'Stop' keycode: [{ConfigurationManager.AppSettings["KeyCodeHide"]}]. Will use default instead!");
                }

                return (_keyCodeHide = 27);
            }
        }

        private static int _thresholdDuration = 4;
        public static int ThresholdDuration
        {
            get
            {
                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "WarnWhenDurationLowerThan"))
                {
                    return int.TryParse(ConfigurationManager.AppSettings["WarnWhenDurationLowerThan"].Trim(), out _thresholdDuration) ?
                        _thresholdDuration :
                        (_thresholdDuration = 4);
                }

                return _thresholdDuration;
            }
        }

        private static int _thresholdHarvest = 20;
        public static int ThresholdHarvest
        {
            get
            {
                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "WarnWhenBoxItemsHigherThan"))
                {
                    return int.TryParse(ConfigurationManager.AppSettings["WarnWhenBoxItemsHigherThan"].Trim(), out _thresholdHarvest) ?
                        _thresholdHarvest :
                        (_thresholdHarvest = 20);
                }

                return _thresholdHarvest;
            }
        }

        private static int _checkDelay = 10;
        public static int Delay
        {
            get
            {
                if (ConfigurationManager.AppSettings.AllKeys.Any(key => key == "CheckDelay"))
                {
                    return int.TryParse(ConfigurationManager.AppSettings["CheckDelay"].Trim(), out _checkDelay) ?
                        _checkDelay :
                        (_checkDelay = 20);
                }

                return _checkDelay;
            }
        }

        public static Overlay OverlaySettings { get; set; } = new Overlay();

        // Config template

        public class Overlay
        {
            public bool Enabled { get; set; } = true;
            public int[] Position { get; set; } = new int[2] { 0, 0 };
            public bool HideWhenGameIsUnfocused { get; set; } = false;
            public Harvestboxcomponent HarvestBoxComponent { get; set; } = new Harvestboxcomponent();
        }

        public class Harvestboxcomponent
        {
            public bool Enabled { get; set; } = true;
            public int[] Position { get; set; } = new int[2] { 1110, 30 };
        }

        public class Options
        {
            public bool CloseWhenGameCloses { get; set; } = true;
        }
    }
}