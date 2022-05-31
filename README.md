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
#### Configuring Logitech Gaming Software

In order to best use the G-keys with this application, they should be configured to be G-keys inside the Logitech Gaming Software:

![](Docs/overlay.png)  

**Hover over the G-key:**  
![](Docs/hover.png)

**Select "Assign new command"**  
![](Docs/assign-new-command.png)

**Assign it to "G-Key" (you may need to scroll down)**   
![](Docs/gkey.png)

**When you start this program for the first time, it will start minimized to tray.**  
The program is running, look for it down in the bottom right corner.


#### Lua configuration  

The .lua file must have a function called `OnEvent`. See [example.lua](./Logitech/Resources/example.lua) for details on usage.


### Example scripts:
* Satisfactory (autorun and spam E) ([satisfactory.lua](./Docs/satisfactory.lua))
* Example script ([example.lua](./Logitech/Resources/example.lua))

### Missing/TODO:
* Macro support? (Undecided, is it even possible?)
* Reset the LUA script on command (currently crashes if called)
* Anonymous usage stats (webstats)
* Probably several bugs still not discovered.
* A fallback profile (if a profile is in use, but does not map this key) + a default profile (if no other profile is in use)

### Unsupported:
* Logitech G Hub - Support may be considered for the future.
* The project is for Windows only.  
