using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphViewDialogueTree.Nodes
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
        public override void AddChild(DialogueNode childNode)
        {
            options.Add(new ChoiceOption() { Next = childNode });
        }

        /// <inheritdoc />
        public override void RemoveChild(DialogueNode childNode)
        {
            for (int i = options.Count - 1; i > 0; i--)
            {
                if (options[i].Next == childNode)
                    options.RemoveAt(i);
            }
        }

        /// <inheritdoc />
        public override List<DialogueNode> GetChildren()
        {
            return options.Select(o => o.Next).ToList();
        }

        #endregion
    }
}
