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
    /// Dungeon map editor form for editing dungeon/floor maps in RogueEssence.
    /// Implements IMapEditor to provide map editing functionality with undo/redo support.
    /// </summary>
    public class MapEditForm : ParentForm, IMapEditor
    {
        /// <summary>
        /// Gets whether the editor form is currently active.
        /// </summary>
        public bool Active { get; private set; }

        /// <summary>
        /// Gets the undo/redo stack for map editing operations.
        /// </summary>
        public UndoStack Edits { get; }

        /// <summary>
        /// Initializes a new instance of the MapEditForm class.
        /// </summary>
        public MapEditForm()
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
        /// Processes user input for the map editor.
        /// </summary>
        /// <param name="input">The input manager containing current input state.</param>
        public void ProcessInput(InputManager input)
        {
            DevForm.ExecuteOrInvoke(() => ((MapEditViewModel)DataContext).ProcessInput(input));
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
                GameManager.Instance.SceneOutcome = exitMapEdit();
        }


        private IEnumerator<YieldInstruction> exitMapEdit()
        {
            DevForm form = (DevForm)DiagManager.Instance.DevEditor;
            form.MapEditForm = null;

            //move to the previous scene or the title, if there was none
            if (DataManager.Instance.Save != null && DataManager.Instance.Save.NextDest.IsValid())
                yield return CoroutineManager.Instance.StartCoroutine(GameManager.Instance.MoveToZone(DataManager.Instance.Save.NextDest, true, false));
            else
                yield return CoroutineManager.Instance.StartCoroutine(GameManager.Instance.RestartToTitle());
        }
    }
}
