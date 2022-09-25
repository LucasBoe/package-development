using GraphViewDialogueTree.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphViewDialogueTree.Editor.Views
{
    public class DialogueTreeChoiceNodeView : DialogueTreeNodeView
    {
        List<Port> choiceOutputPorts;
        protected override List<Port> GetAllOutputs()
        {
            return choiceOutputPorts;
        }
        public DialogueTreeChoiceNodeView(Choice node) : base(node, AssetDatabase.GetAssetPath(Resources.Load<VisualTreeAsset>("DialogueTreeChoiceNodeView")))
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

                    TextField textField = element.Q<TextField>("textField");
                    textField.bindingPath = "Text";
                    textField.Bind(new SerializedObject(option));

                    VisualElement outputContainer = element.Q<VisualElement>("output");

                    Debug.LogWarning(outputContainer);

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

                    DialogueTreeEditor.UpdateDialogueTreeEditorManuallyEvent?.Invoke();
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

                    DialogueTreeEditor.UpdateDialogueTreeEditorManuallyEvent?.Invoke();
                }
            };
        }
    }
}
