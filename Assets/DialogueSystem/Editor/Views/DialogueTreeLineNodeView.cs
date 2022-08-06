using GraphViewDialogueTree.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using DialogueNode = GraphViewDialogueTree.Nodes.DialogueNode;

namespace GraphViewDialogueTree.Editor.Views
{
    public class DialogueTreeLineNodeView : DialogueTreeNodeView
    {
        /// <value>
        /// The Output <a href="https://docs.unity3d.com/ScriptReference/Experimental.GraphView.Port.html" rel="external">UnityEditor.Experimental.GraphView.Port</a>.
        /// </value>
        public Port Output;
        private TextField textField;
        protected override List<Port> GetAllOutputs()
        {
            return new List<Port>() { Output };
        }

        public DialogueTreeLineNodeView(Line lineNode) : base(lineNode, AssetDatabase.GetAssetPath(Resources.Load<VisualTreeAsset>("DialogueTreeLineNodeView")))
        {
            textField = this.Q<TextField>("textField");
            textField.bindingPath = "Text";
            textField.Bind(new SerializedObject(lineNode));
            CreateOutputPorts();
        }

        /// <summary>
        /// Create Output Port based on the Node Type.
        /// </summary>
        private void CreateOutputPorts()
        {

            Output = InstantiatePort(Orientation.Horizontal, Direction.Output,
                                   Port.Capacity.Single, typeof(DialogueNode));

            if (Output == null) return;
            Output.portName = "";
            Output.name = "output-port";
            outputContainer.Add(Output);
        }
    }
}
