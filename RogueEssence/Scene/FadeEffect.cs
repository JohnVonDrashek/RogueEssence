using System;
using System.Collections.Generic;
using System.IO;
using RogueElements;
using RogueEssence.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueEssence.Data;
using RogueEssence.Menu;
using RogueEssence.Dungeon;
using RogueEssence.Ground;
using RogueEssence.Script;
using RogueEssence.Dev;
using RogueEssence.LevelGen;
using System.Xml.Linq;

namespace RogueEssence
{
    /// <summary>
    /// Abstract base class for screen fade effects.
    /// Provides common functionality for fading visual elements in and out.
    /// </summary>
    public abstract class FadeFX
    {
        /// <summary>
        /// The current fade amount from 0 (invisible) to 1 (fully visible).
        /// </summary>
        public float fadeAmount;

        /// <summary>
        /// Draws the fade effect if the fade amount is greater than zero.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (fadeAmount > 0)
                DrawInternal(spriteBatch);
        }

        /// <summary>
        /// Internal drawing method to be implemented by derived classes.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        protected abstract void DrawInternal(SpriteBatch spriteBatch);

        /// <summary>
        /// Performs the fade animation over the specified time.
        /// </summary>
        /// <param name="fadeIn">True to fade in, false to fade out.</param>
        /// <param name="fadeTime">The duration of the fade in frames.</param>
        /// <returns>An enumerator for coroutine execution.</returns>
        protected IEnumerator<YieldInstruction> FadeInternal(bool fadeIn, int fadeTime)
        {
            long currentFadeTime = fadeTime;
            while (currentFadeTime > 0)
            {
                currentFadeTime--;
                float amount = 0f;
                if (fadeIn)
                    amount = ((float)currentFadeTime / (float)fadeTime);
                else
                    amount = ((float)(fadeTime - currentFadeTime) / (float)fadeTime);
                fadeAmount = 1f - amount;
                yield return new WaitForFrames(1);
            }
        }
    }

    /// <summary>
    /// Fade effect that covers the screen with a solid color (black or white).
    /// </summary>
    public class ScreenFadeFX : FadeFX
    {
        /// <summary>
        /// Whether to fade to white instead of black.
        /// </summary>
        public bool fadeWhite;

        /// <summary>
        /// Draws the screen fade overlay.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        protected override void DrawInternal(SpriteBatch spriteBatch)
        {
            GraphicsManager.Pixel.Draw(spriteBatch, new Rectangle(0, 0, GraphicsManager.ScreenWidth, GraphicsManager.ScreenHeight), null, (fadeWhite ? Color.White : Color.Black) * fadeAmount);
        }

        /// <summary>
        /// Immediately sets the fade state without animation.
        /// </summary>
        /// <param name="faded">True to set fully faded, false for no fade.</param>
        /// <param name="useWhite">Whether to use white instead of black.</param>
        public void SetFade(bool faded, bool useWhite)
        {
            fadeAmount = faded ? 1f : 0f;
            fadeWhite = useWhite;
        }

        /// <summary>
        /// Performs a screen fade animation.
        /// </summary>
        /// <param name="fadeIn">True to fade in (to colored screen), false to fade out (to clear).</param>
        /// <param name="useWhite">Whether to use white instead of black.</param>
        /// <param name="fadeTime">The duration of the fade in frames.</param>
        /// <returns>An enumerator for coroutine execution.</returns>
        public IEnumerator<YieldInstruction> Fade(bool fadeIn, bool useWhite, int fadeTime)
        {
            if (!fadeIn && fadeAmount == 0f)
                yield break;
            if (fadeIn && fadeAmount == 1f)
            {
                SetFade(true, useWhite);
                yield break;
            }

            fadeWhite = useWhite;

            yield return CoroutineManager.Instance.StartCoroutine(FadeInternal(fadeIn, fadeTime));
        }
    }

    /// <summary>
    /// Fade effect that displays title text in the center of the screen.
    /// </summary>
    public class TitleFadeFX : FadeFX
    {
        private string title;

        /// <summary>
        /// Initializes a new instance of the TitleFadeFX class.
        /// </summary>
        public TitleFadeFX()
        {
            title = "";
        }

        /// <summary>
        /// Draws the title text overlay.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        protected override void DrawInternal(SpriteBatch spriteBatch)
        {
            GraphicsManager.DungeonFont.DrawText(spriteBatch, GraphicsManager.ScreenWidth / 2, GraphicsManager.ScreenHeight / 2,
                        title, null, DirV.None, DirH.None, Color.White * fadeAmount);
        }

        /// <summary>
        /// Performs a title text fade animation.
        /// </summary>
        /// <param name="fadeIn">True to fade in, false to fade out.</param>
        /// <param name="newTitle">The title text to display.</param>
        /// <param name="fadeTime">The duration of the fade in frames.</param>
        /// <returns>An enumerator for coroutine execution.</returns>
        public IEnumerator<YieldInstruction> Fade(bool fadeIn, string newTitle, int fadeTime)
        {
            if (fadeIn)
                title = newTitle;

            yield return CoroutineManager.Instance.StartCoroutine(FadeInternal(fadeIn, fadeTime));

            if (!fadeIn)
                title = "";
        }
    }

    /// <summary>
    /// Fade effect that displays an animated background image.
    /// </summary>
    public class BGFadeFX : FadeFX
    {
        private BGAnimData bg;

        /// <summary>
        /// Initializes a new instance of the BGFadeFX class.
        /// </summary>
        public BGFadeFX()
        {
            bg = new BGAnimData();
        }

        /// <summary>
        /// Draws the background image overlay.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        protected override void DrawInternal(SpriteBatch spriteBatch)
        {
            if (bg.AnimIndex == "")
                return;

            DirSheet sheet = GraphicsManager.GetBackground(bg.AnimIndex);
            sheet.DrawDir(spriteBatch, new Vector2(GraphicsManager.ScreenWidth / 2 - sheet.TileWidth / 2, GraphicsManager.ScreenHeight / 2 - sheet.TileHeight / 2),
                bg.GetCurrentFrame(GraphicsManager.TotalFrameTick, sheet.TotalFrames), Dir8.Down, Color.White * ((float)bg.Alpha / 255) * fadeAmount);
        }

        /// <summary>
        /// Performs a background image fade animation.
        /// </summary>
        /// <param name="fadeIn">True to fade in, false to fade out.</param>
        /// <param name="newBG">The background animation data to display.</param>
        /// <param name="fadeTime">The duration of the fade in frames.</param>
        /// <returns>An enumerator for coroutine execution.</returns>
        public IEnumerator<YieldInstruction> Fade(bool fadeIn, BGAnimData newBG, int fadeTime)
        {
            if (fadeIn)
                bg = newBG;

            yield return CoroutineManager.Instance.StartCoroutine(FadeInternal(fadeIn, fadeTime));

            if (!fadeIn)
                bg = new BGAnimData();
        }
    }
}
