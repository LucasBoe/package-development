using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using DialogueNode = GraphViewDialogueTree.Nodes.DialogueNode;

namespace GraphViewDialogueTree.Editor.Views
{
    /// <summary>
    /// > [!WARNING]
    /// > Experimental: this API is experimental and might be changed or removed in the future.
    /// 
    /// A View for the Behavior Tree, derived from <a href="https://docs.unity3d.com/ScriptReference/Experimental.GraphView.GraphView.html" rel="external">UnityEditor.Experimental.GraphView.GraphView</a>
    /// Can be used in the UI Builder.
    /// </summary>
    public class DialogueTreeView : GraphView
    {
        /// <summary>
        /// Required in order to have <see cref="DialogueTreeView"/> show up in the UI Builder Library.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<DialogueTreeView, UxmlTraits> { }

        /// <summary>
        /// <value>Notifies the Observers that a <see cref="DialogueNode"/> has been Selected and pass the <see cref="DialogueNode"/> that was selected.</value>
        /// </summary>
        public Action<DialogueNode> onNodeSelected;

        /// <summary>
        /// <value>The Tree associated with this view.</value>
        /// </summary>
        private DialogueTree m_tree;

        /// <summary>
        /// <value>Dose the view have a tree</value>
        /// </summary>
        private bool m_hasTree;

        /// <summary>
        /// Creates a new <see cref="DialogueTreeView"/>.
        /// Required in order to have this show up in the UI Builder Library.
        /// </summary>
        public DialogueTreeView()
        {
            style.flexGrow = 1;
            Insert(0, new GridBackground() { name = "grid_background" });
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        /// <summary>
        /// <a href="https://docs.unity3d.com/ScriptReference/Undo-Undo.UndoRedoCallback.html" rel="external">UnityEditor.Undo.UndoRedoCallback</a> assigned to <a href="https://docs.unity3d.com/ScriptReference/Undo-undoRedoPerformed.html" rel="external">UnityEditor.Undo.undoRedoPerformed</a>
        /// </summary>
        private void UndoRedoPerformed()
        {
            PopulateView(m_tree);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Populate the View with the passed in tree
        /// </summary>
        /// <param name="tree">The <see cref="DialogueTree"/> to populate the View from</param>
        public void PopulateView(DialogueTree tree)
        {
            m_tree = tree;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            m_hasTree = m_tree != null;
            if (!m_hasTree) return;
            m_tree.GetNodes().ForEach(CreateNodeView);

            foreach (Edge edge in from node in m_tree.GetNodes()
                                  let parentView = GetNodeByGuid(node.guid) as DialogueTreeNodeView
                                  where parentView is { AllOutputs: { } }
                                  from child in m_tree.GetChildren(node)
                                  where child != null
                                  let childView = GetNodeByGuid(child.guid) as DialogueTreeNodeView
                                  where childView is { Input: { } }
                                  select parentView.AllOutputs[0].ConnectTo(childView.Input))
            {
                AddElement(edge);
            }
        }

        /// <summary>
        /// Hook into the Graph View Change to delete Nodes when the Node View Element is slated to be Removed.
        /// </summary>
        /// <param name="graphViewChange"><a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/Experimental.GraphView.GraphViewChange.html">GraphViewChange</a></param>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (GraphElement element in graphViewChange.elementsToRemove)
                {
                    switch (element)
                    {
                        case DialogueTreeNodeView nodeView:
                            DeleteNode(nodeView.Node);
                            break;
                        case Edge edge:
                            {
                                DialogueTreeNodeView parentView = edge.output.node as DialogueTreeNodeView;
                                DialogueTreeNodeView childView = edge.input.node as DialogueTreeNodeView;
                                m_tree.RemoveChild(parentView.Node, childView.Node);
                                int count = (childView.Input.connections ?? Array.Empty<Edge>()).Count();
                                childView.Node.hasMultipleParents = count > 2;

                                break;
                            }
                    }
                }
            }

            if (graphViewChange.edgesToCreate != null && m_hasTree)
            {
                foreach (Edge edge in graphViewChange.edgesToCreate)
                {
                    DialogueTreeNodeView parentView = edge.output.node as DialogueTreeNodeView;
                    DialogueTreeNodeView childView = edge.input.node as DialogueTreeNodeView;

                    m_tree.AddChild(parentView.Node, childView.Node);

                    int count = (childView.Input.connections ?? Array.Empty<Edge>()).Count();
                    childView.Node.hasMultipleParents = count > 0;
                }
            }

            return graphViewChange;
        }

