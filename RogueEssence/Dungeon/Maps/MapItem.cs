using System;
using RogueElements;
using RogueEssence.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using RogueEssence.Data;
using Newtonsoft.Json;
using RogueEssence.Dev;
using System.Runtime.Serialization;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents an item on the dungeon map floor, including money and regular items.
    /// Supports drawing, spawning, and conversion to inventory items.
    /// </summary>
    [Serializable]
    public class MapItem : IDrawableSprite, ISpawnable, IPreviewable
    {
        /// <summary>
        /// Whether this item represents money rather than a regular item.
        /// </summary>
        public bool IsMoney;

        /// <summary>
        /// Whether this item is cursed.
        /// </summary>
        public bool Cursed;

        /// <summary>
        /// The item ID, or empty for money.
        /// </summary>
        [JsonConverter(typeof(ItemConverter))]
        public string Value;

        /// <summary>
        /// A hidden value for box items containing other items.
        /// </summary>
        public string HiddenValue;

        /// <summary>
        /// The quantity for stackable items or money amount.
        /// </summary>
        public int Amount;

        /// <summary>
        /// The shop price, or 0 if not a shop item.
        /// </summary>
        public int Price;

        /// <summary>
        /// Gets the sprite index for rendering this item.
        /// </summary>
        public string SpriteIndex
        {
            get
            {
                if (IsMoney)
                    return GraphicsManager.MoneySprite;
                else
                    return DataManager.Instance.GetItem(Value).Sprite;
            }
        }

        /// <summary>
        /// The tile location of this item on the map.
        /// </summary>
        public Loc TileLoc;

        /// <summary>
        /// Gets the pixel location on the map.
        /// </summary>
        public Loc MapLoc { get { return TileLoc * GraphicsManager.TileSize; } }

        /// <summary>
        /// Gets the height offset for rendering. Always 0 for items.
        /// </summary>
        public int LocHeight { get { return 0; } }

        /// <summary>
        /// Initializes a new empty MapItem.
        /// </summary>
        public MapItem()
        {
            Value = "";
            HiddenValue = "";
            TileLoc = new Loc();
        }

        /// <summary>
        /// Initializes a new MapItem with the specified item ID.
        /// </summary>
        /// <param name="value">The item ID.</param>
        public MapItem(string value)
        {
            Value = value;
            HiddenValue = "";
        }

        /// <summary>
        /// Initializes a new MapItem with the specified item ID and amount.
        /// </summary>
        /// <param name="value">The item ID.</param>
        /// <param name="amount">The quantity.</param>
        public MapItem(string value, int amount)
            : this(value)
        {
            Amount = amount;
        }

        /// <summary>
        /// Initializes a new MapItem with item ID, amount, and price.
        /// </summary>
        /// <param name="value">The item ID.</param>
        /// <param name="amount">The quantity.</param>
        /// <param name="price">The shop price.</param>
        public MapItem(string value, int amount, int price)
            : this(value, amount)
        {
            Price = price;
        }

        /// <summary>
        /// Creates a copy of another MapItem.
        /// </summary>
        /// <param name="other">The MapItem to copy.</param>
        public MapItem(MapItem other)
        {
            IsMoney = other.IsMoney;
            Cursed = other.Cursed;
            Value = other.Value;
            HiddenValue = other.HiddenValue;
            Amount = other.Amount;
            Price = other.Price;
            TileLoc = other.TileLoc;
        }

        /// <summary>
        /// Creates a spawnable copy of this item.
        /// </summary>
        /// <returns>A new MapItem copy.</returns>
        public ISpawnable Copy() { return new MapItem(this); }

        /// <summary>
        /// Initializes a MapItem from an inventory item at the origin.
        /// </summary>
        /// <param name="item">The inventory item to convert.</param>
        public MapItem(InvItem item) : this(item, new Loc()) { }

        /// <summary>
        /// Initializes a MapItem from an inventory item at the specified location.
        /// </summary>
        /// <param name="item">The inventory item to convert.</param>
        /// <param name="loc">The tile location.</param>
        public MapItem(InvItem item, Loc loc)
        {
            Value = item.ID;
            Cursed = item.Cursed;
            Amount = item.Amount;
            HiddenValue = item.HiddenValue;
            Price = item.Price;
            TileLoc = loc;
        }

        /// <summary>
        /// Converts this map item to an inventory item.
        /// </summary>
        /// <returns>An InvItem with the same properties.</returns>
        public InvItem MakeInvItem()
        {
            InvItem item = new InvItem(Value, Cursed);
            item.Amount = Amount;
            item.HiddenValue = HiddenValue;
            item.Price = Price;
            return item;
        }

        /// <summary>
        /// Creates a money item with the specified amount.
        /// </summary>
        /// <param name="amt">The money amount.</param>
        /// <returns>A new MapItem representing money.</returns>
        public static MapItem CreateMoney(int amt)
        {
            MapItem item = new MapItem();
            item.IsMoney = true;
            item.Amount = amt;
            return item;
        }

        /// <summary>
        /// Creates a box item containing another item.
        /// </summary>
        /// <param name="value">The box item ID.</param>
        /// <param name="hiddenValue">The contained item ID.</param>
        /// <param name="price">The shop price.</param>
        /// <param name="cursed">Whether the item is cursed.</param>
        /// <returns>A new MapItem representing a box.</returns>
        public static MapItem CreateBox(string value, string hiddenValue, int price = 0, bool cursed = false)
        {
            MapItem item = new MapItem();
            item.Value = value;
            item.HiddenValue = hiddenValue;
            item.Cursed = cursed;
            item.Price = price;
            return item;
        }

        /// <summary>
        /// Gets the sell value of this item.
        /// </summary>
        /// <returns>The item's base price multiplied by amount, or 0 for money.</returns>
        public int GetSellValue()
        {
            if (IsMoney)
                return 0;

            ItemData entry = DataManager.Instance.GetItem(Value);
            if (entry.MaxStack > 1)
                return entry.Price * Amount;
            else
                return entry.Price;
        }

        /// <summary>
        /// Gets the price string formatted with special characters for display.
        /// </summary>
        /// <returns>The formatted price string, or null if no price.</returns>
        public string GetPriceString()
        {
            if (Price > 0)
            {
                string baseStr = Price.ToString();
                StringBuilder resultStr = new StringBuilder();
                for (int ii = 0; ii < baseStr.Length; ii++)
                {
                    int en = (int)baseStr[ii] - 0x30;
                    int un = en + 0xE100;
                    resultStr.Append((char)un);
                }
                return resultStr.ToString();
            }
            return null;
        }

        /// <summary>
        /// Gets a price string formatted with special characters for display.
        /// </summary>
        /// <param name="price">The price value.</param>
        /// <returns>The formatted price string, or empty if no price.</returns>
        public static string GetPriceString(int price)
        {
            if (price > 0)
            {
                string baseStr = price.ToString();
                StringBuilder resultStr = new StringBuilder();
                for (int ii = 0; ii < baseStr.Length; ii++)
                {
                    int en = (int)baseStr[ii] - 0x30;
                    int un = en + 0xE100;
                    resultStr.Append((char)un);
                }
                return resultStr.ToString();
            }
            return "";
        }

        /// <summary>
        /// Gets the display name for this item in the dungeon.
        /// </summary>
        /// <returns>The formatted item name with colors and icons.</returns>
        public string GetDungeonName()
        {
            if (IsMoney)
                return Text.FormatKey("MONEY_AMOUNT", Amount);
            else
            {
                EntryDataIndex idx = DataManager.Instance.DataIndices[DataManager.DataType.Item];
                if (!idx.ContainsKey(Value))
                    return String.Format("[color=#FF0000]{0}[color]", Value);

                ItemEntrySummary summary = (ItemEntrySummary)idx.Get(Value);

                string prefix = "";
                if (summary.Icon > -1)
                    prefix += ((char)(summary.Icon + 0xE0A0)).ToString();
                if (Cursed)
                    prefix += "\uE10B";

                string nameStr = summary.Name.ToLocal();
                if (summary.MaxStack > 1)
                    nameStr += " (" + Amount + ")";

                if (summary.UsageType == ItemData.UseType.Treasure)
                    return String.Format("{0}[color=#6384E6]{1}[color]", prefix, nameStr);
                else
                    return String.Format("{0}[color=#FFCEFF]{1}[color]", prefix, nameStr);
            }
        }

        /// <summary>
        /// Returns a string representation of this map item.
        /// </summary>
        /// <returns>A string describing the item, price, and properties.</returns>
        public override string ToString()
        {
            if (IsMoney)
                return String.Format("${0}", Amount);

            string nameStr = "";
            if (Price > 0)
                nameStr += String.Format("${0} ", Price);
            if (Cursed)
                nameStr += "[X]";

            nameStr += Value;
            if (Amount > 0)
                nameStr += String.Format("({0})", Amount);

            if (!String.IsNullOrEmpty(HiddenValue))
                nameStr += String.Format("[{0}]", HiddenValue);

            return nameStr;
        }

        /// <summary>
        /// Draws debug information. This implementation does nothing.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="offset">The camera offset.</param>
        public void DrawDebug(SpriteBatch spriteBatch, Loc offset) { }

        /// <summary>
        /// Draws the item sprite at its location with white color.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="offset">The camera offset.</param>
        public void Draw(SpriteBatch spriteBatch, Loc offset)
        {
            Draw(spriteBatch, offset, Color.White);
        }

        /// <summary>
        /// Draws a preview of the item with the specified alpha.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="offset">The camera offset.</param>
        /// <param name="alpha">The alpha transparency value.</param>
        public void DrawPreview(SpriteBatch spriteBatch, Loc offset, float alpha)
        {
            Draw(spriteBatch, offset, Color.White * alpha);
        }

        /// <summary>
        /// Draws the item sprite at its location with the specified color tint.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="offset">The camera offset.</param>
        /// <param name="color">The color tint to apply.</param>
        public void Draw(SpriteBatch spriteBatch, Loc offset, Color color)
        {
            Loc drawLoc = GetDrawLoc(offset);

            DirSheet sheet = GraphicsManager.GetItem(SpriteIndex);
            sheet.DrawDir(spriteBatch, new Vector2(drawLoc.X, drawLoc.Y), 0, Dir8.Down, color);
        }

        /// <summary>
        /// Gets the drawing location accounting for centering and camera offset.
        /// </summary>
        /// <param name="offset">The camera offset.</param>
        /// <returns>The screen position to draw the item at.</returns>
        public Loc GetDrawLoc(Loc offset)
        {
            return new Loc(MapLoc.X + GraphicsManager.TileSize / 2 - GraphicsManager.GetItem(SpriteIndex).TileWidth / 2,
                MapLoc.Y + GraphicsManager.TileSize / 2 - GraphicsManager.GetItem(SpriteIndex).TileHeight / 2) - offset;
        }

        /// <summary>
        /// Gets the sheet offset. Always returns zero for items.
        /// </summary>
        /// <returns>A zero location.</returns>
        public Loc GetSheetOffset() { return Loc.Zero; }

        /// <summary>
        /// Gets the size of the item sprite.
        /// </summary>
        /// <returns>The dimensions of the item sprite.</returns>
        public Loc GetDrawSize()
        {
            return new Loc(GraphicsManager.GetItem(SpriteIndex).TileWidth,
                GraphicsManager.GetItem(SpriteIndex).TileHeight);
        }
    }
}
