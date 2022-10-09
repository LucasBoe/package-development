using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Simple.DialogueTree.Nodes
{
    [System.Serializable]
    public class ChoiceOption : ScriptableObject, ILocalizeableText
    {
        [SerializeField, ReadOnly] string guid;
        [SerializeField, ReadOnly] bool isLocalized = false;
        public bool IsLocalized { get => isLocalized; }
        public string Text;
        string ILocalizeableText.Text => Text;
        public SerializedProperty Property { get => new SerializedObject(this).FindProperty("Text"); }

        public DialogueNode Next;
        public void GenerateGUID() => guid = UnityEditor.GUID.Generate().ToString();
        public string GetGUID() => guid;
        public string GetValue() => Text;
        public void SetIsLocalized(bool localized, string key = "")
        {
            isLocalized = localized;
            if (isLocalized) Text = key;
        }
    }
}
