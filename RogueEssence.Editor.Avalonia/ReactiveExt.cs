using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia.Controls;
using ReactiveUI;
using RogueEssence.Dev.Views;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Extension methods for ReactiveUI objects and controls.
    /// </summary>
    public static class ReactiveExt
    {
        /// <summary>
        /// Sets a backing field if the new value is different from the current value, raising property changed.
        /// </summary>
        /// <typeparam name="TObj">The reactive object type.</typeparam>
        /// <typeparam name="TRet">The property type.</typeparam>
        /// <param name="reactiveObject">The reactive object.</param>
        /// <param name="backingField">Reference to the backing field.</param>
        /// <param name="newValue">The new value to set.</param>
        /// <param name="propertyName">The property name (auto-populated by compiler).</param>
        /// <returns>True if the value was changed, otherwise false.</returns>
        public static bool SetIfChanged<TObj, TRet>(
            this TObj reactiveObject,
            ref TRet backingField,
            TRet newValue,
            [CallerMemberName] string propertyName = null)
            where TObj : IReactiveObject
        {

            if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
            {
                return false;
            }
            reactiveObject.RaiseAndSetIfChanged(ref backingField, newValue, propertyName);
            return true;
        }

        /// <summary>
        /// Sets a backing field and raises property changed, regardless of whether the value changed.
        /// </summary>
        /// <typeparam name="TObj">The reactive object type.</typeparam>
        /// <typeparam name="TRet">The property type.</typeparam>
        /// <param name="reactiveObject">The reactive object.</param>
        /// <param name="backingField">Reference to the backing field.</param>
        /// <param name="newValue">The new value to set.</param>
        /// <param name="propertyName">The property name (auto-populated by compiler).</param>
        public static void RaiseAndSet<TObj, TRet>(
            this TObj reactiveObject,
            ref TRet backingField,
            TRet newValue,
            [CallerMemberName] string propertyName = null)
            where TObj : IReactiveObject
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            backingField = newValue;
            reactiveObject.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Gets the owning parent form for a control.
        /// </summary>
        /// <param name="control">The control to find the parent form for.</param>
        /// <returns>The ParentForm that owns this control.</returns>
        public static ParentForm GetOwningForm(this IControl control)
        {
            while (control.Parent != null)
                control = control.Parent;
            return (ParentForm)control;
        }
    }
}
