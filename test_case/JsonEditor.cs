using Godot;
using System;
using Godot.Collections;
using GodotImGuiExtension;
using ImGuiNET;
using MethodTimer;
using Microsoft.VisualBasic;

public partial class JsonEditor : Node2D
{
	[Export(PropertyHint.None, "balabalal")]
	public Dictionary _testDictionary;
	
	[Export]
	public Godot.Collections.Array _testArray;

	[Export(PropertyHint.Range, "-10000,10000,1")]
	public long LongTest;
	
	private Variant _jsonData;
	
	public override void _Ready()
	{
		var json = Json.ParseString(FileAccess.GetFileAsString("C:/Users/maido/Downloads/data.json"));
		if (json.VariantType != Variant.Type.Nil)
		{
			_testDictionary = json.AsGodotDictionary();
		}
	}

	[Time]
	public override void _Process(double delta)
	{
		VariantExt.DictionaryEditor(_testDictionary, "balabalal");
	}
}
