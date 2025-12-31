using System;
using RogueElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace RogueEssence.Content
{
    /// <summary>
    /// Interface for animations that have a finite number of frames and can be disposed.
    /// </summary>
    public interface IEffectAnim : IDisposable
    {
        /// <summary>
        /// Gets the total number of frames in the animation.
        /// </summary>
        int TotalFrames { get; }
    }

    /// <summary>
    /// A tile sheet that supports directional sprites with automatic rotation or mirroring.
    /// Used for effects and animations that need to face different directions.
    /// </summary>
    public class DirSheet : TileSheet, IEffectAnim
    {
        /// <summary>
        /// Defines how sprites are rotated or mirrored to achieve different directions.
        /// </summary>
        public enum RotateType
        {
            /// <summary>No directional variation.</summary>
            None,
            /// <summary>Rotate the sprite programmatically to all 8 angles.</summary>
            Dir1,
            /// <summary>Two base sprites, rotated to achieve all directions.</summary>
            Dir2,
            /// <summary>Five directional sprites with horizontal mirroring for the other three.</summary>
            Dir5,
            /// <summary>Full 8-directional sprites with no mirroring.</summary>
            Dir8,
            /// <summary>Two directions using horizontal flip.</summary>
            Flip,
        }

        /// <summary>
        /// Gets the total number of animation frames.
        /// </summary>
        public int TotalFrames { get; protected set; }

        /// <summary>
        /// Gets the rotation type used for directional rendering.
        /// </summary>
        public RotateType Dirs { get; protected set; }

        //public DirSheet(int width, int height, int tileWidth, int tileHeight, RotateType dirs)
        //    : base(width, height, tileWidth, tileHeight)
        //{
        //    Dirs = dirs;
        //}

        /// <summary>
        /// Creates a new DirSheet from an existing texture.
        /// </summary>
        /// <param name="tex">The source texture.</param>
        /// <param name="tileWidth">The width of each tile in pixels.</param>
        /// <param name="tileHeight">The height of each tile in pixels.</param>
        /// <param name="totalFrames">The total number of animation frames.</param>
        /// <param name="dirs">The rotation type for directional rendering.</param>
        protected DirSheet(Texture2D tex, int tileWidth, int tileHeight, int totalFrames, RotateType dirs)
            : base(tex, tileWidth, tileHeight)
        {
            Dirs = dirs;
            TotalFrames = totalFrames;
        }

        /// <summary>
        /// Imports a directional sprite sheet from a file or directory.
        /// Supports directory import with DirData.xml and numbered frame PNGs,
        /// or single PNG files with frame count or direction type in the filename.
        /// </summary>
        /// <param name="path">The path to the file or directory to import.</param>
        /// <returns>A new DirSheet imported from the specified path.</returns>
        public static new DirSheet Import(string path)
        {
            if (Directory.Exists(path))//assume directory structure
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Path.Combine(path, "DirData.xml"));
                RotateType totalDirs = Enum.Parse<RotateType>(doc.SelectSingleNode("DirData/DirType").InnerText);

                List<(Color[] tex, int width, int height)> frames = new List<(Color[], int, int)>();
                while (true)
                {
                    string pngFile = Path.Combine(path, frames.Count.ToString() + ".png");
                    if (!File.Exists(pngFile))
                        break;

                    using (FileStream fileStream = new FileStream(pngFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (Texture2D tex = ImportTex(fileStream))
                            frames.Add((BaseSheet.GetData(tex), tex.Width, tex.Height));
                    }

                    //dimensions consistency check
                    if (frames.Count > 1)
                    {
                        if (frames[0].width != frames[frames.Count - 1].width && frames[0].height != frames[frames.Count - 1].height)
                            throw new Exception(String.Format("Frame {0} does not match previous frame's dimensions!", frames.Count-1));
                    }
                }

                int totalPixels = frames.Count * frames[0].width * frames[0].height;
                int tilesX = (int)Math.Ceiling(Math.Sqrt(totalPixels) / frames[0].width);
                int tilesY = MathUtils.DivUp(frames.Count, tilesX);

                int tileWidth = frames[0].width;
                int tileHeight = frames[0].height;
                int dirHeight = tileHeight / getDirDiv(totalDirs);
                int maxWidth = tilesX * tileWidth;
                int maxHeight = tilesY * tileHeight;

                Color[] texColors = new Color[maxWidth * maxHeight];

                for (int ii = 0; ii < frames.Count; ii++)
                {
                    int xx = ii % tilesX;
                    int yy = ii / tilesX;
                    BaseSheet.Blit(frames[ii].tex, texColors, new Point(tileWidth, tileHeight), new Point(maxWidth, maxHeight), new Point(tileWidth * xx, tileHeight * yy), SpriteEffects.None);
                }

                Texture2D full = new Texture2D(device, maxWidth, maxHeight);
                full.SetData<Color>(0, null, texColors, 0, texColors.Length);
                return new DirSheet(full, tileWidth, dirHeight, frames.Count, totalDirs);
            }
            else //assume png file
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                string[] components = fileName.Split('.');
                string typeString = components[components.Length - 1];
                int framesW = 0;
                if (Int32.TryParse(typeString, out framesW))
                {
                    using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        Texture2D tex = ImportTex(fileStream);
                        return new DirSheet(tex, tex.Width / framesW, tex.Height, framesW, RotateType.None);
                    }
                }
                string[] dims = typeString.Split("x");
                int framesH = 0;
                if (dims.Length == 2 && Int32.TryParse(dims[0], out framesW) && Int32.TryParse(dims[1], out framesH))
                {
                    using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        Texture2D tex = ImportTex(fileStream);
                        return new DirSheet(tex, tex.Width / framesW, tex.Height / framesH, framesW * framesH, RotateType.None);
                    }
                }


                RotateType dirs = Enum.Parse<RotateType>(typeString);
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    Texture2D tex = ImportTex(fileStream);
                    int tileWidth = tex.Height / getDirDiv(dirs);
                    return new DirSheet(tex, tileWidth, tileWidth, tex.Width / tileWidth, dirs);
                }
            }
            throw new Exception("Unsupported file extension.");
        }

        /// <summary>
        /// Gets the number of rows in the sprite sheet for a given rotation type.
        /// </summary>
        /// <param name="dirs">The rotation type.</param>
        /// <returns>The number of directional rows (1, 2, 5, or 8).</returns>
        public static int getDirDiv(RotateType dirs)
        {
            int div;
            switch (dirs)
            {
                case RotateType.Dir2:
                    div = 2;
                    break;
                case RotateType.Dir5:
                    div = 5;
                    break;
                case RotateType.Dir8:
                    div = 8;
                    break;
                default:
                    div = 1;
                    break;
            }
            return div;
        }

        /// <summary>
        /// Generates the filename suffix for exporting a DirSheet based on its properties.
        /// </summary>
        /// <param name="sheet">The DirSheet to generate a name for.</param>
        /// <param name="basename">The base name without extension.</param>
        /// <returns>The complete filename with appropriate suffix.</returns>
        public static string GetExportString(DirSheet sheet, string basename)
        {
            string suffix;
            int dirs = getDirDiv(sheet.Dirs);
            if (sheet.TotalY > dirs)//uses the second tier or more
            {
                if (sheet.Dirs == RotateType.None && sheet.TotalFrames % sheet.TotalX == 0)
                    suffix = string.Format("{0}x{1}", sheet.TotalX.ToString(), sheet.TotalY.ToString());
                else
                    suffix = "";
            }
            else
            {
                switch (sheet.Dirs)
                {
                    case RotateType.None:
                        {
                            if (sheet.TileWidth == sheet.TileHeight)//all square frames on one tier
                                suffix = sheet.Dirs.ToString();
                            else//rectangular frames on one tier
                                suffix = sheet.TotalFrames.ToString();
                        }
                        break;
                    default:
                        suffix = sheet.Dirs.ToString();
                        break;
                }
            }
            if (suffix == "")
                return basename;
            return basename + "." + suffix;
        }

        /// <summary>
        /// Exports a DirSheet to a file or directory.
        /// </summary>
        /// <param name="sheet">The DirSheet to export.</param>
        /// <param name="filepath">The output file path.</param>
        /// <param name="singleFrames">If true, exports as separate frame files in a directory.</param>
        public static void Export(DirSheet sheet, string filepath, bool singleFrames)
        {
            if (singleFrames)
            {
                throw new NotImplementedException("Exporting folders not yet done.");
            }
            else
            {
                using (Stream stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.None))
                    ExportTex(stream, sheet.baseTexture);
            }
        }

        /// <summary>
        /// Loads a DirSheet from a binary stream.
        /// </summary>
        /// <param name="reader">The binary reader to read from.</param>
        /// <returns>A new DirSheet loaded from the stream.</returns>
        public static new DirSheet Load(BinaryReader reader)
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
            RotateType dirs = (RotateType)reader.ReadInt32();
            int totalFrames = reader.ReadInt32();
            return new DirSheet(tex, tileWidth, tileHeight, totalFrames, dirs);
            
        }

        /// <summary>
        /// Creates a fallback DirSheet using the default error texture.
        /// </summary>
        /// <returns>A DirSheet containing the default fallback texture.</returns>
        public static new DirSheet LoadError()
        {
            return new DirSheet(defaultTex, defaultTex.Width, defaultTex.Height, 1, RotateType.None);
        }

        /// <summary>
        /// Saves the DirSheet to a binary stream.
        /// </summary>
        /// <param name="writer">The binary writer to write to.</param>
        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);
            writer.Write((int)Dirs);
            writer.Write(TotalFrames);
        }


        /// <summary>
        /// Draws a frame from the directional sheet facing down.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="frame">The frame index to draw.</param>
        public void DrawDir(SpriteBatch spriteBatch, Vector2 pos, int frame)
        {
            DrawDir(spriteBatch, pos, frame, Dir8.Down);
        }

        /// <summary>
        /// Draws a frame from the directional sheet in the specified direction.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="frame">The frame index to draw.</param>
        /// <param name="dir">The direction to face.</param>
        public void DrawDir(SpriteBatch spriteBatch, Vector2 pos, int frame, Dir8 dir)
        {
            DrawDir(spriteBatch, pos, frame, dir, Color.White);
        }

        /// <summary>
        /// Draws a frame from the directional sheet with a color tint.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="frame">The frame index to draw.</param>
        /// <param name="dir">The direction to face.</param>
        /// <param name="color">The color tint to apply.</param>
        public void DrawDir(SpriteBatch spriteBatch, Vector2 pos, int frame, Dir8 dir, Color color)
        {
            DrawDir(spriteBatch, pos, frame, dir, color, SpriteFlip.None);
        }

        /// <summary>
        /// Draws a frame from the directional sheet with full drawing options.
        /// Automatically handles rotation and mirroring based on the sheet's RotateType.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="frame">The frame index to draw.</param>
        /// <param name="dir">The direction to face.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="spriteFlip">Additional sprite flip effects.</param>
        public void DrawDir(SpriteBatch spriteBatch, Vector2 pos, int frame, Dir8 dir, Color color, SpriteFlip spriteFlip)
        {
            if (frame >= TotalFrames)
            {
                DrawDefault(spriteBatch, new Rectangle((int)pos.X, (int)pos.Y, TileWidth, TileHeight));
                return;
            }
            SpriteEffects flip = (SpriteEffects)((int)spriteFlip);
            switch (Dirs)
            {
                case RotateType.None:
                    DrawTile(spriteBatch, pos, frame % TotalX, frame / TotalX, color, flip);
                    break;
                case RotateType.Dir1:
                    DrawTile(spriteBatch, pos + new Vector2(TileWidth / 2, TileHeight / 2), frame % TotalX, frame / TotalX, color, (float)((int)dir * Math.PI / 4), flip);
                    break;
                case RotateType.Dir2:
                    DrawTile(spriteBatch, pos + new Vector2(TileWidth / 2, TileHeight / 2), frame % TotalX, frame / TotalX * 2 + (int)dir % 2, color, (float)(((int)dir / 2) * Math.PI / 2), flip);
                    break;
                case RotateType.Dir5:
                    {
                        int index = (int)dir;
                        if (dir > Dir8.Up)
                        {
                            //flip the sprite for the reverse angles
                            index = 8 - index;
                            flip ^= SpriteEffects.FlipHorizontally;
                        }
                        DrawTile(spriteBatch, pos, frame % TotalX, frame / TotalX * 5 + index, color, flip);
                        break;
                    }
                case RotateType.Dir8:
                    DrawTile(spriteBatch, pos, frame % TotalX, frame / TotalX * 8 + (int)dir, color, flip);
                    break;
                case RotateType.Flip:
                    {
                        if (dir >= Dir8.Up)
                            flip ^= SpriteEffects.FlipHorizontally;
                        DrawTile(spriteBatch, pos, frame % TotalX, frame / TotalX, color, flip);
                        break;
                    }
            }
        }
    }

}
