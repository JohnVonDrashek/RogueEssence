using System;
using System.Collections.Generic;
using RogueElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueEssence.Content;

namespace RogueEssence.Menu
{
    /// <summary>
    /// Abstract base class for selectable menu choices.
    /// Provides common functionality for mouse and keyboard selection with visual feedback.
    /// </summary>
    public abstract class MenuChoice : BaseMenuElement, IChoosable
    {
        /// <inheritdoc/>
        public Rect Bounds { get; set; }

        /// <summary>
        /// The action to execute when this choice is selected.
        /// </summary>
        public Action ChoiceAction;

        /// <summary>
        /// Whether this choice is enabled and selectable.
        /// </summary>
        public bool Enabled;

        /// <inheritdoc/>
        public bool Selected { get; private set; }

        /// <inheritdoc/>
        public bool Hovered => hover;

        private bool hover;
        private bool click;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuChoice"/> class with a label.
        /// </summary>
        /// <param name="label">The identifier label for this choice.</param>
        /// <param name="choiceAction">The action to execute when selected.</param>
        /// <param name="enabled">Whether this choice is enabled.</param>
        protected MenuChoice(string label, Action choiceAction, bool enabled)
        {
            Label = label;
            Bounds = new Rect();
            ChoiceAction = choiceAction;
            Enabled = enabled;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuChoice"/> class without a label.
        /// </summary>
        /// <param name="choiceAction">The action to execute when selected.</param>
        /// <param name="enabled">Whether this choice is enabled.</param>
        protected MenuChoice(Action choiceAction, bool enabled) : this("", choiceAction, enabled) { }

        /// <inheritdoc/>
        public void OnMouseState(bool clicked)
        {
            if (click && !clicked && hover)
            {
                if (Enabled)
                {
                    if (ChoiceAction != null)
                        ChoiceAction();
                }
            }
            else if (!click && clicked && hover)
            {
                if (Enabled)
                    GameManager.Instance.SE("Menu/Confirm");
                else
                    GameManager.Instance.SE("Menu/Cancel");
            }

            this.click = clicked && hover;
        }

        /// <summary>
        /// Selects or deselects this choice without playing a sound effect.
        /// </summary>
        /// <param name="select">True to select; false to deselect.</param>
        public void SilentSelect(bool select)
        {
            if (Enabled)
            {
                Selected = select;
            }
        }

        /// <inheritdoc/>
        public void OnSelect(bool select)
        {
            if (Enabled)
            {
                GameManager.Instance.SE("Menu/Toggle");
                Selected = select;
            }
            else
                GameManager.Instance.SE("Menu/Cancel");
        }

        /// <inheritdoc/>
        public void OnHoverChanged(bool hover)
        {
            this.hover = hover;
        }

        /// <inheritdoc/>
        public void OnConfirm()
        {
            if (Enabled)
            {
                GameManager.Instance.SE("Menu/Confirm");
                if (ChoiceAction != null)
                    ChoiceAction();
            }
            else
                GameManager.Instance.SE("Menu/Cancel");

        }

        /// <summary>
        /// Returns an iterator of all elements for the purpose of drawing.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<IMenuElement> GetDrawElements();

        public override void Draw(SpriteBatch spriteBatch, Loc offset)
        {
            //draw the highlight
            if (Selected)
                GraphicsManager.Pixel.Draw(spriteBatch, new Rectangle(Bounds.X + offset.X, Bounds.Y + offset.Y, Bounds.Size.X, Bounds.Size.Y), null, Color.Cyan * 0.5f);
            if (hover && Enabled)
                GraphicsManager.Pixel.Draw(spriteBatch, new Rectangle(Bounds.X + offset.X, Bounds.Y + offset.Y, Bounds.Size.X, Bounds.Size.Y), null, Color.White * (click ? 0.5f : 0.2f));
            //draw all elements with offset added
            foreach (IMenuElement element in GetDrawElements())
                element.Draw(spriteBatch, Bounds.Start + offset);
        }
    }


    /// <summary>
    /// A menu choice that displays simple text.
    /// </summary>
    public class MenuTextChoice : MenuChoice
    {
        /// <summary>
        /// The text element displayed for this choice.
        /// </summary>
        public MenuText Text;

