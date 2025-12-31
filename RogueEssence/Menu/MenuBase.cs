using System.Collections.Generic;
using RogueElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueEssence.Content;

namespace RogueEssence.Menu
{
    /// <summary>
    /// Base class for all menu windows in the game. Provides fundamental menu functionality including
    /// rendering, bounds management, element containment, and label-based element lookup.
    /// </summary>
    public abstract class MenuBase : ILabeled
    {
        /// <summary>
        /// The vertical spacing between menu items in pixels.
        /// </summary>
        public const int VERT_SPACE = 14;

        /// <summary>
        /// The height of a single line of text in pixels.
        /// </summary>
        public const int LINE_HEIGHT = 12;

        // Standard text colors for menu rendering.
        // System colors: White (#FFFFFF), Yellow (#FFFF00), Red (#FF0000), Cyan (#00FFFF), Lime (#00FF00)

        /// <summary>Blue text color (#8484FF) for special menu text.</summary>
        public static readonly Color TextBlue = new Color(132, 132, 255); // #8484FF

        /// <summary>Indigo text color (#009CFF) for special menu text.</summary>
        public static readonly Color TextIndigo = new Color(0, 156, 255); // #009CFF

        /// <summary>Pink text color (#FFA5FF) for special menu text.</summary>
        public static readonly Color TextPink = new Color(255, 165, 255); // #FFA5FF

        /// <summary>Pale/light red text color (#FFCEFF) for special menu text.</summary>
        public static readonly Color TextPale = new Color(255,206,206); // #FFCEFF

        /// <summary>Tan/orange text color (#FFC663) for special menu text.</summary>
        public static readonly Color TextTan = new Color(255, 198, 99); // #FFC663

        /// <summary>
        /// Gets or sets the unique identifier label for this menu.
        /// </summary>
        public virtual string Label { get; protected set; }

        /// <summary>
        /// The rectangular bounds defining the menu's position and size on screen.
        /// </summary>
        public Rect Bounds;

        /// <summary>
        /// Gets or sets a value indicating whether this menu is visible.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Global setting that controls whether menu backgrounds are rendered with transparency.
        /// </summary>
        public static bool Transparent;

        /// <summary>
        /// Global setting for the current border style index.
        /// </summary>
        public static int BorderStyle;

        /// <summary>
        /// Global setting for the current border flash animation frame.
        /// </summary>
        public static int BorderFlash;

        /// <inheritdoc/>
        public bool HasLabel()
        {
            return !string.IsNullOrEmpty(Label);
        }

        /// <inheritdoc/>
        public bool LabelContains(string substr)
        {
            return HasLabel() && Label.Contains(substr);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuBase"/> class with default settings.
        /// </summary>
        public MenuBase()
        {
            Label = "";
            Visible = true;

            elements = new List<IMenuElement>();
        }

        // TODO: set to private when deprecated setters are removed.
        /// <summary>
        /// The backing field for menu elements.
        /// </summary>
        protected List<IMenuElement> elements;

        /// <summary>
        /// Gets the collection of menu elements contained in this menu.
        /// </summary>
        public virtual List<IMenuElement> Elements { get { return elements; } }


        /// <summary>
        /// Returns an iterator of all elements for the purpose of drawing.
        /// </summary>
        /// <returns>An enumerable collection of menu elements to be drawn.</returns>
        protected virtual IEnumerable<IMenuElement> GetDrawElements()
        {
            foreach (IMenuElement element in Elements)
                yield return element;
        }

        /// <summary>
        /// Draws the menu including its background, border, and all contained elements.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            spriteBatch.End();
            float scale = GraphicsManager.WindowZoom;
            Matrix zoomMatrix = Matrix.CreateScale(new Vector3(scale, scale, 1));
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, GraphicsManager.MenuPreStencil, null, GraphicsManager.MenuAlpha);

            TileSheet menuBack = GraphicsManager.MenuBG;
            TileSheet menuBorder = GraphicsManager.MenuBorder;

            int addTrans = Transparent ? 3 : 0;

            DrawMenuPiece(spriteBatch, menuBack, Color.White, 0, addTrans);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, GraphicsManager.MenuPostStencil, null, null, zoomMatrix);

