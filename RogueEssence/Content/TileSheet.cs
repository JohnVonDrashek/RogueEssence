using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace RogueEssence.Content
{
    /// <summary>
    /// A sprite sheet organized as a uniform grid of equally-sized tiles.
    /// Used for tileset graphics and grid-based sprite animations.
    /// </summary>
    public class TileSheet : BaseSheet
    {
        /// <summary>
        /// Gets the total number of tiles in the sheet.
        /// </summary>
        public int TotalTiles { get { return TotalX * TotalY; } }

        /// <summary>
        /// Gets the number of tile columns in the sheet.
        /// </summary>
        public int TotalX { get; protected set; }

        /// <summary>
        /// Gets the number of tile rows in the sheet.
        /// </summary>
        public int TotalY { get; protected set; }

        /// <summary>
        /// Gets the width of each tile in pixels.
        /// </summary>
        public int TileWidth { get; protected set; }

        /// <summary>
        /// Gets the height of each tile in pixels.
        /// </summary>
        public int TileHeight { get; protected set; }

        /// <summary>
        /// Creates a new TileSheet from an existing texture.
        /// </summary>
        /// <param name="tex">The source texture.</param>
        /// <param name="tileWidth">The width of each tile in pixels.</param>
        /// <param name="tileHeight">The height of each tile in pixels.</param>
        /// <exception cref="ArgumentException">Thrown if texture dimensions are not evenly divisible by tile dimensions.</exception>
        protected TileSheet(Texture2D tex, int tileWidth, int tileHeight)
            : base(tex)
        {
            if (Width % tileWidth != 0 || Height % tileHeight != 0)
                throw new ArgumentException(String.Format("Texture dimensions ({0},{1}) cannot be divided by ({2},{3})", Width, Height, tileWidth, tileHeight));

            TotalX = Width / tileWidth;
            TotalY = Height / tileHeight;
            TileWidth = tileWidth;
            TileHeight = tileHeight;

        }

        /// <summary>
        /// Imports a tile sheet from a PNG file with tile dimensions in the filename.
        /// Filename format: name-tileWidth-tileHeight.png
        /// </summary>
        /// <param name="path">The path to the PNG file.</param>
        /// <returns>A new TileSheet imported from the file.</returns>
        public static new TileSheet Import(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            string[] components = fileName.Split('-');
            int tileWidth = Convert.ToInt32(components[components.Length-2]);
            int tileHeight = Convert.ToInt32(components[components.Length-1]);
            return TileSheet.Import(path, tileWidth, tileHeight);
        }

        /// <summary>
        /// Imports a tile sheet from a PNG file with explicitly specified tile dimensions.
        /// </summary>
        /// <param name="path">The path to the PNG file.</param>
        /// <param name="tileWidth">The width of each tile in pixels.</param>
        /// <param name="tileHeight">The height of each tile in pixels.</param>
        /// <returns>A new TileSheet imported from the file.</returns>
        public static TileSheet Import(string path, int tileWidth, int tileHeight)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Texture2D tex = ImportTex(fileStream);
                return new TileSheet(tex, tileWidth, tileHeight);
            }
        }

        /// <summary>
        /// Exports a tile sheet to a PNG file.
        /// </summary>
        /// <param name="sheet">The tile sheet to export.</param>
        /// <param name="filepath">The output file path.</param>
        public static void Export(TileSheet sheet, string filepath)
        {
            using (Stream stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.None))
                ExportTex(stream, sheet.baseTexture);
        }

        /// <summary>
        /// Loads a tile sheet from a binary stream.
        /// </summary>
        /// <param name="reader">The binary reader to read from.</param>
        /// <returns>A new TileSheet loaded from the stream.</returns>
        public static new TileSheet Load(BinaryReader reader)
        {
            long length = reader.ReadInt64();
            Texture2D tex = null;
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(reader.ReadBytes((int)length), 0, (int)length);
                ms.Position = 0;
                tex = Texture2D.FromStream(device, ms);
            }

            int tileWidth = reader.ReadInt32();
            int tileHeight = reader.ReadInt32();
            return new TileSheet(tex, tileWidth, tileHeight);
        }

        /// <summary>
        /// Creates a fallback tile sheet using the default error texture.
        /// </summary>
        /// <returns>A TileSheet containing the default fallback texture.</returns>
        public static new TileSheet LoadError()
        {
            return new TileSheet(defaultTex, defaultTex.Width, defaultTex.Height);
        }

        /// <summary>
        /// Saves the tile sheet to a binary stream.
        /// </summary>
        /// <param name="writer">The binary writer to write to.</param>
        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);
            writer.Write(TileWidth);
            writer.Write(TileHeight);
        }


        /// <summary>
        /// Draws a tile at the specified position with white color.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="x">The tile column index.</param>
        /// <param name="y">The tile row index.</param>
        public void DrawTile(SpriteBatch spriteBatch, Vector2 pos, int x, int y)
        {
            DrawTile(spriteBatch, pos, x, y, Color.White);
        }

        /// <summary>
        /// Draws a tile at the specified position with a color tint.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="x">The tile column index.</param>
        /// <param name="y">The tile row index.</param>
        /// <param name="color">The color tint to apply.</param>
        public void DrawTile(SpriteBatch spriteBatch, Vector2 pos, int x, int y, Color color)
        {
            DrawTile(spriteBatch, pos, x, y, color, SpriteEffects.None);
        }

        /// <summary>
        /// Draws a tile with rotation.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="x">The tile column index.</param>
        /// <param name="y">The tile row index.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="rotation">The rotation angle in radians.</param>
        public void DrawTile(SpriteBatch spriteBatch, Vector2 pos, int x, int y, Color color, float rotation)
        {
            DrawTile(spriteBatch, pos, x, y, color, 1f, rotation);
        }

        /// <summary>
        /// Draws a tile with rotation and sprite effects.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="x">The tile column index.</param>
        /// <param name="y">The tile row index.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="rotation">The rotation angle in radians.</param>
        /// <param name="spriteEffects">The sprite effects to apply.</param>
        public void DrawTile(SpriteBatch spriteBatch, Vector2 pos, int x, int y, Color color, float rotation, SpriteEffects spriteEffects)
        {
            DrawTile(spriteBatch, pos, x, y, color, Vector2.One, rotation, spriteEffects);
        }

        /// <summary>
        /// Draws a tile with uniform scale and rotation.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="x">The tile column index.</param>
        /// <param name="y">The tile row index.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="scale">The uniform scale factor.</param>
        /// <param name="rotation">The rotation angle in radians.</param>
        public void DrawTile(SpriteBatch spriteBatch, Vector2 pos, int x, int y, Color color, float scale, float rotation)
        {
            DrawTile(spriteBatch, pos, x, y, color, new Vector2(scale), rotation);
        }

        /// <summary>
        /// Draws a tile with non-uniform scale and rotation.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="x">The tile column index.</param>
        /// <param name="y">The tile row index.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="scale">The scale factor.</param>
        /// <param name="rotation">The rotation angle in radians.</param>
        public void DrawTile(SpriteBatch spriteBatch, Vector2 pos, int x, int y, Color color, Vector2 scale, float rotation)
        {
            DrawTile(spriteBatch, pos, x, y, color, scale, rotation, SpriteEffects.None);
        }

        /// <summary>
        /// Draws a tile with full control over scale, rotation, and sprite effects.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="x">The tile column index.</param>
        /// <param name="y">The tile row index.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="scale">The scale factor.</param>
        /// <param name="rotation">The rotation angle in radians.</param>
        /// <param name="spriteEffects">The sprite effects to apply.</param>
        public void DrawTile(SpriteBatch spriteBatch, Vector2 pos, int x, int y, Color color, Vector2 scale, float rotation, SpriteEffects spriteEffects)
        {
            if (x < TotalX && y < TotalY)
                Draw(spriteBatch, pos, new Rectangle(TileWidth * x, TileHeight * y, TileWidth, TileHeight), color, scale, rotation, spriteEffects);
            else
                DrawDefault(spriteBatch, new Rectangle((int)pos.X, (int)pos.Y, TileWidth, TileHeight));
        }

        /// <summary>
        /// Draws a tile with a custom origin point for rotation.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="x">The tile column index.</param>
        /// <param name="y">The tile row index.</param>
        /// <param name="origin">The origin point for rotation.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="scale">The scale factor.</param>
        /// <param name="rotation">The rotation angle in radians.</param>
        public void DrawTile(SpriteBatch spriteBatch, Vector2 pos, int x, int y, Vector2 origin, Color color, Vector2 scale, float rotation)
        {
            if (x < TotalX && y < TotalY)
                Draw(spriteBatch, pos, new Rectangle(TileWidth * x, TileHeight * y, TileWidth, TileHeight), origin, color, scale, rotation);
            else
                DrawDefault(spriteBatch, new Rectangle((int)pos.X, (int)pos.Y, TileWidth, TileHeight));
        }

        /// <summary>
        /// Draws a tile with sprite effects (flip).
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="x">The tile column index.</param>
        /// <param name="y">The tile row index.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="effects">The sprite effects to apply.</param>
        public void DrawTile(SpriteBatch spriteBatch, Vector2 pos, int x, int y, Color color, SpriteEffects effects)
        {
            if (x < TotalX && y < TotalY)
                Draw(spriteBatch, pos, new Rectangle(TileWidth * x, TileHeight * y, TileWidth, TileHeight), color, new Vector2(1), effects);
            else
                DrawDefault(spriteBatch, new Rectangle((int)pos.X, (int)pos.Y, TileWidth, TileHeight));
        }

        /// <summary>
        /// Draws a tile stretched to fit a destination rectangle.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="destRect">The destination rectangle to fill.</param>
        /// <param name="x">The tile column index.</param>
        /// <param name="y">The tile row index.</param>
        /// <param name="color">The color tint to apply.</param>
        public void DrawTile(SpriteBatch spriteBatch, Rectangle destRect, int x, int y, Color color)
        {
            if (x < TotalX && y < TotalY)
                Draw(spriteBatch, destRect, new Rectangle(TileWidth * x, TileHeight * y, TileWidth, TileHeight), color);
            else
                DrawDefault(spriteBatch, destRect);
        }

        /// <summary>
        /// Replaces the tile sheet texture with new tile dimensions.
        /// </summary>
        /// <param name="tex">The new texture.</param>
        /// <param name="tileWidth">The new tile width.</param>
        /// <param name="tileHeight">The new tile height.</param>
        public void SetTileTexture(Texture2D tex, int tileWidth, int tileHeight)
        {
            base.SetTexture(tex);
            TotalX = Width / tileWidth;
            TotalY = Height / tileHeight;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
        }

    }
}
