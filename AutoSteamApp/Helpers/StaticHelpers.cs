using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSteamApp.Helpers
{
    public static class StaticHelpers
    {
        /// <summary>
        /// Returns the process which contains the process name as defined in the fields
        /// </summary>
        /// <returns></returns>
        public static Process GetMHWProcess()
        {
            //Retrieve all processes with defined process name
            var processes = Process.GetProcessesByName(AutomatonConfiguration.ProcessName);
            try
            {
                // Try to return the first one
                return processes.FirstOrDefault(p => p != null && p.ProcessName.Equals(AutomatonConfiguration.ProcessName) && !p.HasExited);
            }
            catch
            {
                throw new Exception("Could not find a running MHW process.");
            }
        }
    }
}
