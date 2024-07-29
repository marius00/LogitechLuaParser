require "core"

-- Red: 0%, Green: 100%, Blue: 100%
SetBacklightColor('W', 0, 100, 0)
SetBacklightColor('S', 0, 100, 0)
OutputLogMessage('Starting script with W and S being Green')

-- Click or hold keys
-- KeyDown('Shift')
-- KeyPress('A')
-- KeyUp('Shift')

-- Click the mouse
-- Mouse keys: LMB, RMB, MMB
-- MouseClick('LMB')
-- MouseDoubleClick('LMB')
-- MouseDown('RMB')
-- MouseUp('RMB')

-- Move the mouse cursor
-- MouseMove(15, 0)

-- Sleep/wait
-- OutputLogMessage('{0}', time()) -- Time in milliseconds
-- Sleep(2000)
-- OutputLogMessage('{0}', time())

-- Check if Ctrl, Alt or Shift are pressed during an KeyDownEvent
-- IsShift(modifiers)
-- IsCtrl(modifiers)
-- IsAlt(modifiers)
-- IsM1(modifiers)
-- IsM2(modifiers)
-- IsM3(modifiers)

OutputLogMessage('Text with no prefix')
SetOutputPrefix("[SomeGame] ")
OutputLogMessage('Text with prefix')

active = false
function OnEvent(event, arg, modifiers)
	if event == TickEvent then
		-- Tick event, can be used to repeatedly spam keys
		-- Arg is null
	elseif event == FocusEvent then
		-- The game has either gained or lost focus
		-- Arg will be 'true' or 'false'
		if arg == 'false' and active then
			active = false -- Auto disable G8 when alt-tabbing out of the game.
		end

		-- Restore any colors that were set when we alt-tabbed out (optional)
		if arg == "true" then
			RestoreColors()
		end
	elseif event == KeyDownEvent then
		-- A key has been pressed
		-- Arg will contain the relevant key, modifiers will contain information about Shift, Alt, Ctrl, M1..
		if arg == 'G8' then
			active = not active
			if active then
				SetBacklightColor(arg, 0, 100, 0)
			else
				SetBacklightColor(arg, 0, 0, 0)
			end
		end
	elseif event == KeyUpEvent then
		-- Useful for things like cancelling autorun, a key has just been released.
		-- Arg will contain the relevant key, modifiers will not be included
		-- G-keys will not send KeyUpEvents
	end
end
