require "core"

-- Red: 0%, Green: 100%, Blue: 100%
SetBacklightColor('W', 0, 100, 0)
SetBacklightColor('S', 0, 100, 0)
SetBacklightColor('Q', 100, 78, 6)
SetBacklightColor('G1', 0, 0, 0)
SetBacklightColor('G2', 0, 0, 0)

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

spamLmb = false
spamRmb = false

function OutputHelpText()
	OutputLogMessage('')
	OutputLogMessage('dinkum.lua mappings are:')
	OutputLogMessage('')
	OutputLogMessage('- G1: Keep spamming LMB')
	OutputLogMessage('- G2: Keep spamming RMB')

end


function togglespamLmb(cancel)
	spamLmb = not spamLmb
	if cancel and spamLmb then
		spamLmb = false
		OutputLogMessage("Cancelling spam LMB")
	end
	
	
	if spamLmb then
		SetBacklightColor('G1', 100, 0, 0)
	else
		SetBacklightColor('G1', 0, 0, 0)
		UnsetBacklightColor('G1')
	end
end


function togglespamRmb(cancel)
	spamRmb = not spamRmb
	if cancel and spamRmb then
		spamRmb = false
		OutputLogMessage("Cancelling spam RMB")
	end
	
	
	if spamRmb then
		SetBacklightColor('G2', 100, 0, 0)
	else
		SetBacklightColor('G2', 0, 0, 0)
		UnsetBacklightColor('G2')
	end
end

function cancelAll()
	OutputLogMessage("Cancelling all scripts")
	togglespamLmb(true)
	togglespamRmb(true)
	ResetBacklightColors()
end

active = false
function OnEvent(event, arg, modifiers)
    -- Do we need active check? Don't think we get triggers if not active..
	if event == TickEvent then
		-- Tick event, can be used to repeatedly spam keys
		-- Arg is null
			MouseClick('LMB')
		if spamLmb then
			Sleep(50)
		elseif spamRmb then
			MouseClick('RMB')
			Sleep(50)
		end
	elseif event == FocusEvent then
		-- The game has either gained or lost focus
		-- Arg will be 'true' or 'false'
		if arg == 'false' then
			cancelAll()
		end
		
		if arg == 'true' then
			OutputHelpText()
			RestoreColors()
		end
		
	elseif event == KeyDownEvent then
		-- A key has been pressed
		-- Arg will contain the relevant key, modifiers will contain information about Shift, Alt, Ctrl, M1..
		if arg == 'G1' then
			togglespamLmb(false)
		elseif arg == 'Escape' then
			-- For colors, it's recommended to start the script by setting the desired "default colors"
			cancelAll()
		elseif arg == 'G2' then
			togglespamRmb(false)
		end
	elseif event == KeyUpEvent then
		-- Useful for things like cancelling autorun, a key has just been released.
		-- Arg will contain the relevant key, modifiers will not be included
		-- G-keys will not send KeyUpEvents
	end
end
