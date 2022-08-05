using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphViewDialogueTree.Nodes
{
    /// <summary>
    /// <see cref="Node"/> that contains a single line of text.
    /// </summary>
    [System.Serializable]
    public class Line : Node
    {
        /// <value>
        /// The <see cref="Node"/> to Augment.
        /// </value>
        [SerializeField, HideInInspector] protected Node child;

        #region Overrides of Node

        /// <inheritdoc />
        public override void AddChild(Node childNode)
        {
            child = childNode;
        }

        /// <inheritdoc />
        public override void RemoveChild(Node childNode)
        {
            if (child == childNode)
                child = null;
        }

        /// <inheritdoc />
        public override List<Node> GetChildren()
        {
            return new List<Node> { child };
        }

        #endregion
        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            return State.Running;
        }
    }
}