        /// <summary>
        /// Initializes a new enabled <see cref="MenuTextChoice"/> with white text.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="choiceAction">The action to execute when selected.</param>
        public MenuTextChoice(string text, Action choiceAction) : this("", text, choiceAction, true, Color.White) { }

        /// <summary>
        /// Initializes a new labeled <see cref="MenuTextChoice"/> with white text.
        /// </summary>
        /// <param name="label">The identifier label for this choice.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="choiceAction">The action to execute when selected.</param>
        public MenuTextChoice(string label, string text, Action choiceAction) : this(label, text, choiceAction, true, Color.White) { }

        /// <summary>
        /// Initializes a new <see cref="MenuTextChoice"/> with custom color and enabled state.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="choiceAction">The action to execute when selected.</param>
        /// <param name="enabled">Whether this choice is enabled.</param>
        /// <param name="color">The text color.</param>
        public MenuTextChoice(string text, Action choiceAction, bool enabled, Color color) : this("", text, choiceAction, enabled, color) { }

        /// <summary>
        /// Initializes a new labeled <see cref="MenuTextChoice"/> with full customization.
        /// </summary>
        /// <param name="label">The identifier label for this choice.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="choiceAction">The action to execute when selected.</param>
        /// <param name="enabled">Whether this choice is enabled.</param>
        /// <param name="color">The text color.</param>
        public MenuTextChoice(string label, string text, Action choiceAction, bool enabled, Color color)
            : base(label, choiceAction, enabled)
        {
            Text = new MenuText(text, new Loc(2, 1), color);
        }

        /// <inheritdoc/>
        protected override IEnumerable<IMenuElement> GetDrawElements()
        {
            yield return Text;
        }
    }

    /// <summary>
    /// A menu choice that can contain multiple custom elements.
    /// </summary>
    public class MenuElementChoice : MenuChoice
    {
        /// <summary>
        /// The collection of elements displayed for this choice.
        /// </summary>
        public List<IMenuElement> Elements;

        /// <summary>
        /// Initializes a new <see cref="MenuElementChoice"/> with the specified elements.
        /// </summary>
        /// <param name="choiceAction">The action to execute when selected.</param>
        /// <param name="enabled">Whether this choice is enabled.</param>
        /// <param name="elements">The elements to display.</param>
        public MenuElementChoice(Action choiceAction, bool enabled, params IMenuElement[] elements) : this("", choiceAction, enabled, elements) { }

        /// <summary>
        /// Initializes a new labeled <see cref="MenuElementChoice"/> with the specified elements.
        /// </summary>
        /// <param name="label">The identifier label for this choice.</param>
        /// <param name="choiceAction">The action to execute when selected.</param>
        /// <param name="enabled">Whether this choice is enabled.</param>
        /// <param name="elements">The elements to display.</param>
        public MenuElementChoice(string label, Action choiceAction, bool enabled, params IMenuElement[] elements) : base(label, choiceAction, enabled)
        {
            ChoiceAction = choiceAction;
            Enabled = enabled;

            Elements = new List<IMenuElement>();
            foreach (IMenuElement element in elements)
                Elements.Add(element);
        }

        /// <inheritdoc/>
        protected override IEnumerable<IMenuElement> GetDrawElements()
        {
            foreach (IMenuElement element in Elements)
                yield return element;
        }

        /// <summary>
        /// Gets the index of an element by its label.
        /// </summary>
        /// <param name="label">The label to search for.</param>
        /// <returns>The index of the element, or -1 if not found.</returns>
        public int GetElementIndexByLabel(string label)
        {
            return GetElementIndicesByLabel(label)[label];
        }

        /// <summary>
        /// Gets the indices of multiple elements by their labels.
        /// </summary>
        /// <param name="labels">The labels to search for.</param>
        /// <returns>A dictionary mapping labels to their element indices.</returns>
        public virtual Dictionary<string, int> GetElementIndicesByLabel(params string[] labels)
        {
            return MenuBase.SearchLabels(labels, Elements);
        }
    }
}
