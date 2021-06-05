# Auto Steamworks Exec
Supported version: 421471 (V15.11.01)
App Version: 19.0

Functionality
Reads game memory and finds the "expected" key pattern that the Steamworks needs and then Types that pattern.
Restarts automatically when the Steamworks game is over IF if you have enough fuel and skips animations if setup correctly (presses X - the default animation cancel keyboard button)
Stops automatically if you are out of fuel or you press the stop button.
Random Pattern clicking if the app is out of date - checks game version against internal version - This provides random results of course, but better than nothing until app is updated.

Introduced a menu to chose what type of run you want.

[*New*] 
Possibility to choose:
	- Configurable value **ShouldConsumeAllFuel** added to AutoSteamApp.exe.config option was added to setup using ALL the fuel or Only the Natural Fuel
	- Default value is **true**
	
[*New*] 
Do a smart RANDOM run:
	- New functionality to do a RANDOM run - this will give random results, as no memory reading is happening. 
	- The other functions of the application will still work (fuel checking, automatic stop and restart etc). 
	- Random Run will be available even if the app is up to date.
	

How to install it
1. Download the .zip 
2. Extract it wherever you like (i.e: Desktop)
3. Installation Done.

How to use it
0. Game must be started, press "start" on your Steamworks minigame
1. Execute AutoSteamApp.exe
2. Switch to the game window 
3. Press "**Home**" for the app to run with 100% accuracy
4. Press "**F1**" for the app to do a Random run
5. Press "End" for the app to stop typing or wait for the fuel tank to be empty

Configurable Parts - AutoSteamApp.exe.config
1. Combo delay - time to wait between 3-key press sequence - useful to sync with animations (larger value might result in better results)
2. ShouldConsumeAllFuel - set this to **false** if you want only the Natural Fuel to be used. Otherwise set to true - default value **true**
3. Enable/Disable logging - keep it disabled unless something looks fishy or malfunctions (or you wanna sniff the logs)
4. Change between AZERTY(QZD) or QUERTY(AWD) - based on your keyboard, pick one - default is QUERTY
5. KeyCodeStart - allows to bind other key for Starting the App in 100% accuracy mode - default is "36" - 'Home' button
6. KeyCodeStartRandom - allows to bind other key for Starting the App in Random mode - default is "112" - 'F1' button
7. KeyCodeStop - allows to bind other key for Stopping the App - default is "35" - 'End' button
8. KeyCutsceneSkip - allows to bind other key for Cutscene/Animation skip - must match the controls set in game at System > Options > Controls > Keyboard Settings > Menu Control > "Use or Register Loadout / Cancel Animation" - default is "88" - 'X' button

Open Source code for you to make it better if you want: [Code might not be stable all the time] https://github.com/AdiBorsos/AutoSteamworks

Please use the Github page to ask for features, it is easier to track.

﻿#Contributors & Credits:
﻿
Geobryn﻿ 
- Made it 100% accurate without writing memory
﻿﻿- Added CutsceneSkip functionality

Info about how it's done:
* [Marcus101RR]( https://fearlessrevolution.com/memberlist.php?mode=viewprofile&u=438 ) for his work on this [Cheat Table](https://fearlessrevolution.com/viewtopic.php?f=4&t=9923)

Thx for spreading love through open source:
* [r00telement](https://github.com/r00telement/SmartHunter) 
* [gabrielefilipp](https://github.com/gabrielefilipp/SmartHunter)

# Pull requests are welcome.