            //draw Texts
            foreach (IMenuElement element in GetDrawElements())
                element.Draw(spriteBatch, Bounds.Start);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, zoomMatrix);

            int addX = 3 * BorderStyle;
            int addY = 3 * BorderFlash;

            DrawMenuPiece(spriteBatch, menuBorder, Color.White, addX, addY);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, zoomMatrix);
        }

        /// <summary>
        /// Determines if the given screen location is within this menu's bounds and returns
        /// the relative position within the menu.
        /// </summary>
        /// <param name="screenLoc">The screen location to test.</param>
        /// <param name="menu">When this method returns, contains the menu at the location, or null if not found.</param>
        /// <param name="relativeLoc">When this method returns, contains the position relative to the menu's origin, or null.</param>
        /// <returns>True if the location is within the menu bounds; otherwise, false.</returns>
        public virtual bool GetRelativeMouseLoc(Loc screenLoc, out MenuBase menu, out Loc? relativeLoc)
        {
            menu = null;
            relativeLoc = null;

            if (!Visible)
                return false;

            screenLoc /= GraphicsManager.WindowZoom;

            if (Bounds.Contains(screenLoc))
            {
                menu = this;
                relativeLoc = screenLoc - Bounds.Start;
                return true;
            }

            return false;
        }

        private void DrawMenuPiece(SpriteBatch spriteBatch, TileSheet menu, Color color, int addX, int addY)
        {
            //draw background
            //top-left
            menu.DrawTile(spriteBatch, new Vector2(Bounds.X, Bounds.Y), addX, addY, color);
            //top-right
            menu.DrawTile(spriteBatch, new Vector2(Bounds.End.X - menu.TileWidth, Bounds.Y), addX + 2, addY, color);
            //bottom-right
            menu.DrawTile(spriteBatch, new Vector2(Bounds.End.X - menu.TileWidth, Bounds.End.Y - menu.TileHeight), addX + 2, addY + 2, color);
            //bottom-left
            menu.DrawTile(spriteBatch, new Vector2(Bounds.X, Bounds.End.Y - menu.TileHeight), addX, addY + 2, color);

            //top
            menu.DrawTile(spriteBatch, new Rectangle(Bounds.X + menu.TileWidth, Bounds.Y, Bounds.End.X - Bounds.X - 2 * menu.TileWidth, menu.TileHeight), addX + 1, addY, color);

            //right
            menu.DrawTile(spriteBatch, new Rectangle(Bounds.End.X - menu.TileWidth, Bounds.Y + menu.TileHeight, menu.TileWidth, Bounds.End.Y - Bounds.Y - 2 * menu.TileHeight), addX + 2, addY + 1, color);

            //bottom
            menu.DrawTile(spriteBatch, new Rectangle(Bounds.X + menu.TileWidth, Bounds.End.Y - menu.TileHeight, Bounds.End.X - Bounds.X - 2 * menu.TileWidth, menu.TileHeight), addX + 1, addY + 2, color);

            //left
            menu.DrawTile(spriteBatch, new Rectangle(Bounds.X, Bounds.Y + menu.TileHeight, menu.TileWidth, Bounds.End.Y - Bounds.Y - 2 * menu.TileHeight), addX, addY + 1, color);

            //center
            menu.DrawTile(spriteBatch, new Rectangle(Bounds.X + menu.TileWidth, Bounds.Y + menu.TileHeight, Bounds.End.X - Bounds.X - 2 * menu.TileWidth, Bounds.End.Y - Bounds.Y - 2 * menu.TileHeight), addX + 1, addY + 1, color);
        }


        /// <summary>
        /// Gets the index of an element by its label.
        /// </summary>
        /// <param name="label">The label to search for.</param>
        /// <returns>The index of the element with the specified label, or -1 if not found.</returns>
        public int GetElementIndexByLabel(string label)
        {
            return GetElementIndicesByLabel(label)[label];
        }

        /// <summary>
        /// Gets the indices of multiple elements by their labels.
        /// </summary>
        /// <param name="labels">The labels to search for.</param>
        /// <returns>A dictionary mapping labels to their element indices, with -1 for labels not found.</returns>
        public virtual Dictionary<string, int> GetElementIndicesByLabel(params string[] labels)
        {
            return SearchLabels(labels, Elements);
        }

        /// <summary>
        /// Searches a collection of labeled items and returns a mapping of labels to indices.
        /// </summary>
        /// <param name="labels">The labels to search for.</param>
        /// <param name="list">The collection to search within.</param>
        /// <returns>A dictionary mapping labels to their indices, with -1 for labels not found.</returns>
        public static Dictionary<string, int> SearchLabels(string[] labels, IEnumerable<ILabeled> list)
        {
            Dictionary<string, int> indices = new Dictionary<string, int>();
            int totalFound = 0;
            int ii = 0;
            foreach (string label in labels)
                indices.Add(label, -1);

            foreach (ILabeled element in list)
            {
                int curIndex;
                if (element.HasLabel() && indices.TryGetValue(element.Label, out curIndex))
                {
                    // case for duplicate labels somehow; only get the first index found
                    if (curIndex == -1)
                    {
                        indices[element.Label] = ii;
                        totalFound++;

                        // short-circuit case for having found all indices
                        if (totalFound == indices.Count)
                            return indices;
                    }
                }
                ii++;
            }
            return indices;
        }
    }
}
