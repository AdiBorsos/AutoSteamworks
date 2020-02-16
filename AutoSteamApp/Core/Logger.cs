using System;
using System.IO;
using System.Text;

namespace AutoSteamApp.Core
{
    public static class Logger
    {
        private static readonly StringBuilder logBuffer = new StringBuilder();

        public static void LogInfo(string message)
        {
            if (Settings.EnableLog)
            {
                message = $"[Info][{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}]: {message}";
                Console.WriteLine(message);

                try
                {
                    File.AppendAllText("log.txt", message);
                }
                catch
                {
                    // meh, keep your secrets then :)
                }
            }
        }

        public static void LogError(string message)
        {
            message = $"[Error][{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}]: {message}";
            Console.WriteLine(message);
            
            try
            {
                File.AppendAllText("log.txt", message);
            }
            catch
            {
                // meh, keep your secrets then :)
            }
        }
    }
}
