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
using System.Threading.Tasks;
using System.Transactions;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// A form for editing data objects with OK and Cancel functionality.
    /// Provides a container panel for dynamically loaded editor controls.
    /// </summary>
    public class DataEditForm : ParentForm
    {
        /// <summary>
        /// Delegate for handling the OK button event.
        /// </summary>
        /// <returns>True if the form should close, false otherwise.</returns>
        public delegate Task<bool> OKEvent();

        /// <summary>
        /// Event handler that is invoked when the OK button is clicked.
        /// </summary>
        public OKEvent SelectedOKEvent;

        //public event Action SelectedCancelEvent;

        /// <summary>
        /// Gets the panel that contains the editor controls.
        /// </summary>
        public StackPanel ControlPanel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataEditForm"/> class.
        /// </summary>
        public DataEditForm()
        {
            InitializeComponent();

            ControlPanel = this.FindControl<StackPanel>("stkContent");
            
#if DEBUG
            this.AttachDevTools();
#endif
        }

        /// <summary>
        /// Loads the XAML component for this form.
        /// </summary>
        protected virtual void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Recursively saves all child editor forms.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SaveChildren()
        {
            for (int ii = children.Count - 1; ii >= 0; ii--)
            {
                DataEditForm dataEditor = children[ii] as DataEditForm;
                if (dataEditor != null)
                {
                    await dataEditor.SaveChildren();
                    if (dataEditor.SelectedOKEvent != null)
                        await dataEditor.SelectedOKEvent.Invoke();
                }
            }
        }

        /// <summary>
        /// Handles the window loaded event. Adjusts window size as a workaround for text wrapping bug.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        //TODO: this is a workaround to a bug in text wrapping
        //the window size must be modified in order to invalidate a cached value for width
        //remove this once the bug is fixed
        public void Window_Loaded(object sender, EventArgs e)
        {
            if (Design.IsDesignMode)
                return;
            this.Width = this.Width + 10;
        }

        /// <summary>
        /// Handles the window closing event. Prompts for confirmation if there are unsaved child windows.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The cancel event arguments.</param>
        public virtual async void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Design.IsDesignMode)
                return;
            
            if (!Cancel)
            {
                if (!OK && children.Count > 0)
                {
                    //X button was clicked when there are children, cancel the close, popup the children, and add a warning message.
                    e.Cancel = true;
                    FocusChildren();
                    Task<MessageBox.MessageBoxResult> task =  MessageBox.Show(this, "Are you sure you want to close all subwindows?  Your changes will not be saved.", "Confirm Close",
                        MessageBox.MessageBoxButtons.YesNo);
                    MessageBox.MessageBoxResult result = await task;
                    if (result == MessageBox.MessageBoxResult.Yes)
                    {
                        Cancel = true;
                        Close();
                        return;
                    }
                }
            }
            
            if (!e.Cancel)
                CloseChildren();
        }

        /// <summary>
        /// Handles the OK button click event. Saves children and invokes the OK event handler.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The routed event arguments.</param>
        public async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            bool close = false;
            if (SelectedOKEvent != null)
            {
                await SaveChildren();
                close = await SelectedOKEvent.Invoke();
            }
            if (close)
            {
                OK = true;
                Close();
            }
        }

        /// <summary>
        /// Handles the Cancel button click event. Closes the form without saving.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The routed event arguments.</param>
        public void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //SelectedCancelEvent?.Invoke();
            Cancel = true;
            Close();
        }

        /// <summary>
        /// Sets the form to view-only mode by disabling the OK button.
        /// </summary>
        public void SetViewOnly()
        {
            Button button = this.FindControl<Button>("btnOK");
            button.IsEnabled = false;
        }

    }
}
