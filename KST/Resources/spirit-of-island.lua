require "core"
require "keyspam"

SetBacklightColor('W', 0, 100, 0)
SetBacklightColor('S', 0, 100, 0)
SetBacklightColor('A', 0, 0, 100)
SetBacklightColor('D', 0, 0, 100)

SetBacklightColor('G1', 0, 0, 100)

SetBacklightColor('G2', 100, 0, 0)
SetBacklightColor('G3', 100, 0, 0)
SetBacklightColor('G4', 0, 0, 100)
SetBacklightColor('G5', 100, 0, 0)
SetOutputPrefix("[SOTI] ")

spamLmb = false



ks_add('F5', 'G5', 'E', true)


function OutputHelpText()
	OutputLogMessage('')
	OutputLogMessage('spirit-of-the-island.lua mappings are:')
	OutputLogMessage('')
	OutputLogMessage('- F3/G3: Hold button to spam LMB for fishing')
	OutputLogMessage('- F2/G2: Cancel all scripts')
	ks_outputHelpText()
end

function reset()
	spamLmb = false
	SetBacklightColor('G2', 100, 0, 0)
	SetBacklightColor('F3', 100, 0, 0)
	ks_reset()
end


function OnEvent(event, arg, modifiers)
	if event == TickEvent then
		if spamLmb then
			MouseClick('LMB')
		end
	elseif event == KeyDownEvent then
		if arg == 'F3' then
			if not spamLmb then
				OutputLogMessage("Starting to spam LMB")
			end
			spamLmb = true
			
		elseif arg == 'F2' then
			reset()
		end
	
	elseif event == FocusEvent then
		if arg == 'true' then
			OutputHelpText()
			RestoreColors()
		end
	elseif event == KeyUpEvent then
		if arg == 'F3' then
			spamLmb = false
			OutputLogMessage("Stopping LMB spam")
		end
	else
		OutputLogMessage('Unknown event type: {0}', event)		
	end
	
	ks_OnEvent(event, arg, modifiers)
end

