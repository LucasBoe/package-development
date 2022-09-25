using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Simple.DialogueTree.Nodes
{
    [System.Serializable]
    public class ChoiceOption : ScriptableObject, ILocalizableText
    {
        [SerializeField, ReadOnly] string guid;

        public string Text;
        public DialogueNode Next;

        public void GenerateGUID()
        {
            guid = UnityEditor.GUID.Generate().ToString();
        }

        public string GetGUID()
        {
            return guid;
        }
    }
}
