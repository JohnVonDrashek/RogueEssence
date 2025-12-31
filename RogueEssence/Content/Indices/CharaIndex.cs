using System.Collections.Generic;
using System.IO;

namespace RogueEssence.Content
{
    /// <summary>
    /// A hierarchical index node for locating character sprite data within a binary file.
    /// Forms a tree structure where each level represents a different attribute (species, form, skin, gender).
    /// </summary>
    public class CharaIndexNode
    {
        /// <summary>
        /// The byte position in the file where this node's data is located.
        /// </summary>
        public long Position;

        /// <summary>
        /// Child nodes indexed by their ID.
        /// </summary>
        public Dictionary<int, CharaIndexNode> Nodes;

        /// <summary>
        /// Creates a new empty CharaIndexNode.
        /// </summary>
        public CharaIndexNode()
        {
            Nodes = new Dictionary<int, CharaIndexNode>();
        }

        /// <summary>
        /// Loads a CharaIndexNode tree from a binary stream.
        /// </summary>
        /// <param name="reader">The binary reader to read from.</param>
        /// <returns>The root node of the loaded index tree.</returns>
        public static CharaIndexNode Load(BinaryReader reader)
        {
            CharaIndexNode node = new CharaIndexNode();
            node.Position = reader.ReadInt64();
            int count = reader.ReadInt32();
            for(int ii = 0; ii < count; ii++)
            {
                int id = reader.ReadInt32();
                CharaIndexNode subNode = CharaIndexNode.Load(reader);
                node.Nodes[id] = subNode;
            }
            return node;
        }

        /// <summary>
        /// Saves this CharaIndexNode tree to a binary stream.
        /// </summary>
        /// <param name="writer">The binary writer to write to.</param>
        public void Save(BinaryWriter writer)
        {
            writer.Write(Position);
            writer.Write(Nodes.Count);
            foreach(int key in Nodes.Keys)
            {
                writer.Write(key);
                Nodes[key].Save(writer);
            }
        }

        /// <summary>
        /// Adds a position entry to the index tree at the specified path.
        /// </summary>
        /// <param name="position">The byte position to store.</param>
        /// <param name="subIDs">The path of IDs to the entry. Use -1 to set the current node's position.</param>
        public void AddSubValue(long position, params int[] subIDs)
        {
            addSubValue(position, subIDs, 0);
        }
        private void addSubValue(long position, int[] subIDs, int subIDIndex)
        {
            if (subIDIndex < subIDs.Length)
            {
                int nodeIndex = subIDs[subIDIndex];
                if (nodeIndex == -1)
                    Position = position;
                else
                {
                    if (!Nodes.ContainsKey(nodeIndex))
                        Nodes[nodeIndex] = new CharaIndexNode();
                    Nodes[nodeIndex].addSubValue(position, subIDs, subIDIndex + 1);
                }
            }
            else
                Position = position;
        }

        /// <summary>
        /// Gets the byte position stored at the specified path in the index tree.
        /// </summary>
        /// <param name="subIDs">The path of IDs to look up. Use -1 to get the current node's position.</param>
        /// <returns>The stored byte position, or 0 if not found.</returns>
        public long GetPosition(params int[] subIDs)
        {
            return getPosition(subIDs, 0);
        }

        private long getPosition(int[] subIDs, int subIDIndex)
        {
            if (subIDIndex < subIDs.Length)
            {
                int nodeIndex = subIDs[subIDIndex];
                if (nodeIndex == -1)
                    return Position;
                else
                {
                    if (!Nodes.ContainsKey(nodeIndex))
                        return 0;
                    return Nodes[nodeIndex].getPosition(subIDs, subIDIndex + 1);
                }
            }
            else
                return Position;
        }
    }
}
