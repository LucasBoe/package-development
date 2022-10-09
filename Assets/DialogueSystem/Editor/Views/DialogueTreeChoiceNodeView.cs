using Simple.DialogueTree.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Simple.DialogueTree.Editor.Views
{
    public class DialogueTreeChoiceNodeView : DialogueTreeNodeView
    {
        List<Port> choiceOutputPorts;
        protected override List<Port> GetAllOutputs()
        {
            return choiceOutputPorts;
        }
        public DialogueTreeChoiceNodeView(Choice node, DialogueTreeView tree) : base(node, tree, AssetDatabase.GetAssetPath(Resources.Load<VisualTreeAsset>("DialogueTreeChoiceNodeView")))
        {
            choiceOutputPorts = new List<Port>();

            VisualElement contents = this.Q<VisualElement>("contents");

            for (int i = 0; i < 5; i++)
            {
                bool exists = i < node.Options.Count;
                VisualElement element = this.Q<VisualElement>((i + 1).ToString());

                if (!exists)
                    contents.Remove(element);
                else
                {
                    ChoiceOption option = node.Options[i];
                    SerializedProperty property = DialogueTreeTextProcessor.GetProperty(option);
                    TextField textField = element.Q<TextField>("textField");

                    bool properyExists = property != null;

                    if (!option.IsLocalized || !properyExists)
                        textField.Remove(textField.Q<Label>("localized"));             

                    if (properyExists)
                    {
                        textField.BindProperty(property);
                    }
                    else
                    {
                        textField.SetValueWithoutNotify("<no translation in this language yet>");
                        textField.isReadOnly = true;
                    }

                    VisualElement outputContainer = element.Q<VisualElement>("output");

                    Port output = InstantiatePort(Orientation.Horizontal, Direction.Output,
                          Port.Capacity.Single, typeof(DialogueNode));
                    if (output != null)
                    {
                        output.portName = "";
                        output.name = "output-port";

                        outputContainer.Add(output);
                        choiceOutputPorts.Add(output);
                    }
                }
            }

            this.Q<Button>("add").clicked += () =>
            {
                if (node.Options.Count < 5)
                {
                    ChoiceOption optObj = ChoiceOption.CreateInstance(typeof(ChoiceOption)) as ChoiceOption;
                    optObj.name = node.name + " > Option " + node.Options.Count;
                    optObj.GenerateGUID();

                    Undo.RecordObject(node, "Dialogue Choice (Create Option)");

                    if (Application.isPlaying) return;

                    AssetDatabase.AddObjectToAsset(optObj, node);
                    AssetDatabase.SaveAssets();

                    Undo.RegisterCreatedObjectUndo(optObj, "Dialogue Choice (Create Option");
                    EditorUtility.SetDirty(optObj);

                    node.Options.Add(optObj);
                    TryUpdateTree();
                }
            };

            this.Q<Button>("remove").clicked += () =>
            {
                if (node.Options.Count > 0)
                {
                    ChoiceOption optObj = node.Options[node.Options.Count - 1];
                    node.Options.Remove(optObj);

                    Undo.RecordObject(node, "Dialogue Choice (Delete Option)");

                    Undo.DestroyObjectImmediate(optObj);
                    AssetDatabase.SaveAssets();
                    EditorUtility.SetDirty(node);

                    TryUpdateTree();
                }
            };
        }

        private void TryUpdateTree()
        {
            if (Tree != null)
                Tree.ForceVisualUpdate();
        }
    }
}
