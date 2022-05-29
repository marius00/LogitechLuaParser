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

active = false
function OnEvent(event, arg, modifiers)
	if event == TickEvent then
		-- Tick event, can be used to repeatedly spam keys
	elseif event == FocusEvent then
		-- The game has either gained or lost focus
		if arg == 'false' and active then
			active = false -- Auto disable G8 when alt-tabbing out of the game.
		end
	elseif event == KeyDownEvent then
		-- A key has been pressed
		if arg == 'G8' then
			active = not active
			if active then
				SetBacklightColor(arg, 0, 100, 0)
			else
				SetBacklightColor(arg, 0, 0, 0)
			end
		elseif arg == 'ESCAPE' then
			-- ResetScript() -- This currently crashes and burns, TODO: Fix it.
			-- This will reload the script. All LUA state will reset.
			-- It will however NOT revert colors back to original state, or release any pressed keys.
			-- If LMB for example is pressed during this, it will only cancel once a regular mouseclick has been made.
			-- For colors, it's recommended to start the script by setting the desired "default colors"
		end
	end
end
