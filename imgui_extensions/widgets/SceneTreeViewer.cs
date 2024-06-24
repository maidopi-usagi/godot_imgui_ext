using System.Collections.Generic;
using Godot;
using ImGuiNET;

namespace GodotImGuiExtension;

internal static class SceneTreeViewer
{
    private static Node Root { get; } = ((SceneTree)Engine.GetMainLoop()).Root;

    public static bool DrawSceneTree(ref List<NodePath> _nodePaths)
    {
        ImGui.Begin("Draw Scene Tree");
        ImGui.Text($"{string.Join(",", _nodePaths)}");
        var clicked = ShowNodeStructure(Root, ref _nodePaths);
        ImGui.End();
        return clicked;
    }
    
    public static bool ShowNodeStructure(Node node,ref  List<NodePath> _nodePaths)
    {
        var clicked = false;
        var flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick
                                                   | ImGuiTreeNodeFlags.SpanAvailWidth;
        var nodePath = node.GetPath();
        var inSelection = _nodePaths.Contains(nodePath);
        var keyCtrl = ImGui.GetIO().KeyCtrl;
        if (inSelection)
        {
            flags |= ImGuiTreeNodeFlags.Selected;
        }
        
        var hasChildren = node.GetChildCount() > 0;
        var nodeName = $"{(node.Name.IsEmpty ? node.GetClass() : node.Name)}##{node}";
        if (!hasChildren)
        {
            if (ImGui.Selectable(nodeName, inSelection))
            {
                if (keyCtrl)
                {
                    if (!inSelection)
                    {
                        _nodePaths.Add(nodePath);
                    }
                    else
                    {
                        _nodePaths.Remove(nodePath);
                    }
                }
                else
                {
                    _nodePaths.Clear();
                    _nodePaths.Add(nodePath);
                }
                clicked = true;
            }
        }
        else
        {
            var nodeOpen = ImGui.TreeNodeEx(nodeName, flags: flags);
            var isItemClicked = ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen();
            if (isItemClicked)
            {
                if (keyCtrl)
                {
                    if (!inSelection)
                    {
                        _nodePaths.Add(nodePath);
                    }
                    else
                    {
                        _nodePaths.Remove(nodePath);
                    }
                }
                else
                {
                    _nodePaths.Clear();
                    _nodePaths.Add(nodePath);
                }
                clicked = true;
            }
            if (nodeOpen)
            {
                var nodes = node.GetChildren();
                for (var index = 0; index < nodes.Count; index++)
                {
                    var child = nodes[index];
                    clicked |= ShowNodeStructure(child, ref _nodePaths);
                }
                ImGui.TreePop();
            }
        }

        return clicked;
    }
}