        /// <summary>
        /// Adds a <see cref="DialogueTreeNodeView"/> from the passed in Node.
        /// </summary>
        /// <param name="node">The <see cref="DialogueNode"/> to create a view for.</param>
        private void CreateNodeView(DialogueNode node)
        {
            DialogueTreeNodeView nodeView = null;



            if (node as GraphViewDialogueTree.Nodes.Choice != null)
            {
                nodeView = new DialogueTreeChoiceNodeView(node as GraphViewDialogueTree.Nodes.Choice)
                {
                    onNodeSelected = onNodeSelected
                };
            } else if (node as GraphViewDialogueTree.Nodes.Line != null)
            {
                nodeView = new DialogueTreeLineNodeView(node as GraphViewDialogueTree.Nodes.Line)
                {
                    onNodeSelected = onNodeSelected
                };
            }

            if (nodeView == null) return;

            if (m_hasTree)
                nodeView.onSetRootNode = _ => m_tree.rootNode = node;

            AddElement(nodeView);
        }

        /// <summary>
        /// Create a new <see cref="DialogueNode"/> with a Node View.
        /// </summary>
        /// <param name="type">The Type of Node to create.</param>
        private void CreateNode(Type type)
        {
            if (!m_hasTree) return;

            DialogueNode node = m_tree.CreateNode(type);
            CreateNodeView(node);
            Undo.RecordObject(m_tree, "Dialogue Tree (Create Node)");

            if (Application.isPlaying) return;

            AssetDatabase.AddObjectToAsset(node, m_tree);
            AssetDatabase.SaveAssets();

            Undo.RegisterCreatedObjectUndo(node, "Dialogue Tree (Create Node)");
            EditorUtility.SetDirty(node);
        }

        /// <summary>
        /// Delete a <see cref="DialogueNode"/> from the tree.
        /// </summary>
        /// <param name="node">The <see cref="DialogueNode"/> to Delete.</param>
        private void DeleteNode(DialogueNode node)
        {
            if (!m_hasTree) return;

            m_tree.DeleteNode(node);
            Undo.RecordObject(m_tree, "Dialogue Tree (Delete Node)");

            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(m_tree);
        }

        #region Overrides of GraphView

        /// <summary>
        /// Override <a href="https://docs.unity3d.com/ScriptReference/Experimental.GraphView.GraphView.BuildContextualMenu.html" rel="external">UnityEditor.Experimental.GraphView.GraphView.BuildContextualMenu</a>
        /// Add menu items to the contextual menu.
        /// </summary>
        /// <param name="evt">The (<a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/UIElements.ContextualMenuPopulateEvent.html" rel="external">UnityEngine.UIElements.ContextualMenuPopulateEvent</a>) event holding the menu to populate.</param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Add Choice", _ => CreateNode(typeof(GraphViewDialogueTree.Nodes.Choice)));
            evt.menu.AppendAction("Add Line", _ => CreateNode(typeof(GraphViewDialogueTree.Nodes.Line)));

            //TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<Node>();
            //foreach (Type type in types)
            //{
            //    if (type.IsAbstract) continue;
            //    evt.menu.AppendAction($"{type.BaseType.Name}/{type.Name}",
            //                          _ => CreateNode(type));
            //}

            base.BuildContextualMenu(evt);
        }

        /// <summary>
        /// Override <a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/Experimental.GraphView.GraphView.GetCompatiblePorts.html" rel="external">UnityEditor.Experimental.GraphView.GraphView.GetCompatiblePorts</a>
        /// Get all ports compatible with given port.
        /// </summary>
        /// <param name="startPort">
        /// <a href="https://docs.unity3d.com/ScriptReference/Experimental.GraphView.Port.html" rel="external">UnityEditor.Experimental.GraphView.Port</a>
        /// Start port to validate against.
        /// </param>
        /// <param name="nodeAdapter">
        /// <a href="https://docs.unity3d.com/ScriptReference/Experimental.GraphView.Port.html" rel="external">UnityEditor.Experimental.GraphView.Port</a>
        /// Node adapter.
        /// </param>
        /// <returns>List of <a href="https://docs.unity3d.com/ScriptReference/Experimental.GraphView.NodeAdapter.html" rel="external">UnityEditor.Experimental.GraphView.NodeAdapter</a> List of compatible ports.</returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()!.Where(endPort =>
                                             endPort.direction != startPort.direction &&
                                             endPort.node != startPort.node &&
                                             endPort.portType == startPort.portType).ToList();
        }

        #endregion
    }
}