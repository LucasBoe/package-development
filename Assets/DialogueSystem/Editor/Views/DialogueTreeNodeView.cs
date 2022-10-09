using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Simple.DialogueTree.Nodes;
using UnityEditor.Experimental.GraphView;
using DialogueNode = Simple.DialogueTree.Nodes.DialogueNode;

namespace Simple.DialogueTree.Editor.Views
{
    /// <summary>
    /// > [!WARNING]
    /// > Experimental: this API is experimental and might be changed or removed in the future.
    /// 
    /// A View for Behavior Tree <see cref="Nodes.DialogueNode"/>, derived from <a href="https://docs.unity3d.com/ScriptReference/Experimental.GraphView.Node.html" rel="external">UnityEditor.Experimental.GraphView.Node</a>
    /// </summary>
    public class DialogueTreeNodeView : UnityEditor.Experimental.GraphView.Node
    {
        /// <value>
        /// The Node Associated with this view
        /// </value>
        private DialogueNode myNode;

        /// <value>
        /// Notifies the Observers that a <see cref="Nodes.DialogueNode"/> has been Selected and pass the <see cref="Nodes.DialogueNode"/> that was selected.
        /// </value>
        public Action<DialogueNode> onNodeSelected;

        /// <value>
        /// Notifies Observers that the set root node has been selected. Pass the <see cref="Nodes.DialogueNode"/> that was selected to be set as the root node.
        /// </value>
        public Action<DialogueNode> onSetRootNode;

        /// <value>
        /// The <see cref="Nodes.DialogueNode"/> that is associate with this view.
        /// </value>
        public DialogueNode Node => myNode;

        /// <value>The Input <a href="https://docs.unity3d.com/ScriptReference/Experimental.GraphView.Port.html" rel="external">UnityEditor.Experimental.GraphView.Port</a></value>
        public Port Input;

        public List<Port> AllOutputs
        {
            get
            {
                return GetAllOutputs();
            }
        }

        protected virtual List<Port> GetAllOutputs()
        {
            return new List<Port>();
        }

        private readonly Label description;
        protected DialogueTreeView Tree;

        /// <summary>
        /// Create a New Node View.
        /// </summary>
        /// <param name="node"><see cref="Nodes.DialogueNode"/> that is associated with this view.</param>
        public DialogueTreeNodeView(DialogueNode node, DialogueTreeView tree, string uiFile) : base(uiFile)
        {
            //description = this.Q<Label>("description-label");
            this.Tree = tree;
            myNode = node;
            if (myNode == null) return;
            base.title = myNode.GetType().Name;
            viewDataKey = myNode.guid;
            style.left = myNode.nodeGraphPosition.x;
            style.top = myNode.nodeGraphPosition.y;

            CreateInputPorts();
            SetupClasses();
        }

        /// <summary>
        /// Adds Classes to the Node View depending on the <see cref="DialogueNode.State"/> of <see cref="myNode"/>
        /// </summary>
        private void SetupClasses()
        {
            if (myNode as Choice != null)
            {
                AddToClassList("choice");
            }
            else if (myNode as Line != null)
            {
                AddToClassList("line");
            }
        }

        /// <summary>
        /// Create an Input port for all Node Types
        /// </summary>
        private void CreateInputPorts()
        {
            Input = InstantiatePort(Orientation.Horizontal, Direction.Input,
                                    Port.Capacity.Multi, typeof(DialogueNode));
            if (Input == null) return;
            Input.portName = "";
            Input.name = "input-port";
            inputContainer.Add(Input);
        }

        #region Overrides of Node

        /// <summary>
        /// Override <a href="https://docs.unity3d.com/ScriptReference/Experimental.GraphView.Node.SetPosition.html" rel="external">UnityEditor.Experimental.GraphView.Node.SetPosition</a>
        /// Set node position.
        /// </summary>
        /// <param name="newPos"><a href="https://docs.unity3d.com/ScriptReference/Rect.html" rel="external">UnityEngine.Rect</a> New position.</param>
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(myNode, "Dialogue Tree (Set Position)");
            myNode.nodeGraphPosition.x = newPos.xMin;
            myNode.nodeGraphPosition.y = newPos.yMin;
            EditorUtility.SetDirty(myNode);
        }

        /// <summary>
        /// Override <a href="https://docs.unity3d.com/ScriptReference/Experimental.GraphView.Node.BuildContextualMenu.html" rel="external">UnityEditor.Experimental.GraphView.Node.BuildContextualMenu</a>
        /// Add menu items to the node contextual menu.
        /// </summary>
        /// <param name="evt">The (<a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/UIElements.ContextualMenuPopulateEvent.html" rel="external">UnityEngine.UIElements.ContextualMenuPopulateEvent</a>) event holding the menu to populate.</param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction($"Set as Root Node", _ => onSetRootNode?.Invoke(myNode));
            base.BuildContextualMenu(evt);
        }

        #endregion

        #region Overrides of GraphElement

        /// <summary>
        /// Override <a href="https://docs.unity3d.com/ScriptReference/Experimental.GraphView.GraphElement.OnSelected.html" rel="external">UnityEditor.Experimental.GraphView.GraphElement.OnSelected</a>
        /// Called when the GraphElement is selected.
        /// </summary>
        public override void OnSelected()
        {
            onNodeSelected?.Invoke(myNode);
            base.OnSelected();
        }

        #endregion
    }
}