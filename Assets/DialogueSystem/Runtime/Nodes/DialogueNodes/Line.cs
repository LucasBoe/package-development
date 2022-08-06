using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphViewDialogueTree.Nodes
{
    /// <summary>
    /// <see cref="DialogueNode"/> that contains a single line of text.
    /// </summary>
    [System.Serializable]
    public class Line : DialogueNode
    {
        /// <value>
        /// The <see cref="DialogueNode"/> to Augment.
        /// </value>
        [SerializeField, ReadOnly] protected DialogueNode next;
        public DialogueNode Next => next;

        [SerializeField, ReadOnly] public string Text;


        #region Overrides of Node

        /// <inheritdoc />
        public override void AddChild(DialogueNode childNode)
        {
            next = childNode;
        }

        /// <inheritdoc />
        public override void RemoveChild(DialogueNode childNode)
        {
            if (next == childNode)
                next = null;
        }

        /// <inheritdoc />
        public override List<DialogueNode> GetChildren()
        {
            return new List<DialogueNode> { next };
        }

        #endregion
    }
}