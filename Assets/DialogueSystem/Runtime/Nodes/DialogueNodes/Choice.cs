using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Simple.DialogueTree.Nodes
{
    public class Choice : DialogueNode
    {
        /// <value>
        /// The Children that this <see cref="DialogueNode"/> contains.
        /// </value>
        [SerializeField] protected List<ChoiceOption> options = new List<ChoiceOption>();
        public List<ChoiceOption> Options => options;

        #region Overrides of Node

        /// <inheritdoc />
        public override void SetNextNode(int optionIndex, DialogueNode nextNode)
        {
            Debug.Log("Set Next from Index: " + optionIndex);

            options[optionIndex].Next = nextNode;
        }

        /// <inheritdoc />
        public override void RemoveAsNextNode(DialogueNode childNode)
        {
            for (int i = options.Count - 1; i > 0; i--)
            {
                if (options[i].Next == childNode)
                    options.RemoveAt(i);
            }
        }

        /// <inheritdoc />
        public override List<DialogueNode> GetNextNodes()
        {
            return options.Select(o => o.Next).ToList();
        }

        public override Dictionary<int, DialogueNode> GetNextNodeInfos()
        {
            Dictionary<int, DialogueNode> nodeInfos = new Dictionary<int, DialogueNode>();

            for (int i = 0; i < options.Count; i++)
            {
                nodeInfos.Add(i, options[i].Next);
            }

            return nodeInfos;
        }

        #endregion
    }
}
