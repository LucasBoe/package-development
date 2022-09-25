using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simple.DialogueTree.Nodes
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
        public override void SetNextNode(int optionIndex, DialogueNode nextNode)
        {
            next = nextNode;
        }

        /// <inheritdoc />
        public override void RemoveAsNextNode(DialogueNode childNode)
        {
            if (next == childNode)
                next = null;
        }

        /// <inheritdoc />
        public override List<DialogueNode> GetNextNodes()
        {
            return new List<DialogueNode> { next };
        }

        public override Dictionary<int, DialogueNode> GetNextNodeInfos()
        {
            Dictionary<int, DialogueNode> nodeInfos = new Dictionary<int, DialogueNode>();
            nodeInfos.Add(0, next);
            return nodeInfos;
        }

        #endregion
    }
}