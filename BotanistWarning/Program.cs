using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BotanistWarning.Core.Utility;
using BotanistWarning.Model;
using Keystroke.API;

namespace BotanistWarning
{
    class Program
    {
        private const string ProcessName = "MonsterHunterWorld";
        private static volatile bool shouldRun = false;

        private static Random rnd = new Random();
        private static KeystrokeAPI api = new KeystrokeAPI();

        static void Main(string[] args)
        {
            HookKeyboardEvents();

            Process mhw = GetMHW();
            while (mhw == null)
            {
                Thread.Sleep(1000);
                mhw = GetMHW();
            }

            DataFeeder sd = new DataFeeder(mhw);

            while (true)
            {
                sd.Load();
                
                if (sd.HarvestBoxCount > Settings.ThresholdHarvest)
                {
                    Logger.LogInfo("LogMessaged asdasdasdasdasda");
                }

                if (sd.FertilizerItems.Any(fert => fert.Duration != 0 && fert.Duration < Settings.ThresholdDuration))
                {
                    Logger.LogInfo("LogMessaged duratioooon");
                }

                Thread.Sleep(Settings.Delay * 1000);
            }
        }

        private static void HookKeyboardEvents()
        {
            Task.Run(() =>
            {
                api.CreateKeyboardHook((character) =>
                {
                    if (character.KeyCode == (KeyCode)Settings.KeyCodeShow)
                    {
                        shouldRun = true;
                        Logger.LogInfo($"Captured Show!");
                    }

                    if (character.KeyCode == (KeyCode)Settings.KeyCodeHide)
                    {
                        shouldRun = false;

                        Logger.LogInfo($"Captured Hide!");

                        Application.Exit();
                    }
                });

                Application.Run();
            });
        }

        private static Process GetMHW()
        {
            var processes = Process.GetProcesses();
            try
            {
                return processes.FirstOrDefault(p => p != null && p.ProcessName.Equals(ProcessName) && !p.HasExited);
            }
            catch
            {
                Logger.LogError($"Error trying to find '{ProcessName}' process.");
            }

            Logger.LogError($"Looks like the game is not running. It should...");

            return null;
        }
    }
}
