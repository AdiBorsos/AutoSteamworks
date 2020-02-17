using System.Configuration;
using System.Linq;

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
                    return uint.TryParse(ConfigurationManager.AppSettings["DelayBetweenKeys"], out _DelayBetweenKeys) ?
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
                    return uint.TryParse(ConfigurationManager.AppSettings["DelayBetweenCombo"], out _DelayBetweenCombo) ?
                        _DelayBetweenCombo :
                        (_DelayBetweenCombo = 500);
                }

                return _DelayBetweenCombo;
            }
        }

        public static bool EnableLog
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
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
                    return bool.TryParse(ConfigurationManager.AppSettings["IsAzerty"], out _isAzerty) ?
                        _isAzerty :
                        (_isAzerty = false);
                }

                return _isAzerty;
            }
        }
    }
}