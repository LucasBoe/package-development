using Simple.DialogueTree.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simple.DialogueTree
{
    [CreateAssetMenu]
    public class DialogueTreeTextProcessor : ScriptableObject
    {
        public virtual string GetText(Line line)
        {
            return line.Text;
        }
        public virtual string GetText(ChoiceOption option)
        {
            return option.Text;
        }
    }
}
