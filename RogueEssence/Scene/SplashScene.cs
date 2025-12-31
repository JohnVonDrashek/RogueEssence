using System.Collections.Generic;
using RogueEssence.Menu;
using Microsoft.Xna.Framework.Graphics;

namespace RogueEssence
{
    /// <summary>
    /// Scene displayed at game startup before the title screen.
    /// Handles initial language selection if not set and transitions to the title screen.
    /// </summary>
    public class SplashScene : BaseScene
    {
        /// <summary>
        /// Initializes a new instance of the SplashScene class.
        /// </summary>
        public SplashScene()
        {
        }

        /// <summary>
        /// Called when exiting the scene. No cleanup required for splash screen.
        /// </summary>
        public override void Exit() { }

        /// <summary>
        /// Called when the scene begins. No initialization required for splash screen.
        /// </summary>
        public override void Begin()
        {

        }

        /// <summary>
        /// Processes input for the splash scene.
        /// Shows language menu if language not set, then transitions to title screen.
        /// </summary>
        /// <returns>An enumerator for coroutine execution.</returns>
        public override IEnumerator<YieldInstruction> ProcessInput()
        {
            if (DiagManager.Instance.CurSettings.Language == "")
            {
                yield return CoroutineManager.Instance.StartCoroutine(MenuManager.Instance.ProcessMenuCoroutine(new LanguageMenu()));
                yield return new WaitForFrames(30);
            }
            GameManager.Instance.SetFade(true, false);
            GameManager.Instance.SceneOutcome = StartToTitle();
        }

        /// <summary>
        /// Start to title, without all the unneeded restart logic.
        /// </summary>
        /// <returns>An enumerator for coroutine execution.</returns>
        public IEnumerator<YieldInstruction> StartToTitle()
        {
            GameManager.Instance.MoveToScene(new TitleScene(false));
            yield return CoroutineManager.Instance.StartCoroutine(GameManager.Instance.FadeIn());
        }

        /// <summary>
        /// Draws the splash scene. Currently renders nothing.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {

        }

    }
}
