using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// A modal message box dialog for displaying messages to the user.
    /// Supports various button configurations including OK, OK/Cancel, Yes/No, and Yes/No/Cancel.
    /// </summary>
    public class MessageBox : Window
    {
        /// <summary>
        /// Defines the available button configurations for the message box.
        /// </summary>
        public enum MessageBoxButtons
        {
            Ok,
            OkCancel,
            YesNo,
            YesNoCancel
        }

        /// <summary>
        /// Defines the possible results from a message box interaction.
        /// </summary>
        public enum MessageBoxResult
        {
            /// <summary>The user clicked OK.</summary>
            Ok,
            /// <summary>The user clicked Cancel.</summary>
            Cancel,
            /// <summary>The user clicked Yes.</summary>
            Yes,
            /// <summary>The user clicked No.</summary>
            No
        }

        /// <summary>
        /// Initializes a new instance of the MessageBox class.
        /// </summary>
        public MessageBox()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Shows a message box dialog with the specified options.
        /// </summary>
        /// <param name="parent">The parent window to show the dialog on.</param>
        /// <param name="text">The message text to display.</param>
        /// <param name="title">The dialog window title.</param>
        /// <param name="buttons">The button configuration to use.</param>
        /// <returns>A task that resolves to the user's choice.</returns>
        // https://stackoverflow.com/questions/55706291/how-to-show-a-message-box-in-avaloniaui-beta
        public static Task<MessageBoxResult> Show(Window parent, string text, string title, MessageBoxButtons buttons)
        {
            var msgbox = new MessageBox()
            {
                Title = title
            };
            msgbox.FindControl<TextBlock>("Text").Text = text;
            var buttonPanel = msgbox.FindControl<StackPanel>("Buttons");

            var res = MessageBoxResult.Ok;

            void AddButton(string caption, MessageBoxResult r, bool def = false)
            {
                var btn = new Button { Content = caption };
                btn.Width = 80;
                btn.Click += (_, __) => {
                    res = r;
                    msgbox.Close();
                };
                buttonPanel.Children.Add(btn);
                if (def)
                    res = r;
            }

            if (buttons == MessageBoxButtons.Ok || buttons == MessageBoxButtons.OkCancel)
                AddButton("Ok", MessageBoxResult.Ok, true);
            if (buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.YesNoCancel)
            {
                AddButton("Yes", MessageBoxResult.Yes);
                AddButton("No", MessageBoxResult.No, true);
            }

            if (buttons == MessageBoxButtons.OkCancel || buttons == MessageBoxButtons.YesNoCancel)
                AddButton("Cancel", MessageBoxResult.Cancel, true);


            var tcs = new TaskCompletionSource<MessageBoxResult>();
            msgbox.Closed += delegate { tcs.TrySetResult(res); };
            if (parent != null)
                msgbox.ShowDialog(parent);
            else msgbox.Show();
            return tcs.Task;
        }


    }

}