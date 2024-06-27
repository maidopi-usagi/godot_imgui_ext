using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotImGuiExtension;
using ImGuiGodot;
using ImGuiNET;

public partial class Demo : Node
{
	private GodotObject? _inspectingObject = null;
	private List<NodePath> _nodePaths = [];
	private float _plotMaxValue = 180.0f;
	private float _plotMaxValueTarget = 180.0f;
	
	[Export(PropertyHint.Range, "0.25, 4.0, 0.1")]
	public float DpiScale
	{
		get => ImGuiGD.Scale;
		set
		{
			Callable.From(() => ImGuiGD.Scale = value).CallDeferred();
		}
	}

	private readonly List<string> _testCases = [
		"res://test_case/01_multi_mesh_instance.tscn",
		"res://test_case/02_json_editor.tscn"
	];

	public override void _Ready()
	{
		ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
		CallDeferred(Node.MethodName.AddSibling, new InputImeHelper());
		ImGuiGD.Scale = 2.0f;
	}
	
	[ExportGroup("My Properties")]
	[Export]
	public int Number { get; set; } = 3;
	
	[ExportCategory("Main Category")]
	[Export]
	public int Number2 { get; set; } = 3;
	[Export]
	public string Text { get; set; } = "";

	[ExportCategory("Extra Category")]
	[Export]
	public bool Flag { get; set; } = false;

	public override void _Process(double delta)
	{
		if (ImGui.BeginMainMenuBar())
		{
			if (ImGui.BeginMenu("Test Cases"))
			{
				foreach (var testCasePath in _testCases)
				{
					if (ImGui.MenuItem(testCasePath.GetFile()))
					{
						GetTree().ChangeSceneToFile(testCasePath);
					}
				}
				ImGui.EndMenu();
			}
			
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
		
		// ImGui.DockSpaceOverViewport();
		
		ImGui.ShowDemoWindow();

		if (SceneTreeViewer.DrawSceneTree(ref _nodePaths))
		{
			_inspectingObject = GetTree().Root.GetNodeOrNull(_nodePaths.Last());
		}
		
		ObjectInspector.ShowInspector(ref _inspectingObject);

		ImGui.Begin("Method Profiler");
		
		foreach (var kv in PerfMeasure.PerfMonitorData)
		{
			var hashCode = kv.Key.GetHashCode();
			if (ImGui.TreeNode($"{kv.Key.ReflectedType?.Name}::{kv.Key.Name}##{hashCode}"))
			{
				ImGui.Text($"Avg:{kv.Value.Average():F}us");
				if (ImGui.TreeNode($"Plot##{hashCode}"))
				{
					var arr = kv.Value.Select(value => (float)value).ToArray();
					ImGui.PlotLines($"##{hashCode}", ref arr[0], arr.Length, 0, "", 0.0f, _plotMaxValue, new (0.0f, 4.0f * ImGui.GetFontSize()));
					if (ImGui.IsItemHovered())
					{
						_plotMaxValueTarget += ImGui.GetIO().MouseWheel * _plotMaxValue * 0.1f;
						if (ImGui.IsMouseDown(ImGuiMouseButton.Middle))
						{
							_plotMaxValueTarget = arr.Max() * 1.1f;
						}
					}
					ImGui.TreePop();
				}
				ImGui.TreePop();
			}
		}
		
		ImGui.End();
		
		_plotMaxValue = float.Lerp(_plotMaxValue, _plotMaxValueTarget, (float)delta * 15.0f);
	}
}
