using Godot;
using GodotImGuiExtension;
using ImGuiNET;

[GlobalClass]
public partial class Demo : Node
{
	private GodotObject _inspectingObject = null;
	
	public override void _Ready()
	{
		ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
		CallDeferred(Node.MethodName.AddSibling, new InputImeHelper());
	}

	public override void _Process(double delta)
	{
		ImGui.ShowDemoWindow();

		if (SceneTreeViewer.DrawSceneTree())
		{
			_inspectingObject = GetTree().Root.GetNodeOrNull(SceneTreeViewer.CurrentSelectingNodePath);
		}
		
		ObjectInspector.ShowInspector(ref _inspectingObject);
	}
}
