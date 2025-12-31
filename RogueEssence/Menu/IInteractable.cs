using Microsoft.Xna.Framework.Graphics;
using RogueElements;

namespace RogueEssence.Menu
{
    /// <summary>
    /// Defines the contract for interactive menu elements that can receive user input and be rendered.
    /// This interface extends <see cref="ILabeled"/> to provide identification capabilities for menu navigation.
    /// </summary>
    public interface IInteractable : ILabeled
    {
        /// <summary>
        /// Gets a value indicating whether this menu acts as a checkpoint in the menu stack.
        /// When clearing menus, checkpoints define boundaries for partial stack clearing.
        /// </summary>
        bool IsCheckpoint { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this menu is visible and should be rendered.
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this menu is inactive and should not receive input.
        /// Menus below the top of the stack are typically marked as inactive.
        /// </summary>
        bool Inactive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this menu blocks rendering of menus below it in the stack.
        /// When true, menus underneath this one will not be drawn.
        /// </summary>
        bool BlockPrevious { get; set; }

        /// <summary>
        /// Processes user input for this menu.
        /// </summary>
        /// <param name="input">The input manager containing the current input state.</param>
        void Update(InputManager input);

        /// <summary>
        /// Processes time-based actions and animations for this menu.
        /// </summary>
        /// <param name="elapsedTime">The time elapsed since the last frame.</param>
        void ProcessActions(FrameTick elapsedTime);

        /// <summary>
        /// Draws this menu to the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        void Draw(SpriteBatch spriteBatch);
    }
}
