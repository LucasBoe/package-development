using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphViewDialogueTree.Nodes
{
    public class Choice : Node
    {
        /// <value>
        /// The Children that this <see cref="Node"/> contains.
        /// </value>
        [SerializeField, HideInInspector] protected List<Node> children = new List<Node>();

        #region Overrides of Node

        /// <inheritdoc />
        public override void AddChild(Node childNode)
        {
            children.Add(childNode);
        }

        /// <inheritdoc />
        public override void RemoveChild(Node childNode)
        {
            children.Remove(childNode);
        }

        /// <inheritdoc />
        public override List<Node> GetChildren()
        {
            return children;
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
