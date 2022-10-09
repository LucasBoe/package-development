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
        public virtual string GetText(ILocalizeableText text) => text.TextValue;
        public virtual SerializedProperty GetProperty(ILocalizeableText text) => text.TextProperty;
        public static SerializedProperty FindProperty(ILocalizeableText text)
        {
            DialogueTreeTextProcessor processor = SimpleDialogueSettings.Resolve();

            if (processor != null)
                return processor.GetProperty(text);

            Debug.LogError("no dialogue processor found, please make sure to select one in the project settings.");

            return text.TextProperty;
        }
    }
}
