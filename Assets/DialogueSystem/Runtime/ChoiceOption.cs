using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GraphViewDialogueTree.Nodes
{
    [System.Serializable]
    public class ChoiceOption : TextNodeBase
    {
        public string Text;
        public DialogueNode Next;
    }
}
