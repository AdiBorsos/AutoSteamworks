[![Donate](https://img.shields.io/badge/Support-Adi-blue)](https://www.paypal.me/adiborsos)
# Auto Steamworks 
![Auto Steamworks](/Documentation/Images/banner.png)

## About
---
Redesign by DaviesCooper (beta version) - [Github page](https://github.com/DaviesCooper/AutoSteamworks)

The Auto Steamworks is a tool used to automate the Monster Hunter World: Iceborne mini-game, the steamworks. By reading the memory values found within the the Monster Hunter World process, the automated is also able to determine the correct sequence to win every round.

Please use the [Github page](https://github.com/AdiBorsos/AutoSteamworks) to ask for features.


#### How To Install
---
###### Requires [.NET Framework 4.6.1](https://www.microsoft.com/en-us/download/details.aspx?id=49982) or greater


1. Download the .zip 
2. Extract to wherever you want.

- To uninstall simply delete the folder containing the exe.

#### How To Use
---
##### To start
###### Currently supported MHW:IB version: 410918
1. MHW:IB must be running, and you must have loaded into a character file.
2. Open the steamworks mini game. Press "start" to begin the game.
3. Execute AutoSteamworks.exe.
4. Upon being prompted, press any key to start.
5. Switch to the game window.

##### To stop
1. Switch to the AutoSteamworks application
2. type "quit" and press enter.

#### How to Configure
---
 - The default configuration file can be found in AutoSteamworks.exe.config. Editing the values in this file will effect how the Auto Steamworks runs.
 - If you wish to create multiple config files, you can pass an absolute config file path as the first argument when running the exe. The config file must be in the following xml format.
```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <appSettings>
    <add key="Config Key" value ="Config Value"/>
  </appSettings>
</configuration>
```

##### Configuration Keys


Key | Type | Default | Description
 --- | --- | --- | ---
 Debug | bool | false | Debug mode will cause for more verbose messages when running
 LogFile | string | "" | The absolute path to a file where you want all messages written to. If you are having issues, this might be beneficial to specify so that you can email the file to one of the contributors
 DelayBetweenCombo | + int | 50 | The delay between key presses. If the app is having issues synchronizing, this may be something to tweak
 RandomRun | bool | false | Setting this will cause the app to input completely random sequences. The app defaults to this mode if the game version you are running is not supported by the app.
 IsAzerty| bool | false | USed to indicate if your keyboard layout is azerty. false indicates qwerty.
 KeyCutsceneSkip | + int | 88 |The [key code integer representation](https://www.cambiaresearch.com/articles/15/javascript-char-codes-key-codes) of the key to press to skip cutscenes. Default is 88 which represents the **x** key
 CommonSuccessRate | Range[0.0,1.0] | 1.0 | The probability to win when the prize is a common reward
 RareSuccessRate | Range[0.0,1.0] | 1.0 | The probability to win when the prize is a rare reward
 MaxTimeSlotNumberSeconds | + int | 30 | The maximum amount of time the app should spend trying to determine which character slot you are using. This depends on how often your game writes to memory so slower pcs might need to increase this value
 OnlyUseNaturalFuel | bool | false | Should the app stop if stored fuel needs to be spent to continue?
 StopAtFuelAmount | +int | 0 | How much fuel you want to be left (i.e. play the game **until** I have this much or less fuel). Which fuel storage to look at for this check is determined by the OnlyUseNaturalFuel config key.
 AutoQuit | bool | false | Should the app auto quit when complete or if an error occurs, or wait for user indication to close?

 #### Contributors & Credits:
 ---
 - [DaviesCooper](https://github.com/DaviesCooper/AutoSteamworks)
	- complete revamp of the code and added functionality - many thanx :DaviesCooper 
	
 - [Geobryn](https://github.com/Geobryn)
   - Made it 100% accurate without writing memory
   - Added CutsceneSkip functionality
 - [Marcus101RR]( https://fearlessrevolution.com/memberlist.php?mode=viewprofile&u=438 ) 
 - - for his work on this [Cheat Table](https://fearlessrevolution.com/viewtopic.php?f=4&t=9923)
 - [r00telement](https://github.com/r00telement/SmartHunter) 
 - [gabrielefilipp](https://github.com/gabrielefilipp/SmartHunter)

# Pull requests are welcome.
