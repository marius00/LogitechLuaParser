SetBacklightColor('W', 0, 100, 0)
SetBacklightColor('S', 0, 100, 0)
SetBacklightColor('G3', 0, 0, 0)
SetBacklightColor('G4', 0, 0, 0)

-- Simple script to spam "e" on G4 (loot plants in Satisfactory)
-- And autorun on G3. Autorun will cancel by clicking "S" or G3 again.
-- G1 cancels both scripts

spamKey = false
key = 'e' -- Loot plants
autorun = false

function reset()
	spamKey = false
	SetBacklightColor('G4', 0, 0, 0)
	stopAutorun()
end

function stopAutorun()
	autorun = false
	SetBacklightColor('G3', 0, 0, 0)
	KeyUp('W')
end

function toggleAutorun()
	autorun = not autorun
	if autorun then
		SetBacklightColor('G3', 100, 0, 0)
		KeyDown('W')
	else
		stopAutorun()
	end
end

function toggleSpamKey(keyToSpam)
	spamKey = not spamKey
	key = keyToSpam
	if spamKey then
		SetBacklightColor('G3', 0, 100, 0)
	else
		SetBacklightColor('G3', 0, 0, 0)
	end
end

function OnEvent(event, arg, modifiers)
	if event == TickEvent then
		if spamKey then
			KeyPress(key)
		end
	elseif event == KeyDownEvent then
		if arg == 'G4' then
			toggleSpamKey('e')
		elseif arg == 'G3' then
			toggleAutorun()
		elseif arg == 'G1' then
			reset()
		elseif arg == 'S' and autorun then
			stopAutorun()
		end
	else
		OutputLogMessage('Unknown event type: {0}', event)		
	end
	
	if event == KeyUpEvent and autorun and arg == 'W' then
		OutputLogMessage('Cancelling autorun due to W')
		stopAutorun()
	end
end

