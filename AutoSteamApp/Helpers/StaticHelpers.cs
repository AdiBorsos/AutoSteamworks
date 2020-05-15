using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoSteamApp.Helpers
{
    public class StaticHelpers
    {
        public static bool IsDebug
        {
            get
            {
                bool debug = false;
#if DEBUG
                debug = true;
#endif
                return debug;
            }
        }

        public static Version ApplicationVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

    }
}
