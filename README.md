# Logitech Lua Parser

Project Name pending


## Purpose of project
Add LUA programming options to my Logitech G910 keyboard.  
The Logitech Gaming Software already supports this, but with a few critical shortcomigns:
* It does not allow me to set the _color_ of the G-key that's currently active
* It does not allow me to spam keys (a workaround is to make a macro and call PlayMacro("name"))
* It does not receive input events from regular keys (cannot cancel autorun on 'W' for example)

The purpose of this tool is to correct some of these shortcomings. (and possibly introduce new issues ;))  


### Missing/TODO:
* Send focus events to Lua, to detect when the game loses/regains focus
* Example script(s)
* Ability to run it for "any" application, not just hardcoded processes
* Auto add process .exe to Logitech Gaming Software when detected as missing
* Macro support? (Undecided, is it even possible?)
* Detect key modifiers (Shift, Alt, Ctrl)
* Reset the LUA script on command (currently crashes if called)

### Unsupported:
Logitech G Hub will never be supported.