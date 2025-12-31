using System;
using System.Collections.Generic;
using RogueElements;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueEssence.Content
{
    /// <summary>
    /// A sprite sheet specifically designed for rendering beam-type effects.
    /// Contains separate head, body, and tail frames for creating extensible beam visuals.
    /// </summary>
    public class BeamSheet : SpriteSheet, IEffectAnim
    {
        /// <summary>
        /// Defines the components of a beam animation.
        /// </summary>
        private enum BeamFrame
        {
            Head,
            Body,
            Tail
        }

        /// <summary>
        /// Gets the total number of animation frames in the beam.
        /// </summary>
        public int TotalFrames { get; private set; }

        /// <summary>
        /// Creates a new beam sheet from a texture and rectangle definitions.
        /// </summary>
        /// <param name="tex">The source texture containing all beam frames.</param>
        /// <param name="rects">The rectangles defining each frame's location.</param>
        /// <param name="totalFrames">The number of animation frames.</param>
        public BeamSheet(Texture2D tex, Rectangle[] rects, int totalFrames)
            :base(tex, rects)
        {
            TotalFrames = totalFrames;
        }

        /// <summary>
        /// Imports a beam sheet from a directory containing Head.png, Body.png, Tail.png, and BeamData.xml.
        /// </summary>
        /// <param name="path">The path to the directory containing beam assets.</param>
        /// <returns>A new BeamSheet imported from the specified directory.</returns>
        public static new BeamSheet Import(string path)
        {
            if (File.Exists(path + "BeamData.xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path + "BeamData.xml");
                int totalFrames = Convert.ToInt32(doc.SelectSingleNode("BeamData/TotalFrames").InnerText);

                List<(Color[] tex, int width, int height)> sheets = new List<(Color[], int, int)>();
                Rectangle[] rects = new Rectangle[3 * totalFrames];
                int maxWidth = 0;
                int maxHeight = 0;

                for (int ii = 0; ii < 3; ii++)
                {
                    using (FileStream fileStream = new FileStream(path + ((BeamFrame)ii).ToString() + ".png", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (Texture2D newSheet = ImportTex(fileStream))
                        {

                            for (int jj = 0; jj < totalFrames; jj++)
                                rects[ii * totalFrames + jj] = new Rectangle(newSheet.Width / totalFrames * jj, maxHeight, newSheet.Width / totalFrames, newSheet.Height);

                            maxWidth = Math.Max(maxWidth, newSheet.Width);
                            maxHeight += newSheet.Height;
                            sheets.Add((BaseSheet.GetData(newSheet), newSheet.Width, newSheet.Height));
                        }
                    }
                }
                
                Color[] texColors = new Color[maxWidth * maxHeight];

                int curHeight = 0;
                for (int ii = 0; ii < sheets.Count; ii++)
                {
                    BaseSheet.Blit(sheets[ii].tex, texColors, new Point(sheets[ii].width, sheets[ii].height), new Point(maxWidth, maxHeight), new Point(0, curHeight), SpriteEffects.None);
                    curHeight += sheets[ii].height;
                }

                Texture2D tex = new Texture2D(device, maxWidth, maxHeight);
                tex.SetData<Color>(0, null, texColors, 0, texColors.Length);
                return new BeamSheet(tex, rects, totalFrames);
            }
            else
                throw new Exception("Error finding XML file in " + path + ".");
        }

        /// <summary>
        /// Exports a beam sheet to a directory as separate Head.png, Body.png, Tail.png, and BeamData.xml files.
        /// </summary>
        /// <param name="sheet">The beam sheet to export.</param>
        /// <param name="baseDirectory">The directory to export to.</param>
        public static void Export(BeamSheet sheet, string baseDirectory)
        {
            //export head
            for(int ii = 0; ii < 3; ii++)
            {
                Rectangle referenceRect = sheet.spriteRects[ii * sheet.TotalFrames];
                Point imgSize = new Point(referenceRect.Width * sheet.TotalFrames, referenceRect.Height);

                Color[] part_colors = BaseSheet.GetData(sheet, 0, referenceRect.Y, sheet.Width, referenceRect.Height);
                ExportColors(baseDirectory + ((BeamFrame)ii).ToString() + ".png", part_colors, imgSize);
            }

            //export xml
            XmlDocument doc = new XmlDocument();
            XmlNode configNode = doc.CreateXmlDeclaration("1.0", null, null);
            doc.AppendChild(configNode);

            XmlNode docNode = doc.CreateElement("BeamData");
            docNode.AppendInnerTextChild(doc, "TotalFrames", sheet.TotalFrames.ToString());
            doc.AppendChild(docNode);

            doc.Save(baseDirectory + "BeamData.xml");
        }


        /// <summary>
        /// Loads a beam sheet from a binary stream.
        /// </summary>
        /// <param name="reader">The binary reader to read from.</param>
        /// <returns>A new BeamSheet loaded from the stream.</returns>
        public static new BeamSheet Load(BinaryReader reader)
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
            int frameCount = reader.ReadInt32();
            return new BeamSheet(tex, rects, frameCount);
            
        }

        /// <summary>
        /// Creates a fallback beam sheet using the default error texture.
        /// </summary>
        /// <returns>A BeamSheet containing the default fallback texture.</returns>
        public static new BeamSheet LoadError()
        {
            Rectangle[] rects = new Rectangle[3];
            for (int ii = 0; ii < rects.Length; ii++)
                rects[ii] = new Rectangle(0, 0, defaultTex.Width, defaultTex.Height);
            return new BeamSheet(defaultTex, rects, 1);
        }

        /// <summary>
        /// Saves the beam sheet to a binary stream.
        /// </summary>
        /// <param name="writer">The binary writer to write to.</param>
        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);
            writer.Write(TotalFrames);
        }

        /// <summary>
        /// Gets the source rectangle for a specific beam component and frame.
        /// </summary>
        /// <param name="component">The beam component (Head, Body, or Tail).</param>
        /// <param name="frame">The animation frame index.</param>
        /// <returns>The source rectangle for the specified frame.</returns>
        private Rectangle getBeamFrame(BeamFrame component, int frame)
        {
            int index = (int)component * TotalFrames + frame;
            return spriteRects[index];
        }

        /// <summary>
        /// Draws a complete beam with head, body, and tail in the specified direction.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The starting position of the beam.</param>
        /// <param name="frame">The animation frame to draw.</param>
        /// <param name="dir">The direction the beam points.</param>
        /// <param name="offset">The offset from the starting position.</param>
        /// <param name="length">The length of the beam body.</param>
        /// <param name="color">The color tint to apply.</param>
        public void DrawBeam(SpriteBatch spriteBatch, Vector2 pos, int frame, Dir8 dir, int offset, int length, Color color)
        {
            Loc dirLoc = dir.GetLoc();

            Loc diff = dirLoc * (length + offset);
            Rectangle body = getBeamFrame(BeamFrame.Body, frame);
            Draw(spriteBatch, new Vector2(pos.X + diff.X, pos.Y + diff.Y), body, new Vector2(body.Width / 2, body.Height),
                color, new Vector2(1, dir.IsDiagonal() ? (float)(length * 1.4142136 + 1) : length), (float)((int)dir * Math.PI / 4));

            diff = dirLoc * offset;
            Rectangle tail = getBeamFrame(BeamFrame.Tail, frame);
            Draw(spriteBatch, new Vector2(pos.X + diff.X, pos.Y + diff.Y), tail, color, new Vector2(1), (float)((int)dir * Math.PI / 4));

            diff = dirLoc * (length + offset - 1);
            Rectangle head = getBeamFrame(BeamFrame.Head, frame);
            Draw(spriteBatch, new Vector2(pos.X + diff.X, pos.Y + diff.Y), head, color, new Vector2(1), (float)((int)dir * Math.PI / 4));
        }

        /// <summary>
        /// Draws a vertical column beam extending upward from the specified position.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The base position of the column.</param>
        /// <param name="frame">The animation frame to draw.</param>
        /// <param name="color">The color tint to apply.</param>
        public void DrawColumn(SpriteBatch spriteBatch, Vector2 pos, int frame, Color color)
        {
            Rectangle head = getBeamFrame(BeamFrame.Head, frame);
            Draw(spriteBatch, pos - new Vector2(head.Width / 2, head.Height / 2), head, color);

            Rectangle body = getBeamFrame(BeamFrame.Body, frame);
            while (pos.Y > 0)
            {
                Draw(spriteBatch, pos - new Vector2(body.Width / 2, body.Height), body, color);
                pos.Y -= body.Height;
            }
        }
    }
}
