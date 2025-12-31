using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using RogueEssence.Dev.ViewModels;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Locates and creates views based on view model types by convention.
    /// Replaces "ViewModel" with "View" in the type name to find the corresponding view.
    /// </summary>
    public class ViewLocator : IDataTemplate
    {
        /// <summary>
        /// Gets whether this template supports recycling. Always returns false.
        /// </summary>
        public bool SupportsRecycling => false;

        /// <summary>
        /// Builds a view for the given data object (view model).
        /// </summary>
        /// <param name="data">The view model to create a view for.</param>
        /// <returns>The created view control, or a TextBlock with an error message if the view type is not found.</returns>
        public IControl Build(object data)
        {
            var name = data.GetType().FullName.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type);
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        /// <summary>
        /// Checks if this template can handle the given data object.
        /// </summary>
        /// <param name="data">The data object to check.</param>
        /// <returns>True if data is a ViewModelBase, otherwise false.</returns>
        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}