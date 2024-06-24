using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotImGuiExtension;
using ImGuiGodot;
using ImGuiNET;

[GlobalClass]
public partial class Demo : Node
{
	private GodotObject _inspectingObject = null;
	private List<NodePath> _nodePaths = [];
	
	[Export(PropertyHint.Range, "0.25, 4.0, 0.1")]
	public float DpiScale
	{
		get => ImGuiGD.Scale;
		set
		{
			Callable.From(() => ImGuiGD.Scale = value).CallDeferred();
		}
	}

	public override void _Ready()
	{
		ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
		CallDeferred(Node.MethodName.AddSibling, new InputImeHelper());
		ImGuiGD.Scale = 2.0f;
	}

	public override void _Process(double delta)
	{
		if (ImGui.BeginMainMenuBar())
		{
			if (ImGui.BeginMenu("Panels"))
			{
				ImGui.EndMenu();
			}
			
			if (ImGui.BeginMenu("About"))
			{
				ImGui.EndMenu();
			}
			
			ImGui.EndMainMenuBar();
		}
		
		ImGui.DockSpaceOverViewport();
		
		ImGui.ShowDemoWindow();

		if (SceneTreeViewer.DrawSceneTree(ref _nodePaths))
		{
			_inspectingObject = GetTree().Root.GetNodeOrNull(_nodePaths.Last());
		}
		
		ObjectInspector.ShowInspector(ref _inspectingObject);
	}
}
