using Godot;
using ImGuiNET;
using static ImGuiNET.ImGuiTableFlags;

namespace GodotImGuiExtension;

internal static class VariantExt
{
    private const ImGuiTableFlags TableFlags = ScrollX | ScrollY | RowBg | BordersOuter | BordersV
                                               | Hideable | Reorderable | Resizable;

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

    public static bool ImEditVariant(string label, ref Variant variant, PropertyHint hint = PropertyHint.None,
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
                var floatValue = (float)variant.AsDouble();
                if (hint == PropertyHint.Range)
                {
                    if (hintString.Contains("radians_as_degrees"))
                    {
                        if (ImGui.SliderAngle(label, ref floatValue))
                        {
                            variant = floatValue;
                            return true;
                        }
                    }
                    else
                    {
                        var paramsFloats = hintString.Split(",");
                        if (ImGui.SliderFloat(label, ref floatValue, paramsFloats[0].ToFloat(),
                                paramsFloats[1].ToFloat()))
                        {
                            variant = floatValue;
                            return true;
                        }
                    }
                }
                else
                {
                    if (ImGui.DragFloat(label, ref floatValue, 0.01f))
                    {
                        variant = floatValue;
                        return true;
                    }
                }

                break;
            }
            case Variant.Type.Int:
            {
                var intValue = (int)variant.AsInt64();
                if (hint == PropertyHint.Enum)
                {
                    var enums = hintString.Split(",");
                    if (ImGui.Combo(label, ref intValue, enums, enums.Length))
                    {
                        variant = intValue;
                        return true;
                    }
                }
                else if (hint is PropertyHint.Layers3DPhysics or PropertyHint.Layers2DPhysics)
                {
                    uint uintValue = (uint)intValue;
                    var valueChanged = false;
                    ImGui.Text(label);
                    ImGui.BeginChild(label, new(0.0f, ImGui.GetTextLineHeight() * 8));
                    ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X / 8.0f);
                    for (int i = 0; i < 32; i++)
                    {
                        var u = 1U << i;
                        var flag = (uintValue & u) != 0;
                        if (ImGui.Checkbox($"##{i}{label}", ref flag))
                        {
                            uintValue = flag ? uintValue + u : uintValue - u;
                            variant = uintValue;
                            valueChanged = true;
                        }

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
                else
                {
                    if (ImGui.DragInt(label, ref intValue, 1))
                    {
                        variant = intValue;
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
                var dictValue = variant.AsGodotDictionary();
                var dictChanged = false;
                foreach (var kv in dictValue)
                {
                    var valueVariant = kv.Value;
                    if (ImEditVariant($"{kv.Key}##{label}", ref valueVariant))
                    {
                        dictValue[kv.Key] = valueVariant;
                        dictChanged = true;
                    }
                }

                if (dictChanged) return true;
                break;
            }
            case Variant.Type.Array:
            {
                var arrValue = variant.AsGodotArray();
                if (ImEditArray(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedByteArray:
            {
                var arrValue = variant.AsByteArray();
                if (ImEditArray(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedInt32Array:
            {
                var arrValue = variant.AsInt32Array();
                if (ImEditArray(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedInt64Array:
            {
                var arrValue = variant.AsInt64Array();
                if (ImEditArray(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedFloat32Array:
            {
                var arrValue = variant.AsFloat32Array();
                if (ImEditArray(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedFloat64Array:
            {
                var arrValue = variant.AsFloat64Array();
                if (ImEditArray(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedStringArray:
            {
                var arrValue = variant.AsStringArray();
                if (ImEditArray(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedVector2Array:
            {
                var arrValue = variant.AsVector2Array();
                if (ImEditArray(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedVector3Array:
            {
                var arrValue = variant.AsVector3Array();
                if (ImEditArray(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedVector4Array:
            {
                var arrValue = variant.AsVector4Array();
                if (ImEditArray(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.PackedColorArray:
            {
                var arrValue = variant.AsColorArray();
                if (ImEditArray(label, ref arrValue))
                {
                    variant = arrValue;
                    return true;
                }

                break;
            }
            case Variant.Type.Rect2:
            {
                if (!ImGui.TreeNode(label))
                {
                    break;
                }

                var rectValue = variant.AsRect2();
                var end = rectValue.End;
                var size = rectValue.Size;
                var pos = rectValue.Position;
                var valueChanged = false;
                if (ImEditValue($"Pos##{label}", ref pos))
                {
                    rectValue.Position = pos;
                    valueChanged = true;
                }

                if (ImEditValue($"End##{label}", ref end))
                {
                    rectValue.End = end;
                    valueChanged = true;
                }

                if (ImEditValue($"Size##{label}", ref size))
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
                if (!ImGui.TreeNode(label))
                {
                    break;
                }

                var rectValue = variant.AsRect2I();
                var end = rectValue.End;
                var size = rectValue.Size;
                var pos = rectValue.Position;
                var valueChanged = false;
                if (ImEditValue($"Pos##{label}", ref pos))
                {
                    rectValue.Position = pos;
                    valueChanged = true;
                }

                if (ImEditValue($"End##{label}", ref end))
                {
                    rectValue.End = end;
                    valueChanged = true;
                }

                if (ImEditValue($"Size##{label}", ref size))
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
                if (!ImGui.TreeNode(label))
                {
                    break;
                }

                var xform2DValue = variant.AsTransform2D();
                var origin = xform2DValue.Origin;
                var x = xform2DValue.X;
                var y = xform2DValue.Y;
                var valueChanged = false;
                if (ImEditValue($"Origin##{label}", ref origin))
                {
                    xform2DValue.Origin = origin;
                    valueChanged = true;
                }

                if (ImEditValue($"X##{label}", ref x))
                {
                    xform2DValue.X = x;
                    valueChanged = true;
                }

                if (ImEditValue($"Y##{label}", ref y))
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
                if (!ImGui.TreeNode(label))
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
                if (ImEditValue($"Distance##{label}", ref d))
                {
                    planeValue.D = d;
                    valueChanged = true;
                }

                if (ImEditValue($"End##{label}", ref normal))
                {
                    planeValue.Normal = normal;
                    valueChanged = true;
                }

                if (ImEditValue($"X##{label}", ref x))
                {
                    planeValue.X = x;
                    valueChanged = true;
                }

                if (ImEditValue($"Y##{label}", ref y))
                {
                    planeValue.Y = y;
                    valueChanged = true;
                }

                if (ImEditValue($"Z##{label}", ref z))
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
                if (!ImGui.TreeNode(label))
                {
                    break;
                }

                var quatValue = variant.AsQuaternion();
                var w = quatValue.W;
                var x = quatValue.X;
                var y = quatValue.Y;
                var z = quatValue.Z;
                var valueChanged = false;
                if (ImEditValue($"W##{label}", ref w))
                {
                    quatValue.W = w;
                    valueChanged = true;
                }

                if (ImEditValue($"X##{label}", ref x))
                {
                    quatValue.X = x;
                    valueChanged = true;
                }

                if (ImEditValue($"Y##{label}", ref y))
                {
                    quatValue.Y = y;
                    valueChanged = true;
                }

                if (ImEditValue($"Z##{label}", ref z))
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
                if (!ImGui.TreeNode(label))
                {
                    break;
                }

                var aabbValue = variant.AsAabb();
                var end = aabbValue.End;
                var size = aabbValue.Size;
                var pos = aabbValue.Position;
                var valueChanged = false;
                if (ImEditValue($"Pos##{label}", ref pos))
                {
                    aabbValue.Position = pos;
                    valueChanged = true;
                }

                if (ImEditValue($"End##{label}", ref end))
                {
                    aabbValue.End = end;
                    valueChanged = true;
                }

                if (ImEditValue($"Size##{label}", ref size))
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
                if (!ImGui.TreeNode(label))
                {
                    break;
                }

                var basisValue = variant.AsBasis();
                var x = basisValue.X;
                var y = basisValue.Y;
                var z = basisValue.Z;
                var valueChanged = false;
                if (ImEditValue($"X##{label}", ref x))
                {
                    basisValue.X = x;
                    valueChanged = true;
                }

                if (ImEditValue($"Y##{label}", ref y))
                {
                    basisValue.Y = y;
                    valueChanged = true;
                }

                if (ImEditValue($"Z##{label}", ref z))
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
                if (!ImGui.TreeNode(label))
                {
                    break;
                }

                var xform2DValue = variant.AsTransform3D();
                var origin = xform2DValue.Origin;
                var basis = xform2DValue.Basis;
                var valueChanged = false;
                if (ImEditValue($"Origin##{label}", ref origin))
                {
                    xform2DValue.Origin = origin;
                    valueChanged = true;
                }

                if (ImEditValue($"Basis##{label}", ref basis))
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
                if (!ImGui.TreeNode(label))
                {
                    break;
                }

                var projectionValue = variant.AsProjection();
                var w = projectionValue.W;
                var x = projectionValue.X;
                var y = projectionValue.Y;
                var z = projectionValue.Z;
                var valueChanged = false;
                if (ImEditValue($"W##{label}", ref w))
                {
                    projectionValue.W = w;
                    valueChanged = true;
                }

                if (ImEditValue($"X##{label}", ref x))
                {
                    projectionValue.X = x;
                    valueChanged = true;
                }

                if (ImEditValue($"Y##{label}", ref y))
                {
                    projectionValue.Y = y;
                    valueChanged = true;
                }

                if (ImEditValue($"Z##{label}", ref z))
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

    private static unsafe bool ImEditArray<[MustBeVariant] T>(string label, ref T[] arrValue)
    {
        var valueChanged = false;
        var rowCount = arrValue.Length;
        if (ImGui.TreeNode($"[{rowCount}]{label}"))
        {
            if (ImGui.BeginTable(label, 2, TableFlags, new System.Numerics.Vector2(0.0f, 240.0f)))
            {
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableSetupColumn("Index", ImGuiTableColumnFlags.WidthFixed, 32);
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
                        ImGui.Text($"{row}");
                        ImGui.TableSetColumnIndex(1);
                        var value = arrValue[row];
                        if (value is not Variant variant)
                        {
                            variant = Variant.From(value);
                        }

                        if (ImEditVariant($"##{row}{label}", ref variant))
                        {
                            arrValue[row] = variant.As<T>();
                            valueChanged = true;
                        }
                    }
                }

                ImGui.EndTable();
            }

            ImGui.TreePop();
        }

        return valueChanged;
    }

    private static unsafe bool ImEditArray(string label, ref Godot.Collections.Array arrValue)
    {
        var valueChanged = false;
        var rowCount = arrValue.Count;
        if (ImGui.TreeNode($"[{rowCount}]{label}"))
        {
            if (ImGui.BeginTable(label, 2, TableFlags, new System.Numerics.Vector2(0.0f, 240.0f)))
            {
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableSetupColumn("Index", ImGuiTableColumnFlags.WidthFixed, 32);
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
                        ImGui.Text($"{row}");
                        ImGui.TableSetColumnIndex(1);
                        var value = arrValue[row];
                        if (ImEditVariant($"##{row}{label}", ref value))
                        {
                            arrValue[row] = value;
                            valueChanged = true;
                        }
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
}