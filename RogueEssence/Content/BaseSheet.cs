using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace RogueEssence.Content
{
    /// <summary>
    /// Base class for all sprite sheet types in the game.
    /// Provides core functionality for loading, saving, drawing, and manipulating textures.
    /// Implements IDisposable for proper texture resource management.
    /// </summary>
    public class BaseSheet : IDisposable
    {
        /// <summary>
        /// The shared graphics device used for texture operations.
        /// </summary>
        protected static GraphicsDevice device;

        /// <summary>
        /// The default fallback texture used when loading fails.
        /// </summary>
        protected static Texture2D defaultTex;

        /// <summary>
        /// The underlying texture data for this sheet.
        /// </summary>
        protected Texture2D baseTexture { get; private set; }

        /// <summary>
        /// Gets the width of the sprite sheet in pixels.
        /// </summary>
        public int Width { get { return baseTexture.Width; } }

        /// <summary>
        /// Gets the height of the sprite sheet in pixels.
        /// </summary>
        public int Height { get { return baseTexture.Height; } }

        /// <summary>
        /// Gets the memory size of the texture data in bytes.
        /// </summary>
        public long MemSize { get; private set; }

        /// <summary>
        /// Initializes the static graphics device and default texture for all BaseSheet instances.
        /// Must be called before creating any sprite sheets.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device to use for texture operations.</param>
        /// <param name="tex">The default fallback texture.</param>
        public static void InitBase(GraphicsDevice graphicsDevice, Texture2D tex)
        {
            device = graphicsDevice;
            defaultTex = tex;
        }

        /// <summary>
        /// Creates a new blank sprite sheet with the specified dimensions.
        /// </summary>
        /// <param name="width">The width of the texture in pixels.</param>
        /// <param name="height">The height of the texture in pixels.</param>
        public BaseSheet(int width, int height)
        {
            baseTexture = new Texture2D(device, width, height);
            MemSize = -1;
        }

        /// <summary>
        /// Creates a sprite sheet from an existing texture.
        /// </summary>
        /// <param name="tex">The texture to use as the base.</param>
        protected BaseSheet(Texture2D tex)
        {
            baseTexture = tex;
            MemSize = -1;
        }

        /// <summary>
        /// Releases the texture resources used by this sprite sheet.
        /// </summary>
        public virtual void Dispose()
        {
            if (baseTexture != defaultTex)
                baseTexture.Dispose();
        }

        ~BaseSheet()
        {
            Dispose();
        }

        /// <summary>
        /// Imports a sprite sheet from a PNG file on disk.
        /// </summary>
        /// <param name="path">The file path to the PNG image.</param>
        /// <returns>A new BaseSheet containing the imported texture.</returns>
        public static BaseSheet Import(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Texture2D tex = ImportTex(stream);
                return new BaseSheet(tex);
            }
        }

        /// <summary>
        /// Loads a sprite sheet from a binary stream.
        /// </summary>
        /// <param name="reader">The binary reader to read from.</param>
        /// <returns>A new BaseSheet containing the loaded texture.</returns>
        public static BaseSheet Load(BinaryReader reader)
        {
            long length = reader.ReadInt64();
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(reader.ReadBytes((int)length), 0, (int)length);
                ms.Position = 0;
                Texture2D tex = Texture2D.FromStream(device, ms);
                return new BaseSheet(tex);
            }
        }

        /// <summary>
        /// Creates a fallback sprite sheet using the default error texture.
        /// Used when loading fails.
        /// </summary>
        /// <returns>A BaseSheet containing the default fallback texture.</returns>
        public static BaseSheet LoadError()
        {
            return new BaseSheet(defaultTex);
        }

        /// <summary>
        /// Saves the sprite sheet to a binary stream.
        /// </summary>
        /// <param name="writer">The binary writer to write to.</param>
        public virtual void Save(BinaryWriter writer)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                baseTexture.SaveAsPng(stream, baseTexture.Width, baseTexture.Height);
                MemSize = stream.Position;
                writer.Write(MemSize);
                writer.Write(stream.ToArray());
            }
        }

        /// <summary>
        /// Exports the sprite sheet as a PNG to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to export to.</param>
        public void Export(Stream stream)
        {
            ExportTex(stream, baseTexture);
        }

        /// <summary>
        /// Draws a portion of the sprite sheet at the specified position with white color.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="sourceRect">The source rectangle within the texture, or null for entire texture.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 pos, Rectangle? sourceRect)
        {
            Draw(spriteBatch, pos, sourceRect, Color.White);
        }

        /// <summary>
        /// Draws a portion of the sprite sheet at the specified position with the given color.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="sourceRect">The source rectangle within the texture, or null for entire texture.</param>
        /// <param name="color">The color tint to apply.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 pos, Rectangle? sourceRect, Color color)
        {
            Draw(spriteBatch, pos, sourceRect, color, new Vector2(1));
        }

        /// <summary>
        /// Draws a portion of the sprite sheet with color and scale.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="sourceRect">The source rectangle within the texture, or null for entire texture.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="scale">The scale factor to apply.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 pos, Rectangle? sourceRect, Color color, Vector2 scale)
        {
            Draw(spriteBatch, pos, sourceRect, color, scale, SpriteEffects.None);
        }

        /// <summary>
        /// Draws a portion of the sprite sheet with color, scale, and sprite effects.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="sourceRect">The source rectangle within the texture, or null for entire texture.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="scale">The scale factor to apply.</param>
        /// <param name="effect">The sprite effects (flip horizontal/vertical).</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 pos, Rectangle? sourceRect, Color color, Vector2 scale, SpriteEffects effect)
        {
            spriteBatch.Draw(baseTexture, pos, sourceRect, color, 0f, Vector2.Zero, scale, effect, 0);
        }

        /// <summary>
        /// Draws a portion of the sprite sheet with rotation, centered on the source rectangle.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="sourceRect">The source rectangle within the texture.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="scale">The scale factor to apply.</param>
        /// <param name="rotation">The rotation angle in radians.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 pos, Rectangle sourceRect, Color color, Vector2 scale, float rotation)
        {
            Draw(spriteBatch, pos, sourceRect, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), color, scale, rotation, SpriteEffects.None);
        }

        /// <summary>
        /// Draws a portion of the sprite sheet with rotation and sprite effects, centered on the source rectangle.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="sourceRect">The source rectangle within the texture.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="scale">The scale factor to apply.</param>
        /// <param name="rotation">The rotation angle in radians.</param>
        /// <param name="spriteEffects">The sprite effects (flip horizontal/vertical).</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 pos, Rectangle sourceRect, Color color, Vector2 scale, float rotation, SpriteEffects spriteEffects)
        {
            Draw(spriteBatch, pos, sourceRect, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), color, scale, rotation, spriteEffects);
        }

        /// <summary>
        /// Draws a portion of the sprite sheet with a custom origin point for rotation.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="sourceRect">The source rectangle within the texture.</param>
        /// <param name="origin">The origin point for rotation and scaling.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="scale">The scale factor to apply.</param>
        /// <param name="rotation">The rotation angle in radians.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 pos, Rectangle sourceRect, Vector2 origin, Color color, Vector2 scale, float rotation)
        {
            Draw(spriteBatch, pos, sourceRect, origin, color, scale, rotation, SpriteEffects.None);
        }

        /// <summary>
        /// Draws a portion of the sprite sheet with full control over all drawing parameters.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="pos">The position to draw at.</param>
        /// <param name="sourceRect">The source rectangle within the texture.</param>
        /// <param name="origin">The origin point for rotation and scaling.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="scale">The scale factor to apply.</param>
        /// <param name="rotation">The rotation angle in radians.</param>
        /// <param name="spriteEffects">The sprite effects (flip horizontal/vertical).</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 pos, Rectangle sourceRect, Vector2 origin, Color color, Vector2 scale, float rotation, SpriteEffects spriteEffects)
        {
            spriteBatch.Draw(baseTexture, pos, sourceRect, color, rotation, origin, scale, spriteEffects, 0);
        }

        /// <summary>
        /// Draws a portion of the sprite sheet stretched to fit a destination rectangle.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="destRect">The destination rectangle to draw into.</param>
        /// <param name="sourceRect">The source rectangle within the texture, or null for entire texture.</param>
        /// <param name="color">The color tint to apply.</param>
        public void Draw(SpriteBatch spriteBatch, Rectangle destRect, Rectangle? sourceRect, Color color)
        {
            spriteBatch.Draw(baseTexture, destRect, sourceRect, color, 0f, Vector2.Zero, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draws the default fallback texture into the specified destination rectangle.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="destRect">The destination rectangle to draw into.</param>
        public void DrawDefault(SpriteBatch spriteBatch, Rectangle destRect)
        {
            spriteBatch.Draw(defaultTex, destRect, Color.White);
        }

        /// <summary>
        /// Replaces the current texture with a new one, disposing the old texture.
        /// </summary>
        /// <param name="tex">The new texture to use.</param>
        public void SetTexture(Texture2D tex)
        {
            baseTexture.Dispose();
            baseTexture = tex;
        }

        /// <summary>
        /// Checks if a rectangular region of the sprite sheet is completely transparent.
        /// </summary>
        /// <param name="srcPx">The X coordinate of the source region.</param>
        /// <param name="srcPy">The Y coordinate of the source region.</param>
        /// <param name="srcW">The width of the source region.</param>
        /// <param name="srcH">The height of the source region.</param>
        /// <returns>True if all pixels in the region have zero alpha, false otherwise.</returns>
        public bool IsBlank(int srcPx, int srcPy, int srcW, int srcH)
        {
            Color[] color = new Color[srcW * srcH];
            baseTexture.GetData<Color>(0, new Rectangle(srcPx, srcPy, srcW, srcH), color, 0, color.Length);
            for (int ii = 0; ii < srcW * srcH; ii++)
            {
                if (color[ii].A > 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Multiplies all colors by the alpha, or divides if reversed.
        /// Used to conform with XNA's particular method of rendering.
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="reverse"></param>
        private static void premultiply(Texture2D tex, bool reverse)
        {
            Color[] color = new Color[tex.Width * tex.Height];
            tex.GetData<Color>(0, null, color, 0, color.Length);
            for (int ii = 0; ii < tex.Width * tex.Height; ii++)
            {
                if (reverse)
                {
                    if (color[ii].A > 0)
                        color[ii] = new Color(color[ii].R * 255 / color[ii].A, color[ii].G * 255 / color[ii].A, color[ii].B * 255 / color[ii].A, color[ii].A);
                }
                else
                    color[ii] = new Color(color[ii].R * color[ii].A / 255, color[ii].G * color[ii].A / 255, color[ii].B * color[ii].A / 255, color[ii].A);
            }
            tex.SetData<Color>(0, null, color, 0, color.Length);
        }


        /// <summary>
        /// Imports a texture from a stream and applies premultiplied alpha.
        /// </summary>
        /// <param name="stream">The stream containing the image data.</param>
        /// <returns>The imported texture with premultiplied alpha.</returns>
        public static Texture2D ImportTex(Stream stream)
        {
            Texture2D tex = Texture2D.FromStream(device, stream);
            premultiply(tex, false);
            return tex;
        }

        /// <summary>
        /// Exports a texture to a stream, reversing premultiplied alpha for standard PNG format.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="tex">The texture to export.</param>
        public static void ExportTex(Stream stream, Texture2D tex)
        {
            Texture2D tempTex = CreateTexCopy(tex);
            premultiply(tempTex, true);
            tempTex.SaveAsPng(stream, tempTex.Width, tempTex.Height);
            tempTex.Dispose();
        }



        /// <summary>
        /// Exports color data as a PNG file to the specified file path.
        /// </summary>
        /// <param name="fileName">The path to save the file to.</param>
        /// <param name="colors">The color data array.</param>
        /// <param name="imgSize">The dimensions of the image.</param>
        public static void ExportColors(string fileName, Color[] colors, Point imgSize)
        {
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                ExportColors(stream, colors, imgSize);
        }

        /// <summary>
        /// Exports color data as a PNG to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="colors">The color data array.</param>
        /// <param name="imgSize">The dimensions of the image.</param>
        public static void ExportColors(Stream stream, Color[] colors, Point imgSize)
        {
            Texture2D animImg = new Texture2D(device, imgSize.X, imgSize.Y);
            animImg.SetData<Color>(0, null, colors, 0, colors.Length);
            ExportTex(stream, animImg);
            animImg.Dispose();
        }

        /// <summary>
        /// Gets the bounding rectangle of all non-transparent pixels within the specified bounds.
        /// </summary>
        /// <param name="bounds">The bounds to search within.</param>
        /// <returns>The rectangle containing all non-transparent pixels, relative to the input bounds.</returns>
        public Rectangle GetCoveredRect(Rectangle bounds)
        {
            return GetCoveredRect(baseTexture, bounds);
        }

        /// <summary>
        /// Returns the rectangle bound of all nontransparent pixels within the specified bound.
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="bounds"></param>
        /// <returns>Rectangle bounds relative to input bounds.</returns>
        public static Rectangle GetCoveredRect(Texture2D tex, Rectangle bounds)
        {
            int top = bounds.Height;
            int left = bounds.Width;
            int bottom = 0;
            int right = 0;
            Color[] color = new Color[bounds.Width * bounds.Height];
            tex.GetData<Color>(0, bounds, color, 0, color.Length);
            for (int ii = 0; ii < bounds.Width * bounds.Height; ii++)
            {
                if (color[ii].A > 0)
                {
                    int locX = ii % bounds.Width;
                    int locY = ii / bounds.Width;
                    top = Math.Min(locY, top);
                    left = Math.Min(locX, left);
                    bottom = Math.Max(locY + 1, bottom);
                    right = Math.Max(locX + 1, right);
                }
            }
            return new Rectangle(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Creates a copy of the specified texture.
        /// </summary>
        /// <param name="source">The texture to copy.</param>
        /// <returns>A new texture containing the same pixel data.</returns>
        public static Texture2D CreateTexCopy(Texture2D source)
        {
            Texture2D copy = new Texture2D(device, source.Width, source.Height);
            Color[] color = new Color[source.Width * source.Height];
            source.GetData<Color>(0, null, color, 0, color.Length);
            copy.SetData<Color>(0, null, color, 0, color.Length);
            return copy;
        }

        /// <summary>
        /// Copies a rectangular region from a source sheet to this sheet.
        /// </summary>
        /// <param name="source">The source sprite sheet to copy from.</param>
        /// <param name="srcPx">The X coordinate in the source.</param>
        /// <param name="srcPy">The Y coordinate in the source.</param>
        /// <param name="srcW">The width of the region to copy.</param>
        /// <param name="srcH">The height of the region to copy.</param>
        /// <param name="destX">The X coordinate in the destination.</param>
        /// <param name="destY">The Y coordinate in the destination.</param>
        public void Blit(BaseSheet source, int srcPx, int srcPy, int srcW, int srcH, int destX, int destY)
        {
            BaseSheet.Blit(source.baseTexture, baseTexture, srcPx, srcPy, srcW, srcH, destX, destY);
        }


        /// <summary>
        /// Copies a rectangular region from a source sheet to a destination texture.
        /// </summary>
        /// <param name="source">The source sprite sheet to copy from.</param>
        /// <param name="dest">The destination texture to copy to.</param>
        /// <param name="srcPx">The X coordinate in the source.</param>
        /// <param name="srcPy">The Y coordinate in the source.</param>
        /// <param name="srcW">The width of the region to copy.</param>
        /// <param name="srcH">The height of the region to copy.</param>
        /// <param name="destX">The X coordinate in the destination.</param>
        /// <param name="destY">The Y coordinate in the destination.</param>
        public static void Blit(BaseSheet source, Texture2D dest, int srcPx, int srcPy, int srcW, int srcH, int destX, int destY)
        {
            BaseSheet.Blit(source.baseTexture, dest, srcPx, srcPy, srcW, srcH, destX, destY);
        }

        /// <summary>
        /// Copies a rectangular region from a source texture to a destination texture.
        /// </summary>
        /// <param name="source">The source texture to copy from.</param>
        /// <param name="dest">The destination texture to copy to.</param>
        /// <param name="srcPx">The X coordinate in the source.</param>
        /// <param name="srcPy">The Y coordinate in the source.</param>
        /// <param name="srcW">The width of the region to copy.</param>
        /// <param name="srcH">The height of the region to copy.</param>
        /// <param name="destX">The X coordinate in the destination.</param>
        /// <param name="destY">The Y coordinate in the destination.</param>
        public static void Blit(Texture2D source, Texture2D dest, int srcPx, int srcPy, int srcW, int srcH, int destX, int destY)
        {
            Blit(source, dest, srcPx, srcPy, srcW, srcH, destX, destY, SpriteEffects.None);
        }

        /// <summary>
        /// Copies a rectangular region from a source texture to a destination texture with optional flipping.
        /// </summary>
        /// <param name="source">The source texture to copy from.</param>
        /// <param name="dest">The destination texture to copy to.</param>
        /// <param name="srcPx">The X coordinate in the source.</param>
        /// <param name="srcPy">The Y coordinate in the source.</param>
        /// <param name="srcW">The width of the region to copy.</param>
        /// <param name="srcH">The height of the region to copy.</param>
        /// <param name="destX">The X coordinate in the destination.</param>
        /// <param name="destY">The Y coordinate in the destination.</param>
        /// <param name="flip">The sprite effects to apply (horizontal/vertical flip).</param>
        public static void Blit(Texture2D source, Texture2D dest, int srcPx, int srcPy, int srcW, int srcH, int destX, int destY, SpriteEffects flip)
        {
            Color[] color = new Color[srcW * srcH];
            source.GetData<Color>(0, new Rectangle(srcPx, srcPy, srcW, srcH), color, 0, color.Length);
            bool flipH = (flip & SpriteEffects.FlipHorizontally) != SpriteEffects.None;
            bool flipV = (flip & SpriteEffects.FlipVertically) != SpriteEffects.None;
            if (flipH || flipV)
            {
                Color[] newColor = new Color[srcW * srcH];
                for (int xx = 0; xx < srcW; xx++)
                {
                    for (int yy = 0; yy < srcH; yy++)
                    {
                        int srcIdx = yy * srcW + xx;
                        int destIdx = (flipV ? srcH - yy - 1 : yy) * srcW + (flipH ? srcW - xx - 1 : xx);
                        newColor[destIdx] = color[srcIdx];
                    }
                }
                color = newColor;
            }
            dest.SetData<Color>(0, new Rectangle(destX, destY, srcW, srcH), color, 0, color.Length);
        }


        /// <summary>
        /// Copies color data from a source array to a destination array with optional flipping.
        /// </summary>
        /// <param name="source">The source color array.</param>
        /// <param name="dest">The destination color array.</param>
        /// <param name="srcSz">The dimensions of the source data.</param>
        /// <param name="destSz">The dimensions of the destination array.</param>
        /// <param name="destPt">The position in the destination to copy to.</param>
        /// <param name="flip">The sprite effects to apply (horizontal/vertical flip).</param>
        public static void Blit(Color[] source, Color[] dest, Point srcSz, Point destSz, Point destPt, SpriteEffects flip)
        {
            bool flipH = (flip & SpriteEffects.FlipHorizontally) != SpriteEffects.None;
            bool flipV = (flip & SpriteEffects.FlipVertically) != SpriteEffects.None;
            if (flipH || flipV)
            {
                Color[] newColor = new Color[source.Length];
                for (int xx = 0; xx < srcSz.X; xx++)
                {
                    for (int yy = 0; yy < srcSz.Y; yy++)
                    {
                        int srcIdx = yy * srcSz.X + xx;
                        int destIdx = (flipV ? srcSz.Y - yy - 1 : yy) * srcSz.X + (flipH ? srcSz.X - xx - 1 : xx);
                        newColor[destIdx] = source[srcIdx];
                    }
                }
                source = newColor;
            }
            for (int xx = 0; xx < srcSz.X; xx++)
            {
                for (int yy = 0; yy < srcSz.Y; yy++)
                {
                    int srcIdx = yy * srcSz.X + xx;
                    int destIdx = (yy + destPt.Y) * destSz.X + (xx + destPt.X);
                    dest[destIdx] = source[srcIdx];
                }
            }
        }

        /// <summary>
        /// Gets all pixel data from a sprite sheet as a color array.
        /// </summary>
        /// <param name="source">The sprite sheet to get data from.</param>
        /// <returns>An array containing all pixel colors.</returns>
        public static Color[] GetData(BaseSheet source)
        {
            return GetData(source.baseTexture, 0, 0, source.Width, source.Height);
        }

        /// <summary>
        /// Gets pixel data from a rectangular region of a sprite sheet.
        /// </summary>
        /// <param name="source">The sprite sheet to get data from.</param>
        /// <param name="srcPx">The X coordinate of the source region.</param>
        /// <param name="srcPy">The Y coordinate of the source region.</param>
        /// <param name="srcW">The width of the source region.</param>
        /// <param name="srcH">The height of the source region.</param>
        /// <returns>An array containing the pixel colors from the region.</returns>
        public static Color[] GetData(BaseSheet source, int srcPx, int srcPy, int srcW, int srcH)
        {
            return GetData(source.baseTexture, srcPx, srcPy, srcW, srcH);
        }

        /// <summary>
        /// Gets all pixel data from a texture as a color array.
        /// </summary>
        /// <param name="source">The texture to get data from.</param>
        /// <returns>An array containing all pixel colors.</returns>
        public static Color[] GetData(Texture2D source)
        {
            return GetData(source, 0, 0, source.Width, source.Height);
        }

        /// <summary>
        /// Gets pixel data from a rectangular region of a texture.
        /// </summary>
        /// <param name="source">The texture to get data from.</param>
        /// <param name="srcPx">The X coordinate of the source region.</param>
        /// <param name="srcPy">The Y coordinate of the source region.</param>
        /// <param name="srcW">The width of the source region.</param>
        /// <param name="srcH">The height of the source region.</param>
        /// <returns>An array containing the pixel colors from the region.</returns>
        public static Color[] GetData(Texture2D source, int srcPx, int srcPy, int srcW, int srcH)
        {
            Color[] color = new Color[srcW * srcH];
            source.GetData<Color>(0, new Rectangle(srcPx, srcPy, srcW, srcH), color, 0, color.Length);
            return color;
        }

        /// <summary>
        /// Fills a rectangular region with a solid color.
        /// </summary>
        /// <param name="srcColor">The color to fill with.</param>
        /// <param name="srcW">The width of the region to fill.</param>
        /// <param name="srcH">The height of the region to fill.</param>
        /// <param name="destX">The X coordinate in the destination.</param>
        /// <param name="destY">The Y coordinate in the destination.</param>
        public void BlitColor(Color srcColor, int srcW, int srcH, int destX, int destY)
        {
            BaseSheet.BlitColor(srcColor, baseTexture, srcW, srcH, destX, destY);
        }

        /// <summary>
        /// Fills a rectangular region of a texture with a solid color.
        /// </summary>
        /// <param name="srcColor">The color to fill with.</param>
        /// <param name="dest">The destination texture.</param>
        /// <param name="srcW">The width of the region to fill.</param>
        /// <param name="srcH">The height of the region to fill.</param>
        /// <param name="destX">The X coordinate in the destination.</param>
        /// <param name="destY">The Y coordinate in the destination.</param>
        public static void BlitColor(Color srcColor, Texture2D dest, int srcW, int srcH, int destX, int destY)
        {
            Color[] color = new Color[srcW * srcH];
            for (int ii = 0; ii < color.Length; ii++)
                color[ii] = srcColor;
            dest.SetData<Color>(0, new Rectangle(destX, destY, srcW, srcH), color, 0, color.Length);
        }

        /// <summary>
        /// Gets the color of a single pixel in the sprite sheet.
        /// </summary>
        /// <param name="x">The X coordinate of the pixel.</param>
        /// <param name="y">The Y coordinate of the pixel.</param>
        /// <returns>The color of the pixel at the specified coordinates.</returns>
        public Color GetPixel(int x, int y)
        {
            Color[] color = new Color[1];
            baseTexture.GetData<Color>(0, new Rectangle(x, y, 1, 1), color, 0, 1);
            return color[0];
        }
    }
}
