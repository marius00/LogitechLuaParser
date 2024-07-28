require "core"

SetBacklightColor('W', 0, 100, 0)
SetBacklightColor('S', 0, 100, 0)
SetBacklightColor('G1', 0, 0, 100)
SetBacklightColor('G3', 100, 0, 0)
SetBacklightColor('G4', 100, 0, 0)
SetBacklightColor('G5', 100, 0, 0)
SetOutputPrefix("[STF] ")

-- G5 holds down the left mouse button (manual crafting)
-- G4 spams "e" (loot plants)
-- G3 enables autorun (W, S or G3 to cancel it)
-- G1 cancels all scripts
-- Active keys are green, inactive are red.

spamKey = false
key = 'e' -- Loot plants
autorun = false
holdingLmb = false

function OutputHelpText()
	OutputLogMessage('')
	OutputLogMessage('satisfactory.lua mappings are:')
	OutputLogMessage('')
	OutputLogMessage('- G5: Hold LMB')
	OutputLogMessage('- G4: Spam E')
	OutputLogMessage('- G3: Autorun')
	OutputLogMessage('- G1: Cancel all scripts')
end

function reset()
	spamKey = false
	SetBacklightColor('G4', 100, 0, 0)
	stopAutorun()
	if holdingLmb then
		toggleHoldLmb()
	end
end

function stopAutorun()
	autorun = false
	SetBacklightColor('G3', 100, 0, 0)
	KeyUp('W')
end

function toggleAutorun()
	autorun = not autorun
	if autorun then
		SetBacklightColor('G3', 0, 100, 0)
		KeyDown('W')
	else
		stopAutorun()
	end
end

-- Obs: This can be done easier by simply importing/requiring the "spamkey.lua" script, see spirit-of-the-island.lua for an example
function toggleSpamKey(keyToSpam)
	spamKey = not spamKey
	key = keyToSpam
	if spamKey then
		OutputLogMessage('- Starting to spam key')
		SetBacklightColor('G4', 0, 100, 0)
	else
		OutputLogMessage('- Stopping spam key')
		SetBacklightColor('G4', 100, 0, 0)
	end
end

function toggleHoldLmb()
	holdingLmb = not holdingLmb
	if holdingLmb then
		SetBacklightColor('G5', 0, 100, 0)
		MouseDown('LMB')
	else
		SetBacklightColor('G5', 100, 0, 0)
		MouseUp('LMB')
	end
end

function OnEvent(event, arg, modifiers)
	if event == TickEvent then
		if spamKey then
			KeyPress(key)
		end
	elseif event == KeyDownEvent then
		if arg == 'G5' then
			toggleHoldLmb()
		elseif arg == 'G4' then
			toggleSpamKey('e')
		elseif arg == 'G3' then
			toggleAutorun()
		elseif arg == 'G1' then
			reset()
		elseif arg == 'S' and autorun then
			stopAutorun()
		end
	
	elseif event == FocusEvent then
		if arg == 'true' then
			OutputHelpText()
		end
	elseif event == KeyUpEvent then
		--
	else
		OutputLogMessage('Unknown event type: {0}', event)		
	end
	
	-- Key "UP" event, because W is being held down and we'll constantly receive it.
	if event == KeyUpEvent and autorun and arg == 'W' then
		OutputLogMessage('Cancelling autorun due to W')
		stopAutorun()
	end
end

