using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSteamApp.Core
{
    public static class Logger
    {
        public static void Log(string message)
        {
            if (Settings.EnableLog)
            {
                Console.WriteLine(message);
            }
        }
    }
}
