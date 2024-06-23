using Godot;
using ImGuiNET;

namespace GodotImGuiExtension;

public sealed partial class InputImeHelper : Node
{
    public InputImeHelper()
    {
        Name = "InputImeHelper";
    }

    public override void _Ready()
    {
        base._Ready();
        ((Window)GetViewport()).SetImeActive(true);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventAction action && !action.Action.ToString().Contains("ui"))
        {
            GetViewport().SetInputAsHandled();
            @event.Dispose();
            return;
        }
        if (@event is not InputEventKey key)
        {
            @event.Dispose();
            return;
        }
        var keyUnicode = (char)key.Unicode;
        if (keyUnicode < 0x4e00 || keyUnicode > 0x9fa5)
        {
            @event.Dispose();
            return;
        }
        ImGui.GetIO().AddInputCharacter((uint)key.Unicode);
        GetViewport().SetInputAsHandled();
        @event.Dispose();
    }
}