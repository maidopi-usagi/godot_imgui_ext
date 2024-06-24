using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using ImGuiNET;

namespace GodotImGuiExtension;

internal static class ObjectInspector
{
    private struct PropertyInfo(Dictionary dictionary)
    {
        public readonly StringName Name = dictionary["name"].AsStringName();
        public readonly PropertyHint Hint = (PropertyHint)dictionary["hint"].As<int>();
        public readonly string HintString = dictionary["hint_string"].As<string>();
        public readonly PropertyUsageFlags Usage = (PropertyUsageFlags)dictionary["usage"].AsInt64();
        public readonly Variant.Type Type = (Variant.Type)dictionary["type"].As<int>();
    }

    private static readonly System.Collections.Generic.Dictionary<StringName, List<PropertyInfo>> _cachedProperties = [];

    public static void ShowInspector(ref GodotObject godotObject)
    {
        ImGui.Begin("Node Inspector");
        
        var counter = 0;
        if (godotObject is null || !GodotObject.IsInstanceValid(godotObject))
        {
            ImGui.End();
            return;
        }
        var baseClassName = godotObject.GetClass();
        var script = godotObject.GetScript().As<Script>();
        while (baseClassName != "" && counter < 10)
        {
            var className = baseClassName;
            var isScript = false;
            if (script is not null)
            {
                var list = script.GetScriptPropertyList();
                if (list.Count > 0)
                {
                    className = list[0]["name"].AsStringName();
                    isScript = true;
                }
            }

            if (ImGui.CollapsingHeader(className))
            {
                var hasGroup = false;
                var displayNextWidget = true;
                if (!_cachedProperties.TryGetValue(className, out var prop))
                {
                    var classGetPropertyList = isScript
                        ? script.GetScriptPropertyList()
                        : ClassDB.ClassGetPropertyList(className, true);
                    prop = classGetPropertyList.Select(dict => new PropertyInfo(dict)).ToList();
                    _cachedProperties[className] = prop;
                }

                if (ImGui.BeginTable($"##split{className}", 2, ImGuiTableFlags.Resizable))
                {
                    ImGui.TableSetupScrollFreeze(0,1);
                    ImGui.TableSetupColumn("Property");
                    ImGui.TableSetupColumn("Value");
                    ImGui.TableHeadersRow();
                    
                    for (var index = 0; index < prop.Count; index++)
                    {
                        var info = prop[index];
                        var value = godotObject.Get(info.Name);

                        if (info.Type is Variant.Type.Nil)
                        {
                            continue;
                        }

                        if (info.Usage is PropertyUsageFlags.Group or PropertyUsageFlags.Category)
                        {
                            if (hasGroup && displayNextWidget) ImGui.TreePop();
                            else hasGroup = true;
                            ImGui.Separator();
                            displayNextWidget = ImGui.TreeNode($"{info.Name}");
                        }
                        else if (displayNextWidget)
                        {
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.Text($"{info.Name}");

                            if (ImGui.IsItemHovered())
                            {
                                ImGui.SetTooltip($"{info.Name}\n{info.Hint}\n{info.HintString}\n{info.Usage}\n{info.Type}");
                            }
                            
                            ImGui.TableSetColumnIndex(1);
                            if (info.Type == Variant.Type.Object)
                            {
                                var obj = value.AsGodotObject();
                                if (obj is not null && GodotObject.IsInstanceValid(obj))
                                {
                                    if (ImGui.Button($"{obj}"))
                                    {
                                        godotObject = obj;
                                    }
                                }
                                else
                                {
                                    ImGui.Text("NULL");
                                }
                            }
                            else
                            {
                                if (VariantExt.ImEditVariant($"##{info.Name}Inspector Edit",
                                        ref value, info.Hint, info.HintString))
                                {
                                    godotObject.Set(info.Name, value);
                                }
                            }
                        }
                    }
                    
                    ImGui.EndTable();
                }
                
                if (hasGroup && displayNextWidget) ImGui.TreePop();
            }

            if (isScript)
            {
                script = script.GetBaseScript();
            }
            else
            {
                baseClassName = ClassDB.GetParentClass(className);
            }

            counter++;
        }
        
        ImGui.End();
    }
}