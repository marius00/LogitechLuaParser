-- Red: 0%, Green: 100%, Blue: 100%
SetBacklightColor('W', 0, 100, 0)
SetBacklightColor('S', 0, 100, 0)
OutputLogMessage('Starting script with W and S being Green')

-- KeyDown('Shift')
-- KeyPress('A')
-- KeyUp('Shift')

-- Mouse keys: LMB, RMB, MMB
-- MouseClick('LMB')
-- MouseDoubleClick('LMB')
-- MouseDown('RMB')
-- MouseUp('RMB')

-- MouseMove(15, 0)

-- OutputLogMessage('{0}', time()) -- Time in milliseconds
-- Sleep(2000)
-- OutputLogMessage('{0}', time())

active = false
function OnEvent(event, arg, modifiers)
	if event == TickEvent then
		-- Tick event, can be used to repeatedly spam keys
	elseif event == FocusEvent then
		-- The game has either gained or lost focus
	elseif event == InputEvent then
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
