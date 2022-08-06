using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GraphViewDialogueTree.Nodes
{
    [System.Serializable]
    public class ChoiceOption : ScriptableObject
    {
        public string Text;
        public DialogueNode Next;
    }
}
