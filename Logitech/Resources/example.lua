SetBacklightColor('W', 0, 100, 0)
SetBacklightColor('S', 0, 100, 0)
OutputLogMessage('Starting script with W and S being Green')

active = false
function OnEvent(event, arg, modifiers)
	if event == TickEvent then
		
	elseif event == FocusEvent then
		
	elseif event == InputEvent then
		if arg == 'G8' then
			active = not active
			if active then
				SetBacklightColor(arg, 0, 100, 0)
			else
				SetBacklightColor(arg, 0, 0, 0)
			end
		end
	end
end
