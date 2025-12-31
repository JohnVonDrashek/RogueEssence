using System;
using System.Collections.Generic;
using System.IO;
using RogueElements;

namespace RogueEssence.Content
{
    /// <summary>
    /// An index node for locating tile data within a binary file.
    /// Maps 2D tile coordinates to byte positions in the file.
    /// </summary>
    public class TileIndexNode
    {
        /// <summary>
        /// The size of each tile in pixels.
        /// </summary>
        public int TileSize;

        /// <summary>
        /// Maps tile coordinates to byte positions in the file.
        /// </summary>
        public Dictionary<Loc, long> Positions;

        /// <summary>
        /// Creates a new empty TileIndexNode.
        /// </summary>
        public TileIndexNode()
        {
            Positions = new Dictionary<Loc, long>();
        }

        /// <summary>
        /// Loads a TileIndexNode from a binary stream.
        /// </summary>
        /// <param name="reader">The binary reader to read from.</param>
        /// <returns>The loaded TileIndexNode.</returns>
        public static TileIndexNode Load(BinaryReader reader)
        {
            TileIndexNode node = new TileIndexNode();
            node.TileSize = reader.ReadInt32();
            int count = reader.ReadInt32();
            for (int ii = 0; ii < count; ii++)
            {
                Loc id = new Loc(reader.ReadInt32(), reader.ReadInt32());
                long position = reader.ReadInt64();
                node.Positions[id] = position;
            }
            return node;
        }

        /// <summary>
        /// Saves this TileIndexNode to a binary stream.
        /// </summary>
        /// <param name="writer">The binary writer to write to.</param>
        public void Save(BinaryWriter writer)
        {
            writer.Write(TileSize);
            writer.Write(Positions.Count);
            foreach (Loc key in Positions.Keys)
            {
                writer.Write(key.X);
                writer.Write(key.Y);
                writer.Write(Positions[key]);
            }
        }


        public long GetPosition(Loc tex)
        {
            long val;
            if (Positions.TryGetValue(tex, out val))
                return val;
            else
                return 0;
        }

        public Loc GetTileDims()
        {
            Loc loc = new Loc();
            foreach (Loc key in Positions.Keys)
            {
                loc.X = Math.Max(loc.X, key.X);
                loc.Y = Math.Max(loc.Y, key.Y);
            }
            return loc + new Loc(1);
        }
    }

    public class TileGuide
    {
        public Dictionary<string, TileIndexNode> Nodes;

        public TileGuide()
        {
            Nodes = new Dictionary<string, TileIndexNode>();
        }

        public long GetPosition(string sheet, Loc tex)
        {
            TileIndexNode node;
            if (Nodes.TryGetValue(sheet, out node))
                return node.GetPosition(tex);
            else
                return 0;

        }

        public Loc GetTileDims(string sheetNum)
        {
            TileIndexNode node;
            if (Nodes.TryGetValue(sheetNum, out node))
                return node.GetTileDims();
            else
                return new Loc();
        }

        public int GetTileSize(string sheetNum)
        {
            TileIndexNode node;
            if (Nodes.TryGetValue(sheetNum, out node))
                return node.TileSize;
            else
                return 0;
        }


        public static TileGuide Load(BinaryReader reader)
        {
            TileGuide fullGuide = new TileGuide();
            int count = reader.ReadInt32();
            for (int ii = 0; ii < count; ii++)
            {
                string id = reader.ReadString();
                TileIndexNode node = TileIndexNode.Load(reader);
                fullGuide.Nodes[id] = node;
            }
            return fullGuide;
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Nodes.Count);
            foreach (string key in Nodes.Keys)
            {
                writer.Write(key);
                Nodes[key].Save(writer);
            }
        }
    }

    public struct TileAddr
    {
        public string Sheet;
        public long Addr;

        public TileAddr(long addr, string sheet)
        {
            Addr = addr;
            Sheet = sheet;
        }


        public override bool Equals(object obj)
        {
            return (obj is TileAddr) && Equals((TileAddr)obj);
        }

        public bool Equals(TileAddr other)
        {
            return (Addr == other.Addr && Sheet == other.Sheet);
        }

        public override int GetHashCode()
        {
            return Addr.GetHashCode() ^ Sheet.GetHashCode();
        }

        public static bool operator ==(TileAddr value1, TileAddr value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(TileAddr value1, TileAddr value2)
        {
            return !(value1 == value2);
        }

    }

}
