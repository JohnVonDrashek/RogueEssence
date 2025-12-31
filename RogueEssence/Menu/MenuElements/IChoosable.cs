using RogueElements;

namespace RogueEssence.Menu
{
    /// <summary>
    /// Defines the contract for menu elements that can be selected by the user.
    /// Choosable elements support both keyboard/gamepad and mouse interaction.
    /// </summary>
    public interface IChoosable : IMenuElement
    {
        /// <summary>
        /// Gets or sets the bounds of this choice for mouse hit detection.
        /// </summary>
        Rect Bounds { get; set; }

        /// <summary>
        /// Gets a value indicating whether this choice is currently selected (for multi-select).
        /// </summary>
        bool Selected { get; }

        /// <summary>
        /// Gets a value indicating whether this choice is currently being hovered by the mouse.
        /// </summary>
        bool Hovered { get; }

        /// <summary>
        /// Called when the mouse click state changes on this choice.
        /// </summary>
        /// <param name="clicked">True if the mouse button is pressed.</param>
        void OnMouseState(bool clicked);

        /// <summary>
        /// Called when the selection state changes (for multi-select functionality).
        /// </summary>
        /// <param name="select">True to select; false to deselect.</param>
        void OnSelect(bool select);

        /// <summary>
        /// Called when the mouse hover state changes.
        /// </summary>
        /// <param name="hover">True if the mouse is now hovering over this choice.</param>
        void OnHoverChanged(bool hover);

        /// <summary>
        /// Called when this choice is confirmed via keyboard/gamepad.
        /// </summary>
        void OnConfirm();
    }
}
