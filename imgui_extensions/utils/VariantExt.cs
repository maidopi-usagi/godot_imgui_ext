using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ImGuiNET;
using static ImGuiNET.ImGuiTableFlags;

namespace GodotImGuiExtension;

internal static class VariantExt
{
    private const ImGuiTableFlags TableFlags = ScrollX | ScrollY | RowBg | BordersOuter | BordersV
                                               | Hideable | Reorderable | Resizable | SizingFixedFit;
    
    
    public static bool ImEditValue<[MustBeVariant] T>(string label, ref T value, PropertyHint hint = PropertyHint.None,
        string hintString = "")
    {
        var valueChanged = false;
        if (value is not Variant variant)
        {
            variant = Variant.From(value);
        }

        if (ImEditVariant(label, ref variant, hint, hintString))
        {
            value = variant.As<T>();
            valueChanged = true;
        }

        return valueChanged;
    }

    public static unsafe bool ImEditVariant(string label, ref Variant variant, PropertyHint hint = PropertyHint.None,
        string hintString = "")
    {
        switch (variant.VariantType)
        {
            case Variant.Type.Bool:
            {
                var boolValue = variant.AsBool();
                if (ImGui.Checkbox(label, ref boolValue))
                {
                    variant = boolValue;
                    return true;
                }

                break;
            }
            case Variant.Type.Float:
            {
                var doubleValue = variant.AsDouble();
                if (hint == PropertyHint.Range)
                {
                    if (hintString.Contains("radians_as_degrees"))
                    {
                        var floatValue = (float)doubleValue;
                        if (ImGui.SliderAngle(label, ref floatValue))
                        {
                            variant = floatValue;
                            return true;
                        }
                    }
                    else
                    {
                        var paramsFloats = hintString.Split(",");
                        var ptr = new IntPtr(&doubleValue);
                        double doubleMin = paramsFloats[0].ToFloat();
                        double doubleMax = paramsFloats[1].ToFloat();
                        var ptrMin = new IntPtr(&doubleMin);
                        var ptrMax = new IntPtr(&doubleMax);
                        if (ImGui.SliderScalar(label, ImGuiDataType.Double, ptr, ptrMin, ptrMax))
                        {
                            variant = doubleValue;
                            return true;
                        }
                    }
                }
                else
                {
                    var ptr = new IntPtr(&doubleValue);
                    double doubleMin = double.MinValue;
                    double doubleMax = double.MaxValue;
                    var ptrMin = new IntPtr(&doubleMin);
                    var ptrMax = new IntPtr(&doubleMax);
                    if (ImGui.DragScalar(label, ImGuiDataType.Double, ptr, 0.01f, ptrMin, ptrMax))
                    {
                        variant = doubleValue;
                        return true;
                    }
                }

                break;
            }
            case Variant.Type.Int:
            {
                var longValue = variant.AsInt64();
                if (hint == PropertyHint.Enum)
                {
                    var enums = hintString.Split(",");
                    var intValue = (int)longValue;
                    if (ImGui.Combo(label, ref intValue, enums, enums.Length))
                    {
                        variant = intValue;
                        return true;
                    }
                }
                else if (hint is PropertyHint.Layers3DPhysics or PropertyHint.Layers2DPhysics)
                {
                    uint uintValue = (uint)longValue;
                    var valueChanged = false;
                    ImGui.BeginChild(label, new(0.0f, ImGui.GetTextLineHeight() * 8));
                    ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X / 8.0f);
                    for (int i = 0; i < 32; i++)
                    {
                        var u = 1U << i;
                        var flag = (uintValue & u) != 0;
                        ImGui.PushID(i);
                        if (ImGui.Checkbox(label, ref flag))
                        {
                            uintValue = flag ? uintValue + u : uintValue - u;
                            variant = uintValue;
                            valueChanged = true;
                        }
                        ImGui.PopID();
                        if (i % 8 != 7)
                        {
                            ImGui.SameLine();
                        }
                    }

                    ImGui.PopItemWidth();
                    ImGui.EndChild();
                    if (valueChanged)
                    {
                        return true;
                    }
                }
                else if (hint is PropertyHint.Layers3DRender or PropertyHint.Layers2DRender)
                {
                    uint uintValue = (uint)longValue;
                    var valueChanged = false;
                    ImGui.BeginChild(label, new(0.0f, ImGui.GetTextLineHeight() * 4));
                    ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X / 8.0f);
                    for (int i = 0; i < 20; i++)
                    {
                        var u = 1U << i;
                        var flag = (uintValue & u) != 0;
                        ImGui.PushID(i);
                        if (ImGui.Checkbox(label, ref flag))
                        {
                            uintValue = flag ? uintValue + u : uintValue - u;
                            variant = uintValue;
                            valueChanged = true;
                        }
                        ImGui.PopID();
                        if (i % 10 != 9)
                        {
                            ImGui.SameLine();
                        }
                    }

                    ImGui.PopItemWidth();
                    ImGui.EndChild();
                    if (valueChanged)
                    {
                        return true;
                    }
                }
                else if (hint == PropertyHint.Range)
                {
                    var paramsInts = hintString.Split(",");
                    var ptr = new IntPtr(&longValue);
                    long longMin = paramsInts[0].ToInt();
                    long longMax = paramsInts[1].ToInt();
                    var ptrMin = new IntPtr(&longMin);
                    var ptrMax = new IntPtr(&longMax);
                    if (ImGui.SliderScalar(label, ImGuiDataType.S64, ptr, ptrMin, ptrMax))
                    {
                        variant = longValue;
                        return true;
                    }
                }
                else
                {
                    var ptr = new IntPtr(&longValue);
                    var doubleMin = long.MinValue;
                    var doubleMax = long.MaxValue;
                    var ptrMin = new IntPtr(&doubleMin);
                    var ptrMax = new IntPtr(&doubleMax);
                    if (ImGui.DragScalar(label, ImGuiDataType.S64, ptr, 0.2f, ptrMin, ptrMax))
                    {
                        variant = longValue;
                        return true;
                    }
                }

                break;
            }
            case Variant.Type.Vector2:
            {
                var vec2Value = variant.AsVector2().ToSystemVec();
                if (ImGui.DragFloat2(label, ref vec2Value, 0.01f))
                {
                    variant = vec2Value.ToGodotVec();
                    return true;
                }

                break;
            }
            case Variant.Type.Vector3:
            {
                var vec3Value = variant.AsVector3().ToSystemVec();
                if (ImGui.DragFloat3(label, ref vec3Value, 0.01f))
                {
                    variant = vec3Value.ToGodotVec();
                    return true;
                }

                break;
            }
            case Variant.Type.Vector4:
            {
                var vec4Value = variant.AsVector4().ToSystemVec();
                if (ImGui.DragFloat4(label, ref vec4Value, 0.01f))
                {
                    variant = vec4Value.ToGodotVec();
                    return true;
                }

                break;
            }
            case Variant.Type.Color:
            {
                var colorValue = variant.AsColor().ToSystemVec();
                if (ImGui.ColorEdit4(label, ref colorValue))
                {
                    variant = colorValue.ToGodotColor();
                    return true;
                }

                break;
            }
            case Variant.Type.StringName:
            case Variant.Type.NodePath:
            case Variant.Type.String:
            {
                var strValue = variant.AsString();
                if (ImGui.InputText(label, ref strValue, 256, ImGuiInputTextFlags.EnterReturnsTrue))
                {
                    variant = strValue;
                    return true;
                }

                break;
            }
            case Variant.Type.Vector2I:
            {
                var vec2IValue = variant.AsVector2I().ToSystemVec();
                if (ImGui.DragInt2(label, ref vec2IValue[0]))
                {
                    variant = vec2IValue.ToGodotVec2();
                    return true;
                }

                break;
            }
            case Variant.Type.Vector3I:
            {
                var vec3IValue = variant.AsVector3I().ToSystemVec();
                if (ImGui.DragInt3(label, ref vec3IValue[0]))
                {
                    variant = vec3IValue.ToGodotVec3();
                    return true;
                }

                break;
            }
            case Variant.Type.Vector4I:
            {
                var vec4IValue = variant.AsVector4I().ToSystemVec();
                if (ImGui.DragInt4(label, ref vec4IValue[0]))
                {
                    variant = vec4IValue.ToGodotVec4();
                    return true;
                }

                break;
            }
            case Variant.Type.Dictionary:
            {
                var dictChanged = false;
                var dictValue = variant.AsGodotDictionary();
                var strId = $"{variant.VariantType}##{label}";
                if (ImGui.TreeNode(strId))
                {
                    foreach (var kv in dictValue.WithIndex())
                    {
                        var valueVariant = kv.item.Value;
                        ImGui.PushID(kv.index);
                        if (ImEditVariant(label, ref valueVariant, hint, hintString))
                        {
                            dictValue[kv.item.Key] = valueVariant;
                            dictChanged = true;
                        }
                        ImGui.PopID();
                    }
                    ImGui.TreePop();
                }

                if (dictChanged)
                {
                    return true;
                }
                break;
            }
            case Variant.Type.Array:
            {
                var arrValue = variant.AsGodotArray();
                if (ImEditGodotArray(label, ref arrValue))
                {
                    return true;
                }
                break;
            }
            case Variant.Type.PackedByteArray:
            {
                var arrValue = variant.AsByteArray();
                if (ImEditArrayClipped(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedInt32Array:
            {
                var arrValue = variant.AsInt32Array();
                if (ImEditArrayClipped(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedInt64Array:
            {
                var arrValue = variant.AsInt64Array();
                if (ImEditArrayClipped(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedFloat32Array:
            {
                var arrValue = variant.AsFloat32Array();
                if (ImEditArrayClipped(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedFloat64Array:
            {
                var arrValue = variant.AsFloat64Array();
                if (ImEditArrayClipped(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedStringArray:
            {
                var arrValue = variant.AsStringArray();
                if (ImEditArrayClipped(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedVector2Array:
            {
                var arrValue = variant.AsVector2Array();
                if (ImEditArrayClipped(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedVector3Array:
            {
                var arrValue = variant.AsVector3Array();
                if (ImEditArrayClipped(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedVector4Array:
            {
                var arrValue = variant.AsVector4Array();
                if (ImEditArrayClipped(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedColorArray:
            {
                var arrValue = variant.AsColorArray();
                if (ImEditArrayClipped(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.Rect2:
            {
                var strId = $"{variant.VariantType}##{label}";
                if (!ImGui.TreeNode(strId))
                {
                    break;
                }

                var rectValue = variant.AsRect2();
                var end = rectValue.End;
                var size = rectValue.Size;
                var pos = rectValue.Position;
                var valueChanged = false;
                if (ImEditValue("Pos", ref pos))
                {
                    rectValue.Position = pos;
                    valueChanged = true;
                }

                if (ImEditValue("End", ref end))
                {
                    rectValue.End = end;
                    valueChanged = true;
                }

                if (ImEditValue("Size", ref size))
                {
                    rectValue.Size = size;
                    valueChanged = true;
                }

                ImGui.TreePop();
                if (valueChanged)
                {
                    variant = rectValue;
                    return true;
                }

                break;
            }
            case Variant.Type.Rect2I:
            {
                var strId = $"{variant.VariantType}##{label}";
                if (!ImGui.TreeNode(strId))
                {
                    break;
                }

                var rectValue = variant.AsRect2I();
                var end = rectValue.End;
                var size = rectValue.Size;
                var pos = rectValue.Position;
                var valueChanged = false;
                if (ImEditValue("Pos", ref pos))
                {
                    rectValue.Position = pos;
                    valueChanged = true;
                }

                if (ImEditValue("End", ref end))
                {
                    rectValue.End = end;
                    valueChanged = true;
                }

                if (ImEditValue("Size", ref size))
                {
                    rectValue.Size = size;
                    valueChanged = true;
                }

                ImGui.TreePop();
                if (valueChanged)
                {
                    variant = rectValue;
                    return true;
                }

                break;
            }
            case Variant.Type.Transform2D:
            {
                var strId = $"{variant.VariantType}##{label}";
                if (!ImGui.TreeNode(strId))
                {
                    break;
                }

                var xform2DValue = variant.AsTransform2D();
                var origin = xform2DValue.Origin;
                var x = xform2DValue.X;
                var y = xform2DValue.Y;
                var valueChanged = false;
                if (ImEditValue("Origin", ref origin))
                {
                    xform2DValue.Origin = origin;
                    valueChanged = true;
                }

                if (ImEditValue("X", ref x))
                {
                    xform2DValue.X = x;
                    valueChanged = true;
                }

                if (ImEditValue("Y", ref y))
                {
                    xform2DValue.Y = y;
                    valueChanged = true;
                }

                ImGui.TreePop();
                if (valueChanged)
                {
                    variant = xform2DValue;
                    return true;
                }

                break;
            }
            case Variant.Type.Plane:
            {
                var strId = $"{variant.VariantType}##{label}";
                if (!ImGui.TreeNode(strId))
                {
                    break;
                }

                var planeValue = variant.AsPlane();
                var d = planeValue.D;
                var normal = planeValue.Normal;
                var x = planeValue.X;
                var y = planeValue.Y;
                var z = planeValue.Z;
                var valueChanged = false;
                if (ImEditValue("Distance", ref d))
                {
                    planeValue.D = d;
                    valueChanged = true;
                }

                if (ImEditValue("End", ref normal))
                {
                    planeValue.Normal = normal;
                    valueChanged = true;
                }

                if (ImEditValue("X", ref x))
                {
                    planeValue.X = x;
                    valueChanged = true;
                }

                if (ImEditValue("Y", ref y))
                {
                    planeValue.Y = y;
                    valueChanged = true;
                }

                if (ImEditValue("Z", ref z))
                {
                    planeValue.Z = z;
                    valueChanged = true;
                }

                ImGui.TreePop();
                if (valueChanged)
                {
                    variant = planeValue;
                    return true;
                }

                break;
            }
            case Variant.Type.Quaternion:
            {
                var strId = $"{variant.VariantType}##{label}";
                if (!ImGui.TreeNode(strId))
                {
                    break;
                }

                var quatValue = variant.AsQuaternion();
                var w = quatValue.W;
                var x = quatValue.X;
                var y = quatValue.Y;
                var z = quatValue.Z;
                var valueChanged = false;
                if (ImEditValue("W", ref w))
                {
                    quatValue.W = w;
                    valueChanged = true;
                }

                if (ImEditValue("X", ref x))
                {
                    quatValue.X = x;
                    valueChanged = true;
                }

                if (ImEditValue("Y", ref y))
                {
                    quatValue.Y = y;
                    valueChanged = true;
                }

                if (ImEditValue("Z", ref z))
                {
                    quatValue.Z = z;
                    valueChanged = true;
                }

                ImGui.TreePop();
                if (valueChanged)
                {
                    variant = quatValue;
                    return true;
                }

                break;
            }
            case Variant.Type.Aabb:
            {
                var strId = $"{variant.VariantType}##{label}";
                if (!ImGui.TreeNode(strId))
                {
                    break;
                }

                var aabbValue = variant.AsAabb();
                var end = aabbValue.End;
                var size = aabbValue.Size;
                var pos = aabbValue.Position;
                var valueChanged = false;
                if (ImEditValue("Pos", ref pos))
                {
                    aabbValue.Position = pos;
                    valueChanged = true;
                }

                if (ImEditValue("End", ref end))
                {
                    aabbValue.End = end;
                    valueChanged = true;
                }

                if (ImEditValue("Size", ref size))
                {
                    aabbValue.Size = size;
                    valueChanged = true;
                }

                ImGui.TreePop();
                if (valueChanged)
                {
                    variant = aabbValue;
                    return true;
                }

                break;
            }
            case Variant.Type.Basis:
            {
                var strId = $"{variant.VariantType}##{label}";
                if (!ImGui.TreeNode(strId))
                {
                    break;
                }

                var basisValue = variant.AsBasis();
                var x = basisValue.X;
                var y = basisValue.Y;
                var z = basisValue.Z;
                var valueChanged = false;
                if (ImEditValue("X", ref x))
                {
                    basisValue.X = x;
                    valueChanged = true;
                }

                if (ImEditValue("Y", ref y))
                {
                    basisValue.Y = y;
                    valueChanged = true;
                }

                if (ImEditValue("Z", ref z))
                {
                    basisValue.Z = z;
                    valueChanged = true;
                }

                ImGui.TreePop();
                if (valueChanged)
                {
                    variant = basisValue;
                    return true;
                }

                break;
            }
            case Variant.Type.Transform3D:
            {
                var strId = $"{variant.VariantType}##{label}";
                if (!ImGui.TreeNode(strId))
                {
                    break;
                }

                var xform2DValue = variant.AsTransform3D();
                var origin = xform2DValue.Origin;
                var basis = xform2DValue.Basis;
                var valueChanged = false;
                if (ImEditValue("Origin", ref origin))
                {
                    xform2DValue.Origin = origin;
                    valueChanged = true;
                }

                if (ImEditValue("Basis", ref basis))
                {
                    xform2DValue.Basis = basis;
                    valueChanged = true;
                }

                ImGui.TreePop();
                if (valueChanged)
                {
                    variant = xform2DValue;
                    return true;
                }

                break;
            }
            case Variant.Type.Projection:
            {
                var strId = $"{variant.VariantType}##{label}";
                if (!ImGui.TreeNode(strId))
                {
                    break;
                }

                var projectionValue = variant.AsProjection();
                var w = projectionValue.W;
                var x = projectionValue.X;
                var y = projectionValue.Y;
                var z = projectionValue.Z;
                var valueChanged = false;
                if (ImEditValue("W", ref w))
                {
                    projectionValue.W = w;
                    valueChanged = true;
                }

                if (ImEditValue("X", ref x))
                {
                    projectionValue.X = x;
                    valueChanged = true;
                }

                if (ImEditValue("Y", ref y))
                {
                    projectionValue.Y = y;
                    valueChanged = true;
                }

                if (ImEditValue("Z", ref z))
                {
                    projectionValue.Z = z;
                    valueChanged = true;
                }

                ImGui.TreePop();
                if (valueChanged)
                {
                    variant = projectionValue;
                    return true;
                }

                break;
            }
            case Variant.Type.Rid:
            case Variant.Type.Callable:
            case Variant.Type.Signal:
            case Variant.Type.Object:
            {
                ImGui.Text($"{label}\n{variant}");
                break;
            }
            case Variant.Type.Max:
            case Variant.Type.Nil:
            {
                break;
            }
        }
        return false;
    }

    private static unsafe bool ImEditArrayClipped<[MustBeVariant] T>(string label, ref T[] arrValue)
    {
        var valueChanged = false;
        var rowCount = arrValue.Length;
        if (ImGui.TreeNode($"[{rowCount}]{label}"))
        {
            if (ImGui.BeginTable(label, 2, TableFlags, new System.Numerics.Vector2(0.0f, 240.0f)))
            {
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableSetupColumn("Index", ImGuiTableColumnFlags.WidthFixed, 64);
                ImGui.TableSetupColumn("Value", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow(ImGuiTableRowFlags.Headers);
                ImGui.TableSetColumnIndex(0);
                ImGui.TableHeader("IDX");
                ImGui.TableSetColumnIndex(1);
                ImGui.TableHeader("VALUE");
                var clipperData = new ImGuiListClipper();
                var listClipperPtr = new ImGuiListClipperPtr(&clipperData);
                listClipperPtr.Begin(rowCount);
                while (listClipperPtr.Step())
                {
                    for (var row = listClipperPtr.DisplayStart; row < listClipperPtr.DisplayEnd; row++)
                    {
                        if (row >= rowCount) continue;
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.Text(row.ToString());
                        ImGui.TableSetColumnIndex(1);
                        var value = arrValue[row];
                        if (value is not Variant variant)
                        {
                            variant = Variant.From(value);
                        }
                        ImGui.PushID(row);
                        if (ImEditVariant("row", ref variant))
                        {
                            arrValue[row] = variant.As<T>();
                            valueChanged = true;
                        }
                        ImGui.PopID();
                    }
                }

                ImGui.EndTable();
            }

            ImGui.TreePop();
        }

        return valueChanged;
    }

    private static bool ImEditGodotArray(string label, ref Godot.Collections.Array arrValue)
    {
        var valueChanged = false;
        var rowCount = arrValue.Count;
        if (ImGui.TreeNode($"[{rowCount}]{label}"))
        {
            if (ImGui.BeginTable(label, 2, TableFlags, new System.Numerics.Vector2(0.0f, 240.0f)))
            {
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableSetupColumn("Index", ImGuiTableColumnFlags.WidthFixed, 64);
                ImGui.TableSetupColumn("Value", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow(ImGuiTableRowFlags.Headers);
                ImGui.TableSetColumnIndex(0);
                ImGui.TableHeader("IDX");
                ImGui.TableSetColumnIndex(1);
                ImGui.TableHeader("VALUE");

                for (var row = 0; row < rowCount; row++)
                {
                    if (row >= rowCount) continue;
                    ImGui.TableNextRow();
                    if (ImGui.TableSetColumnIndex(0))
                    {
                        ImGui.Text($"{row}");
                    }
                    if (ImGui.TableSetColumnIndex(1))
                    {
                        var value = arrValue[row];
                        ImGui.PushID(row);
                        if (ImEditVariant("##", ref value))
                        {
                            arrValue[row] = value;
                            valueChanged = true;
                        }
                        ImGui.PopID();
                    }
                }

                ImGui.EndTable();
            }

            ImGui.TreePop();
        }

        return valueChanged;
    }

    public static System.Numerics.Vector2 ToSystemVec(this Vector2 vec)
    {
        return new(vec.X, vec.Y);
    }

    public static Vector2 ToGodotVec(this System.Numerics.Vector2 vec)
    {
        return new(vec.X, vec.Y);
    }

    public static int[] ToSystemVec(this Vector2I vec)
    {
        return [vec.X, vec.Y];
    }

    public static Vector2I ToGodotVec2(this int[] vec)
    {
        return new Vector2I(vec[0], vec[1]);
    }

    public static int[] ToSystemVec(this Vector3I vec)
    {
        return [vec.X, vec.Y, vec.Z];
    }

    public static Vector3I ToGodotVec3(this int[] vec)
    {
        return new Vector3I(vec[0], vec[1], vec[2]);
    }

    public static int[] ToSystemVec(this Vector4I vec)
    {
        return [vec.X, vec.Y, vec.Z];
    }

    public static Vector4I ToGodotVec4(this int[] vec)
    {
        return new Vector4I(vec[0], vec[1], vec[2], vec[3]);
    }

    public static System.Numerics.Vector3 ToSystemVec(this Vector3 vec)
    {
        return new(vec.X, vec.Y, vec.Z);
    }

    public static Vector3 ToGodotVec(this System.Numerics.Vector3 vec)
    {
        return new(vec.X, vec.Y, vec.Z);
    }

    public static System.Numerics.Vector4 ToSystemVec(this Vector4 vec)
    {
        return new(vec.X, vec.Y, vec.Z, vec.W);
    }

    public static Vector4 ToGodotVec(this System.Numerics.Vector4 vec)
    {
        return new(vec.X, vec.Y, vec.Z, vec.W);
    }

    public static System.Numerics.Vector4 ToSystemVec(this Color vec)
    {
        return new(vec.R, vec.G, vec.B, vec.A);
    }

    public static Color ToGodotColor(this System.Numerics.Vector4 vec)
    {
        return new(vec.X, vec.Y, vec.Z, vec.W);
    }

    public static bool DictionaryEditor(Godot.Collections.Dictionary dict, string label)
    {
        const ImGuiTableFlags tableFlags = ScrollX | ScrollY | RowBg
                                           | BordersOuter | BordersV |
                                           Hideable | Reorderable | Resizable;
        var valueChanged = false;
        if (ImGui.BeginTable(label, 2, tableFlags, new System.Numerics.Vector2(0.0f, -100.0f)))
        {
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableSetupColumn("Index");
            ImGui.TableSetupColumn("ResName");
            ImGui.TableHeadersRow();

            foreach (var pair in dict.WithIndex())
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text($"{pair.item.Key}");
                ImGui.TableSetColumnIndex(1);
                var variant = pair.item.Value;
                ImGui.PushID(pair.index);
                if (ImEditVariant("##bal", ref variant))
                {
                    dict[pair.item.Key] = variant;
                    valueChanged = true;
                }
                ImGui.PopID();
            }
            
            ImGui.EndTable();
        }
        
        return valueChanged;
    }

    public static bool TypedArrayEditor<T>(this IEnumerable<T> list, string label)
    {
        return false;
    }
}

public static class IEnumerableExtensions {
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)       
        => self.Select((item, index) => (item, index));
}