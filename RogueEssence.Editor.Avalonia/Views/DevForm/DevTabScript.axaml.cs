using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using RogueEssence.Dev.ViewModels;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Script tab in the developer form.
    /// Provides a Lua console interface for executing scripts during development.
    /// </summary>
    public class DevTabScript : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the DevTabScript class.
        /// </summary>
        public DevTabScript()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


        /// <summary>
        /// Handles key down events in the script input text box.
        /// Supports Shift+Enter for new lines, Shift/Ctrl+Up/Down for history navigation, and Enter to execute.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The key event arguments.</param>
        public void txtScriptInput_KeyDown(object sender, KeyEventArgs args)
        {
            if ((args.KeyModifiers & KeyModifiers.Shift) != KeyModifiers.None || (args.KeyModifiers & KeyModifiers.Control) != KeyModifiers.None)
            {
                DevTabScriptViewModel viewModel = (DevTabScriptViewModel)DataContext;

                switch (args.Key)
                {
                    case Key.Enter:
                        {
                            viewModel.ScriptLine = viewModel.ScriptLine + "\n";
                            viewModel.CmdCaret = viewModel.CmdCaret + 1;
                            break;
                        }
                    case Key.Up:
                        {
                            viewModel.ShiftHistory(1);
                            args.Handled = true;
                            break;
                        }
                    case Key.Down:
                        {
                            viewModel.ShiftHistory(-1);
                            args.Handled = true;
                            break;
                        }
                }
            }
            else if (args.Key == Key.Enter)
            {
                DevTabScriptViewModel viewModel = (DevTabScriptViewModel)DataContext;
                viewModel.SendScript();

            }
        }

    }
}
