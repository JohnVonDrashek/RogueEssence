using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace RogueEssence.Dev.ViewModels
{
    /// <summary>
    /// Base class for all view models in the RogueEssence editor.
    /// Inherits from ReactiveObject to provide property change notification support for Avalonia UI data binding.
    /// </summary>
    public class ViewModelBase : ReactiveObject
    {
    }
}
