using System.Collections.Generic;
using RogueEssence.Content;
using RogueEssence.Data;
using RogueEssence.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueEssence
{
    /// <summary>
    /// Scene that displays the game title screen.
    /// Shows the title logo and waits for player input to proceed to the main menu.
    /// </summary>
    public class TitleScene : BaseScene
    {
        /// <summary>
        /// Time in frames to wait before showing the "Press Enter" subtitle.
        /// </summary>
        const int ENTER_WAIT_TIME = 90;

        /// <summary>
        /// Duration of the flashing effect cycle for the subtitle in frames.
        /// </summary>
        const int ENTER_FLASH_TIME = 60;

        private bool hideTitle;
        private ulong startTime;

        /// <summary>
        /// Saved menu state for returning to the title menu from other screens.
        /// </summary>
        public static List<IInteractable> TitleMenuSaveState;

        /// <summary>
        /// Initializes a new instance of the TitleScene class.
        /// </summary>
        /// <param name="hideTitle">Whether to hide the title and go directly to the menu.</param>
        public TitleScene(bool hideTitle) : base()
        {
            this.hideTitle = hideTitle;
        }

        /// <summary>
        /// Called when exiting the scene. No cleanup required.
        /// </summary>
        public override void Exit() { }

        /// <summary>
        /// Called when the scene begins.
        /// Starts playing the title background music.
        /// </summary>
        public override void Begin()
        {
            //set up title, fade, and start music
            GameManager.Instance.BGM(GraphicsManager.TitleBGM, true);
            startTime = GraphicsManager.TotalFrameTick;
        }

        /// <summary>
        /// Processes input for the title scene.
        /// Waits for any key/button press to proceed to the main menu.
        /// </summary>
        /// <returns>An enumerator for coroutine execution.</returns>
        public override IEnumerator<YieldInstruction> ProcessInput()
        {
            if (!hideTitle && GameManager.Instance.InputManager.AnyKeyPressed() || GameManager.Instance.InputManager.AnyButtonPressed())
            {
                GameManager.Instance.SE("Menu/Confirm");
                hideTitle = true;
            }
            if (hideTitle)
            {
                DataManager.Instance.SetProgress(null);
                DataManager.Instance.LoadProgress();
                if (TitleMenuSaveState == null)
                    yield return CoroutineManager.Instance.StartCoroutine(MenuManager.Instance.ProcessMenuCoroutine(new TopMenu()));
                else
                {
                    List<IInteractable> save = TitleMenuSaveState;
                    TitleMenuSaveState = null;
                    MenuManager.Instance.LoadMenuState(save);
                    yield return CoroutineManager.Instance.StartCoroutine(MenuManager.Instance.ProcessMenuCoroutine());
                }
            }
            else
                yield return new WaitForFrames(1);
        }

        /// <summary>
        /// Draws the title scene including background, title logo, and flashing subtitle.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            float window_scale = GraphicsManager.WindowZoom;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(new Vector3(window_scale, window_scale, 1)));


            BaseSheet bg = GraphicsManager.GetBackground(GraphicsManager.TitleBG);
            bg.Draw(spriteBatch, new Vector2(), null);

            if (!hideTitle)
            {
                BaseSheet title = GraphicsManager.Title;
                title.Draw(spriteBatch, new Vector2(GraphicsManager.ScreenWidth / 2 - title.Width / 2, 0), null);

                if ((GraphicsManager.TotalFrameTick - startTime) > (ulong)FrameTick.FrameToTick(ENTER_WAIT_TIME)
                    && ((GraphicsManager.TotalFrameTick - startTime) / (ulong)FrameTick.FrameToTick(ENTER_FLASH_TIME / 2)) % 2 == 0)
                {
                    BaseSheet subtitle = GraphicsManager.Subtitle;
                    subtitle.Draw(spriteBatch, new Vector2(GraphicsManager.ScreenWidth / 2 - subtitle.Width / 2, GraphicsManager.ScreenHeight * 3 / 4), null);
                }
            }
            spriteBatch.End();
        }

    }
}
