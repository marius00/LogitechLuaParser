require "core"

-- Red: 0%, Green: 100%, Blue: 100%
SetBacklightColor('W', 0, 100, 0)
SetBacklightColor('S', 0, 100, 0)
SetBacklightColor('Q', 100, 78, 6)
SetBacklightColor('G1', 0, 0, 0)
SetBacklightColor('G2', 0, 0, 0)

function OutputHelpText()
	OutputLogMessage('')
	OutputLogMessage('cryofall.lua mappings are:')
	OutputLogMessage('')
	OutputLogMessage('- G1: Hold LMB')
	OutputLogMessage('- G2: Keep spamming RMB')

end


holdingLmb = false
spamRmb = false
function toggleHoldLmb(cancel)
	holdingLmb = not holdingLmb
	if cancel and holdingLmb then
		holdingLmb = false
		OutputLogMessage("Cancelling hold LMB")
	end
	
	
	if holdingLmb then
		SetBacklightColor('G1', 100, 0, 0)
		MouseDown('LMB')
	else
		SetBacklightColor('G1', 0, 0, 0)
		UnsetBacklightColor('G1')
		MouseUp('LMB')
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
	toggleHoldLmb(true)
	togglespamRmb(true)
	ResetBacklightColors()
end

active = false
function OnEvent(event, arg, modifiers)
	if event == TickEvent then
		if spamRmb then
			MouseDown('RMB')
			Sleep(25)
			MouseUp('RMB')
			Sleep(25)
			OutputLogMessage("RMB!")
		end
	elseif event == FocusEvent then
		-- The game has either gained or lost focus
		-- Arg will be 'true' or 'false'
		if arg == 'false' then
			cancelAll()
		end
		
		if arg == 'true' then
			OutputHelpText()
		end
		
	elseif event == KeyDownEvent then
		-- A key has been pressed
		-- Arg will contain the relevant key, modifiers will contain information about Shift, Alt, Ctrl, M1..
		if arg == 'G1' then
			toggleHoldLmb(false)
		elseif arg == 'Escape' then
			-- For colors, it's recommended to start the script by setting the desired "default colors"
			cancelAll()
		elseif arg == 'G2' then
			togglespamRmb(false)
		end
	end
end
