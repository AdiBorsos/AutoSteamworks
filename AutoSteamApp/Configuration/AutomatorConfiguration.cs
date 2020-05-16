using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoSteamApp.Helpers
{
    public static class AutomatorConfiguration
    {
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
    }
}
