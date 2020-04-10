# Auto Steamworks Exec
Supported version: 410013

Functionality
Reads game memory and finds the "expected" key pattern that the Steamworks needs and then Types that pattern.
Restarts automatically when the Steamworks game is over IF if you have enough fuel and skips animations if setup correctly (presses X - the default animation cancel keyboard button)
Stops automatically if you are out of fuel or you press the stop button.
Random Pattern clicking if the app is out of date - checks game version against internal version - This provides random results of course, but better than nothing until app is updated.

How to install it
1. Download the .zip 
2. Extract it wherever you like (i.e: Desktop)
3. Installation Done.

How to use it
0. Game must be started, press "start" on your Steamworks minigame
1. Execute AutoSteamApp.exe
2. Switch to the game window 
3. Press "Home" for the app to start typing
4. Press "End" for the app to stop typing.

Configurable Parts - AutoSteamApp.exe.config
1. Combo delay - time to wait between 3-key press sequence - useful to sync with animations (larger value might result in better results)
2. Key Delay - time to wait between individual presses of keys
3. Enable/Disable logging - keep it disabled unless something looks fishy or malfunctions (or you wanna sniff the logs)
4. Change between AZERTY(QZD) or QUERTY(AWD) - based on your keyboard, pick one - default is QUERTY
5. KeyCodeStart - allows to bind other key for Starting the App
6. KeyCodeStop - allows to bind other key for Stopping the App
7. KeyCutsceneSkip - allows to bind other key for Cutscene/Animation skip - must match the controls set in game at System > Options > Controls > Keyboard Settings > Menu Control > "Use or Register Loadout / Cancel Animation"
8. UseRandomPattern - allows the random pattern functionality - will be used just when versions don't match but it is TURNED OFF by default.

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
