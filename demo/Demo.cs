using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotImGuiExtension;
using ImGuiNET;

[GlobalClass]
public partial class Demo : Node
{
	private GodotObject _inspectingObject = null;
	private List<NodePath> _nodePaths = [];
	
	public override void _Ready()
	{
		ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
		CallDeferred(Node.MethodName.AddSibling, new InputImeHelper());
	}

	public override void _Process(double delta)
	{
		ImGui.ShowDemoWindow();

		if (SceneTreeViewer.DrawSceneTree(ref _nodePaths))
		{
			_inspectingObject = GetTree().Root.GetNodeOrNull(_nodePaths.Last());
		}
		
		ObjectInspector.ShowInspector(ref _inspectingObject);
	}
}
