-- Usage:
-- Call ks_add('G5', 'G5', 'E') to spam E on G5
-- Or if using G-hub with G5 mapped to F5, then ks_add('F5', 'G5', 'E')
-- In OnEvent call ks_OnEvent
-- (Optional) In output text call ks_outputHelpText()
-- (Optional) In reset call ks_reset()

ks_setup = {}
ks_activeKeys = {}
ks_colormapOverride = {}

-- Resets/stops all key spam. 
function ks_reset()
	for k,v in pairs(ks_setup) do
		SetBacklightColor(ks_colormapOverride[k], 100, 0, 0)
	end

	-- No active keys
	ks_activeKeys = {}
end

-- Call this to setup the keymapping, ex: addKeySpamMapping('G4', 'E') will make G4 toggle "spam E"
function ks_add(keyToTrigger, keyToColor, keyToSpam)
	-- ks_setup["G4"] = "E"
	-- Then when activated, ks_activeKeys["E"] = true
	
	-- With some logitech setups (specifically new g-hub), the G1 key will be received as F1
	-- The ks_colormapOverride lets us color G1 while listening for F1
	ks_colormapOverride[keyToTrigger] = keyToColor

	
	ks_setup[keyToTrigger] = keyToSpam	
	SetBacklightColor(keyToColor, 100, 0, 0)
	OutputLogMessage('Configured key "{0}" to spam key "{1}', keyToTrigger, keyToSpam)
end

function ks_outputHelpText()
	for k,v in pairs(ks_setup) do
		OutputLogMessage('- {0}: Spam the "{1}" key', ks_colormapOverride[k], v)
	end
end

function ks_OnEvent(event, arg, modifiers)
	if event == TickEvent then
		-- Spam the active keys
		for k,v in pairs(ks_activeKeys) do
			KeyPress(k)
		end
	elseif event == KeyDownEvent then
		-- Toggle key active/inactive
		if ks_setup[arg] ~= nil then
			-- We have the key in the setup
			if ks_activeKeys[ks_setup[arg]] ~= nil then
				ks_activeKeys[ks_setup[arg]] = nil
				OutputLogMessage('Stopped spamming key "{0}"', ks_setup[arg])
				SetBacklightColor(ks_colormapOverride[arg], 100, 0, 0)
			else
				ks_activeKeys[ks_setup[arg]] = true
				OutputLogMessage('Starting to spam key "{0}"', ks_setup[arg])
				SetBacklightColor(ks_colormapOverride[arg], 0, 100, 0)
			end
		end
	end
end