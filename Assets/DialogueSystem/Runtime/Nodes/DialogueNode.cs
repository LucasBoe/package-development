using System.Collections.Generic;
using UnityEngine;

namespace GraphViewDialogueTree.Nodes
{
    /// <summary>
    /// Base class for all nodes in the Behavior tree.
    /// </summary>
    [System.Serializable]
    public abstract class DialogueNode : ScriptableObject
    {
        /// <value>
        /// The GUID of the Node View, used to get the Node View that this Node is associated with.
        /// </value>
        [HideInInspector] public string guid;

        /// <value>
        /// The Position in the Behavior Tree View that this Node is at.
        /// </value>
        [HideInInspector] public Vector2 nodeGraphPosition;

        /// <value>
        /// Does this node have more then one parent.
        /// </value>
        [HideInInspector] public bool hasMultipleParents;

        #region Virtual Methods

        /// <summary>
        /// Add the child node to this node.
        /// </summary>
        /// <param name="childNode">The Node to add as a Child.</param>
        public virtual void AddChild(DialogueNode childNode) { }

        /// <summary>
        /// Remove a Child from the Node.
        /// </summary>
        /// <param name="childNode">The Child to remove.</param>
        public virtual void RemoveChild(DialogueNode childNode) { }

        /// <summary>
        /// Get a list of children the node contains.
        /// </summary>
        /// <returns>A list of children Nodes.</returns>
        public virtual List<DialogueNode> GetChildren()
        {
            List<DialogueNode> children = new List<DialogueNode>();

            return children;
        }

        #endregion

        /// <summary>
        /// Clone the Node.
        /// </summary>
        /// <returns>A Clone of the Node.</returns>
        public DialogueNode Clone()
        {
            return Instantiate(this);
        }
    }
}
