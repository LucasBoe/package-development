using Simple.DialogueTree.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using DialogueNode = Simple.DialogueTree.Nodes.DialogueNode;

namespace Simple.DialogueTree.Editor.Views
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

        public DialogueTreeLineNodeView(Line lineNode, DialogueTreeView tree) : base(lineNode, tree, AssetDatabase.GetAssetPath(Resources.Load<VisualTreeAsset>("DialogueTreeLineNodeView")))
        {
            textField = this.Q<TextField>("textField");
            textField.bindingPath = "Text";

            SerializedProperty property = DialogueTreeTextProcessor.GetProperty(lineNode);

            bool propertyExists = property != null;

            if (!lineNode.IsLocalized || !propertyExists)
                textField.Remove(textField.Q<Label>("localized"));

            if (propertyExists)
            {
                textField.BindProperty(property);
            }
            else
            {
                textField.SetValueWithoutNotify("<no translation in this language yet>");
                textField.isReadOnly = true;
            }
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
