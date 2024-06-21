using System.Collections.Generic;
using Godot;

namespace GodotUtilities {
    /// <summary>
    /// A static class containing some extension methods for Godot <c>Node</c> objects.
    /// </summary>
    public static class GodotExtensions {
        /// <summary>
        /// Retrieve all sub-nodes in the sub-tree rooted at <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The root node.</param>
        /// <returns>A <c>Stack</c> containing all nodes in the sub-tree. All parent nodes are placed below all of their children in this stack.</returns>
        public static Stack<Node> GetSubTree(this Node node) {
            Stack<Node> offsprings = [];
            offsprings.Push(node);
            foreach (Node child in node.GetChildren()) {
                Stack<Node> lowerOffsprings = child.GetSubTree();
                while (lowerOffsprings.Count > 0) {
                    offsprings.Push(lowerOffsprings.Pop());
                }
            }
            return offsprings;
        }

        /// <summary>
        /// Unsubscribe all events from <paramref name="node"/> and remove it from the memory.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        public static void Recycle(this Node node) {
            Stack<Node> offsprings = node.GetSubTree();
            while (offsprings.Count > 0) {
                Node curr = offsprings.Pop();
                curr.UnsubscribeAllEvents();
                curr.QueueFree();
            }
        }
    }
}