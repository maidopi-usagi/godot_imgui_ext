using Godot;
using Godot.Collections;
using GodotImGuiExtension;
using MethodTimer;

public partial class JsonEditor : Node2D
{
	[Export(PropertyHint.None, "balabalal")]
	public Dictionary _testDictionary;
	
	[Export]
	public Array _testArray;

	[Export(PropertyHint.Range, "-1000000,1000000,1")]
	public long LongTest;

	[Export]
	public string JsonFilePath
	{
		get => _jsonFilePath;
		set
		{
			_jsonFilePath = value;
			var json = Json.ParseString(FileAccess.GetFileAsString(_jsonFilePath));
			if (json.VariantType != Variant.Type.Nil)
			{
				_testDictionary = json.AsGodotDictionary();
			}
		}
	}

	private Variant _jsonData;
	private string _jsonFilePath = string.Empty;

	public override void _Ready()
	{
		JsonFilePath = "C:/Users/maido/Downloads/data.json";
	}

	[Time]
	public override void _Process(double delta)
	{
		VariantExt.DictionaryEditor(_testDictionary, "balabalal");
	}
}
