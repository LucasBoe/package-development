using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Simple.DialogueTree.Nodes
{
    [System.Serializable]
    public class ChoiceOption : ScriptableTextObject
    {
        public string Text;
        public DialogueNode Next;
    }
}
