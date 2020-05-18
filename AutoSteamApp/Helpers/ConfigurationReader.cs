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

        #endregion

    }
}
