# Logitech Lua Parser

Project Name pending


## Purpose of project
Add LUA programming options to my Logitech G910 keyboard.  
The Logitech Gaming Software already supports this, but with a few critical shortcomings:
* It does not allow me to set the _color_ of the G-key that's currently active (missclicked something? Good luck finding out which script is running)
* It does not allow me to spam keys (a workaround is to make a macro and call PlayMacro("name")) (games with RSI inducing features, like click E 500 times to plant/loot 500 times)
* It does not receive input events from regular keys (cannot cancel autorun on 'W' for example, sending you off a cliff to die)

The purpose of this tool is to correct some of these shortcomings. (and possibly introduce new issues ;))  

## Getting started
#### LGS Profile  
In order to receive events from G-buttons, your active profile needs to support the .exe file of this project.  
By default, on startup the program will make a new profile and restart LGS. But it can just as easily be used with existing profiles, simply by adding the exe under "properties" on the profile.

#### Lua configuration  
`%appdata%\..\local\evilsoft\logiled\configuration\settings.json` contains a mapping of processes and lua files.  
Ex: `{ "path": "satisfactory.lua", "process": "FactoryGame-Win64-Shipping" }` says that it will run `satisfactory.lua` if the `FactoryGame-Win64-Shipping` process is running. (FactoryGame-Win64-Shipping.exe)
#### .Lua file
The .lua file must have a function called `OnEvent`. See [example.lua](./Logitech/Resources/example.lua) for details on usage.


### Missing/TODO:
* Send focus events to Lua, to detect when the game loses/regains focus
* Example script(s) ([example.lua](./Logitech/Resources/example.lua))
* Ability to run it for "any" application, not just hardcoded processes
* Auto add process .exe to Logitech Gaming Software when detected as missing
* Macro support? (Undecided, is it even possible?)
* Detect key modifiers (Shift, Alt, Ctrl)
* Reset the LUA script on command (currently crashes if called)
* Anonymous usage stats (webstats)
* Probably several bugs still not discovered.

### Unsupported:
* Logitech G Hub will never be supported.  
* The project is for Windows only.  