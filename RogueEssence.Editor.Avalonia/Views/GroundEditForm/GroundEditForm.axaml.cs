using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.IO;
using RogueEssence;
using RogueEssence.Dev;
using Microsoft.Xna.Framework;
using Avalonia.Threading;
using System.Threading;
using RogueEssence.Data;
using RogueEssence.Content;
using System.Collections.Generic;
using RogueEssence.Dev.ViewModels;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// Ground map editor form for editing ground/hub maps in RogueEssence.
    /// Implements IGroundEditor to provide ground editing functionality with undo/redo support.
    /// </summary>
    public class GroundEditForm : ParentForm, IGroundEditor
    {
        /// <summary>
        /// Gets whether the editor form is currently active.
        /// </summary>
        public bool Active { get; private set; }

        /// <summary>
        /// Gets the undo/redo stack for ground editing operations.
        /// </summary>
        // TODO: make undo/redo enabled/disabled based on if there's anything left on the stack
        public UndoStack Edits { get; }

        /// <summary>
        /// Initializes a new instance of the GroundEditForm class.
        /// </summary>
        public GroundEditForm()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            Edits = new UndoStack();

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


        /// <summary>
        /// Processes user input for the ground editor.
        /// </summary>
        /// <param name="input">The input manager containing current input state.</param>
        public void ProcessInput(InputManager input)
        {
            DevForm.ExecuteOrInvoke(() => ((GroundEditViewModel)DataContext).ProcessInput(input));
        }




        /// <summary>
        /// Handles the window loaded event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void Window_Loaded(object sender, EventArgs e)
        {
            Active = true;
        }

        private bool silentClose;

        /// <summary>
        /// Closes the form without triggering scene transition logic.
        /// </summary>
        public void SilentClose()
        {
            silentClose = true;
            Close();
        }

        /// <summary>
        /// Handles the window closed event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void Window_Closed(object sender, EventArgs e)
        {
            Active = false;
            CloseChildren();
            if (!silentClose)
                GameManager.Instance.SceneOutcome = exitGroundEdit();
        }


        private IEnumerator<YieldInstruction> exitGroundEdit()
        {
            DevForm form = (DevForm)DiagManager.Instance.DevEditor;
            form.GroundEditForm = null;

            //move to the previous scene or the title, if there was none
            if (DataManager.Instance.Save != null && DataManager.Instance.Save.NextDest.IsValid())
                yield return CoroutineManager.Instance.StartCoroutine(GameManager.Instance.MoveToZone(DataManager.Instance.Save.NextDest, true, false));
            else
                yield return CoroutineManager.Instance.StartCoroutine(GameManager.Instance.RestartToTitle());
        }
    }
}
