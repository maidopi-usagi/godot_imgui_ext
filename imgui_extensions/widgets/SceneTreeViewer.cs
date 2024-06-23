using System.Collections.Generic;
using System.Linq;
using Godot;
using ImGuiNET;

namespace GodotImGuiExtension;

internal static class SceneTreeViewer
{
    private static Node Root { get; } = ((SceneTree)Engine.GetMainLoop()).Root;

    private static readonly List<NodePath> _nodePaths = [];

    public static NodePath CurrentSelectingNodePath => _nodePaths.Last();

    public static bool DrawSceneTree()
    {
        ImGui.Begin("Draw Scene Tree");
        ImGui.Text($"{string.Join(",", _nodePaths)}");
        var clicked = ShowNodeStructure(Root);
        ImGui.End();
        return clicked;
    }
    
    public static bool ShowNodeStructure(Node node)
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
                    clicked |= ShowNodeStructure(child);
                }
                ImGui.TreePop();
            }
        }

        return clicked;
    }
}