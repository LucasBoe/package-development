// DialogueTreeRunner.cs
// 05-13-2022
// James LaFritz

using GraphViewDialogueTree.Nodes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraphViewDialogueTree
{
    /// <summary>
    /// <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html" rel="external">"UnityEngine.MonoBehaviour"</a>
    /// That allows you to run a <see cref="DialogueTree"/> in Unity.
    /// </summary>
    public class DialogueTreeRunner : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="DialogueTree"/> to run.
        /// </summary>
        [SerializeField] public DialogueTree tree;

        [SerializeField] string uiText = "";
        System.Action nextNodeAction;

        List<TreeRunnerButton> buttons = new List<TreeRunnerButton>();

        /// <summary>
        /// Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            if (tree == null) return;
            HandleNode(tree.rootNode);
        }

        private void HandleNode(DialogueNode node)
        {
            if (node as Line != null)
            {
                buttons.Clear();

                Line line = (Line)node;
                uiText = line.Text;
                nextNodeAction = () => HandleNode(line.Next);
            }
            else if (node as Choice != null)
            {
                uiText = null;
                nextNodeAction = null;

                Choice choice = (Choice)node;
                buttons = new List<TreeRunnerButton>();

                foreach (ChoiceOption option in choice.Options)
                {
                    buttons.Add(new TreeRunnerButton() { Text = option.Text, Action = () => HandleNode(option.Next) });
                }
            }
            else
            {
                buttons.Clear();
                uiText = null;
                nextNodeAction = null;
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box(uiText);
            if (nextNodeAction != null)
            {
                if (GUILayout.Button("next"))
                {
                    nextNodeAction?.Invoke();
                }
            }
            GUILayout.EndHorizontal();
            for (int i = 0; i < buttons.Count; i++)
            {
                TreeRunnerButton button = buttons[i];
                if (GUILayout.Button(button.Text))
                    button.Action?.Invoke();
            }
        }

        private class TreeRunnerButton
        {
            public string Text;
            public System.Action Action;
        }
    }
}