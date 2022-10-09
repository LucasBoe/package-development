using Simple.DialogueTree.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Simple.DialogueTree
{
    public class DialogueTreeTextProcessor : ScriptableObject
    {
        public virtual string FindText(ILocalizeableText text) => text.Text;
        public virtual SerializedProperty FindProperty(ILocalizeableText text) => text.Property;
        public static SerializedProperty GetProperty(ILocalizeableText text)
        {
            DialogueTreeTextProcessor processor = SimpleDialogueSettings.Resolve();

            if (processor != null)
                return processor.FindProperty(text);

            Debug.LogError("no dialogue processor found, please make sure to select one in the project settings.");

            return text.Property;
        }
    }
}
