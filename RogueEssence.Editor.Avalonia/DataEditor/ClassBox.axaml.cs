using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using Avalonia.Input;
using RogueEssence.Dev.ViewModels;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// A user control that displays a single class object with an edit button.
    /// Allows editing complex objects through a popup editor form.
    /// </summary>
    public class ClassBox : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassBox"/> class.
        /// Sets up the edit button event handler.
        /// </summary>
        public ClassBox()
        {
            this.InitializeComponent();
            Button button = this.FindControl<Button>("ClassBoxEditButton");
            button.AddHandler(PointerReleasedEvent, ClassBoxEditButton_OnPointerReleased, RoutingStrategies.Tunnel);
        }

        /// <summary>
        /// Loads the XAML component for this control.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Handles the edit button release event. Shift key enables advanced edit mode.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The pointer released event arguments.</param>
        private void ClassBoxEditButton_OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            KeyModifiers modifiers = e.KeyModifiers;
            bool advancedEdit = modifiers.HasFlag(KeyModifiers.Shift);
            ClassBoxViewModel vm = (ClassBoxViewModel) DataContext;
            vm.btnEdit_Click(advancedEdit);
        }
    }
}
