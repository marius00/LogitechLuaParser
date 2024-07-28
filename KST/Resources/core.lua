function OutputLogMessage(...)
    provider:OutputLogMessage(...)
end

function SetBacklightColor(k, r, g, b)
    provider:SetColor(k, r, g, b)
end

function KeyUp(key)
    provider:KeyUp(key)
end

function KeyDown(key)
    provider:KeyDown(key)
end

function KeyPress(key)
    provider:KeyPress(key)
end

function ResetScript()
    provider:ResetState()
end

function MouseDown(key)
    provider:MouseDown(key)
end

function MouseUp(key)
    provider:MouseUp(key)
end

function MouseClick(key)
    provider:MouseClick(key)
end

function MouseDoubleClick(key)
    provider:MouseDoubleClick(key)
end

function MouseMove(x, y)
    provider:MouseMove(x, y)
end

function Sleep(ms)
    provider:Sleep(ms)
end

function time()
    return provider:Time()
end

function IsShift(modifier)
    return (modifier & 2) > 0
end

function IsCtrl(modifier)
    return (modifier & 4) > 0
end

function IsAlt(modifier)
    return (modifier & 8) > 0
end

function IsM1(modifier)
    return (modifier & 16) > 0
end

function IsM2(modifier)
    return (modifier & 32) > 0
end

function IsM3(modifier)
    return (modifier & 64) > 0
end

function SetOutputPrefix(prefix)
    provider:SetOutputPrefix(prefix)
end


TickEvent = 1
KeyDownEvent = 2
KeyUpEvent = 3
FocusEvent = 4