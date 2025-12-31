using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using RectPacker;

namespace RogueEssence.Content
{
    /// <summary>
    /// A sprite sheet that stores multiple variable-sized sprites in a texture atlas.
    /// Each sprite has its own source rectangle within the texture.
    /// </summary>
    public class SpriteSheet : BaseSheet
    {
        /// <summary>
        /// The source rectangles for each sprite in the atlas.
        /// </summary>
        protected Rectangle[] spriteRects;

        /// <summary>
        /// Creates a new SpriteSheet from a texture and sprite rectangles.
        /// </summary>
        /// <param name="tex">The texture containing all sprites.</param>
        /// <param name="rects">The source rectangles for each sprite.</param>
        protected SpriteSheet(Texture2D tex, params Rectangle[] rects)
            : base(tex)
        {
            spriteRects = rects;
        }

        /// <summary>
        /// Imports a sprite sheet from a directory containing individual PNG files.
        /// Automatically packs them into a texture atlas.
        /// </summary>
        /// <param name="path">The path to the directory containing PNG files.</param>
        /// <returns>A new SpriteSheet with all images packed into an atlas.</returns>
        public static new SpriteSheet Import(string path)
        {
            string[] pngs = Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly);
            List<ImageInfo> sheets = new List<ImageInfo>();
            int index = 0;
            foreach (string dir in pngs)
            {
                Texture2D newSheet = null;
                using (FileStream fileStream = new FileStream(dir, FileMode.Open, FileAccess.Read, FileShare.Read))
                    newSheet = ImportTex(fileStream);

                sheets.Add(new ImageInfo(index, newSheet));
                index++;
            }
            if (sheets.Count == 0)
                return null;

            Canvas canvas = new Canvas();
            canvas.SetCanvasDimensions(Canvas.INFINITE_SIZE, Canvas.INFINITE_SIZE);
            OptimalMapper mapper = new OptimalMapper(canvas);
            Atlas atlas = mapper.Mapping(sheets);

            Rectangle[] rects = new Rectangle[sheets.Count];
            Texture2D tex = new Texture2D(device, atlas.Width, atlas.Height);
            for (int ii = 0; ii < atlas.MappedImages.Count; ii++)
            {
                MappedImageInfo info = atlas.MappedImages[ii];
                BaseSheet.Blit(info.ImageInfo.Texture, tex, 0, 0, info.ImageInfo.Width, info.ImageInfo.Height, info.X, info.Y);
                rects[info.ImageInfo.ID] = new Rectangle(info.X, info.Y, info.ImageInfo.Width, info.ImageInfo.Height);
                info.ImageInfo.Texture.Dispose();
            }

            return new SpriteSheet(tex, rects);
        }

        /// <summary>
        /// Loads a sprite sheet from a binary stream.
        /// </summary>
        /// <param name="reader">The binary reader to read from.</param>
        /// <returns>A new SpriteSheet loaded from the stream.</returns>
        public static new SpriteSheet Load(BinaryReader reader)
        {
            long length = reader.ReadInt64();
            Texture2D tex = null;
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(reader.ReadBytes((int)length), 0, (int)length);
                ms.Position = 0;
                tex = Texture2D.FromStream(device, ms);
            }

            int rectCount = reader.ReadInt32();
            Rectangle[] rects = new Rectangle[rectCount];
            for (int ii = 0; ii < rectCount; ii++)
                rects[ii] = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            return new SpriteSheet(tex, rects);
        }

        /// <summary>
        /// Creates a fallback sprite sheet using the default error texture.
        /// </summary>
        /// <returns>A SpriteSheet containing the default fallback texture.</returns>
        public static new SpriteSheet LoadError()
        {
            return new SpriteSheet(defaultTex, new Rectangle[0] { });
        }

        /// <summary>
        /// Saves the sprite sheet to a binary stream.
        /// </summary>
        /// <param name="writer">The binary writer to write to.</param>
        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);
            writer.Write(spriteRects.Length);
            for (int ii = 0; ii < spriteRects.Length; ii++)
            {
                writer.Write(spriteRects[ii].X);
                writer.Write(spriteRects[ii].Y);
                writer.Write(spriteRects[ii].Width);
                writer.Write(spriteRects[ii].Height);
            }
        }

        /// <summary>
        /// Draws a sprite from the sheet at the specified position.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="index">The index of the sprite to draw.</param>
        /// <param name="color">The color tint to apply.</param>
        public void DrawSprite(SpriteBatch spriteBatch, Vector2 pos, int index, Color color)
        {
            if (index < spriteRects.Length)
                Draw(spriteBatch, pos, spriteRects[index], color);
            else
                DrawDefault(spriteBatch, new Rectangle((int)pos.X, (int)pos.Y, 32, 32));
        }

    }
}
