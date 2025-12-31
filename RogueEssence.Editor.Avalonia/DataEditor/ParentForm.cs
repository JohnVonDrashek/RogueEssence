using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using RogueEssence;
using RogueEssence.Dev;
using Microsoft.Xna.Framework;
using Avalonia.Threading;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// Base class for forms that can have child windows. Provides child window management functionality.
    /// </summary>
    public class ParentForm : Window
    {
        /// <summary>
        /// List of child windows registered with this parent form.
        /// </summary>
        protected List<Window> children;

        /// <summary>
        /// Indicates whether the form was confirmed with OK.
        /// </summary>
        protected bool OK;

        /// <summary>
        /// Indicates whether the form was cancelled.
        /// </summary>
        protected bool Cancel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParentForm"/> class.
        /// </summary>
        public ParentForm()
        {
            children = new List<Window>();
        }

        /// <summary>
        /// Registers a child window with this parent form.
        /// </summary>
        /// <param name="child">The child window to register.</param>
        public void RegisterChild(Window child)
        {
            children.Add(child);
            child.Closed += (object sender, EventArgs e) =>
            {
                children.Remove(child);
            };
        }

        /// <summary>
        /// Recursively brings all child windows to focus.
        /// </summary>
        public void FocusChildren()
        {
            for (int ii = children.Count - 1; ii >= 0; ii--)
            {
                children[ii].Activate();
                ParentForm dataEditor = children[ii] as ParentForm;
                if (dataEditor != null)
                {
                    dataEditor.FocusChildren();
                }
            }
        }

        /// <summary>
        /// Closes all child windows, propagating the OK/Cancel state.
        /// </summary>
        public void CloseChildren()
        {
            for (int ii = children.Count - 1; ii >= 0; ii--)
            {
                ParentForm dataEditor = children[ii] as ParentForm;
                if (dataEditor != null)
                {
                    dataEditor.OK = this.OK;
                    dataEditor.Cancel = this.Cancel;
                }
                children[ii].Close();
            }
                
        }

    }
}
