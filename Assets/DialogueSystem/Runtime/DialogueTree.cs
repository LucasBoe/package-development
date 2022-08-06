// DialogueTree.cs
// 05-05-2022
// James LaFritz

using System.Collections.Generic;
using System.Linq;
using GraphViewDialogueTree.Nodes;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace GraphViewDialogueTree
{
    /// <summary>
    /// Behavior tree is an execution tree, requires that the Root Node be set, derived from <a hfref="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/ScriptableObject.html">UnityEngine.ScriptableObject</a>
    /// </summary>
    [CreateAssetMenu(fileName = "DialogueTree", menuName = "Behavior Tree")]
    [System.Serializable]
    public class DialogueTree : ScriptableObject
    {
        /// <value>
        /// The Node to start the Behavior tree.
        /// </value>
        [HideInInspector] public DialogueNode rootNode;

        /// <value>
        /// The Nodes that the tree has.
        /// </value>
        [SerializeField, ReadOnly] private List<DialogueNode> nodes = new List<DialogueNode>();

        /// <value>
        /// Get all of the Nodes in the Tree.
        /// </value>
        public List<DialogueNode> GetNodes()
        {
            return nodes;
        }

        /// <value>
        /// Does the Tree have a Root Node.
        /// </value>
        private bool m_hasRootNode;

        /// <summary>
        /// Create a new Node and add it to the nodes.
        /// </summary>
        /// <param name="type">The Type of Node to create.</param>
        public DialogueNode CreateNode(System.Type type)
        {
            DialogueNode node = CreateInstance(type) as DialogueNode;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();

            nodes.Add(node);

            if (rootNode == null)
                rootNode = node;

            return node;
        }

        /// <summary>
        /// Delete a Node from the tree.
        /// </summary>
        /// <param name="node">The Node to Delete.</param>
        public void DeleteNode(DialogueNode node)
        {
            nodes.Remove(node);

            if (rootNode == node)
            {
                rootNode = null;

                if (nodes.Count > 0)
                {
                    rootNode = nodes[0];
                }
            }
        }

        /// <summary>
        /// Add a child node to the parent.
        /// </summary>
        /// <param name="parent">The parent Node.</param>
        /// <param name="child">The Node to add to the parent.</param>
        public void AddChild(DialogueNode parent, DialogueNode child)
        {
            if (!nodes.Contains(parent)) return;

            nodes[nodes.IndexOf(parent)].AddChild(child);

            if (nodes.Contains(child)) return;

            nodes.Add(child);
        }

        /// <summary>
        /// Remove a node from the parent.
        /// </summary>
        /// <param name="parent">The parent Node.</param>
        /// <param name="child">The Node to remove from the parent.</param>
        public void RemoveChild(DialogueNode parent, DialogueNode child)
        {
            if (!nodes.Contains(parent)) return;

            nodes[nodes.IndexOf(parent)].RemoveChild(child);
        }

        /// <summary>
        /// Get a list of children from the parent.
        /// </summary>
        /// <param name="parent">The node to get the children from</param>
        /// <returns>A list of children Nodes that the parent contains.</returns>
        public List<DialogueNode> GetChildren(DialogueNode parent)
        {
            return !nodes.Contains(parent)
                ? new List<DialogueNode>()
                : nodes[nodes.IndexOf(parent)].GetChildren();
        }

        /// <summary>
        /// Traverse the node and run the Action.
        /// </summary>
        //public void Traverse(DialogueNode node, System.Action<DialogueNode> visitor)
        //{
        //    if (!node) return;
        //    visitor?.Invoke(node);
        //    node.GetChildren()?.ForEach((n) => Traverse(n, visitor));
        //}

        /// <summary>
        /// Clone the Tree.
        /// </summary>
        /// <returns>A Clone of the tree</returns>
        //public DialogueTree Clone()
        //{
        //    DialogueTree tree = Instantiate(this);
        //    tree.nodes = new List<DialogueNode>();
        //    foreach (DialogueNode node in nodes)
        //    {
        //        tree.nodes.Add(node.Clone());
        //    }
        //
        //    tree.rootNode = tree.nodes[nodes.IndexOf(rootNode)];
        //    Traverse(rootNode, (n) =>
        //    {
        //        int nodeIndex = nodes.IndexOf(n);
        //        foreach (int childIndex in nodes[nodeIndex]?.GetChildren().Select(c => nodes.IndexOf(c)))
        //        {
        //            tree.nodes[nodeIndex].RemoveChild(nodes[childIndex]);
        //            tree.AddChild(tree.nodes[nodeIndex], tree.nodes[childIndex]);
        //        }
        //    });
        //
        //    return tree;
        //}
    }
}