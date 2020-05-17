﻿using AutoSteamApp.Core;
using AutoSteamApp.Helpers;
using AutoSteamApp.Process_Memory;
using Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoSteamApp
{
    public class SteamworkAutomaton
    {

        // First thing is to get the MHWProcess
        Process mhwProcess;

        // Using the process, load the save data;
        SaveData saveData;

        public SteamworkAutomaton()
        {
            try
            {
                // First set the mhw process
                mhwProcess = StaticHelpers.GetMHWProcess();


            }
            catch (Exception e)
            {
                Exception error = new Exception("Failed to initialze Steamworks Automaton.", e);
                Log.Exception(error);
                throw error;
            }
        }






        public void Run(CancellationToken cts)
        {

            Environment.Exit(1);
        }
    }
}