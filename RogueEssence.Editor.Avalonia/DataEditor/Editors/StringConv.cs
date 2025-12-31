using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RogueEssence.Dev.ViewModels;
using RogueEssence.Dev.Views;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;

namespace RogueEssence.Dev
{
    /// <summary>
    /// A string converter used in conjunction with class editors that contain other elements inside.
    /// Converts objects to their string representation for display purposes.
    /// </summary>
    public class StringConv
    {
        /// <summary>
        /// The type of object this converter handles.
        /// </summary>
        public Type ObjectType;

        /// <summary>
        /// The attributes associated with the member being converted.
        /// </summary>
        public object[] Attributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringConv"/> class with default values.
        /// </summary>
        public StringConv()
        {
            ObjectType = typeof(object);
            Attributes = new object[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringConv"/> class with specified type and attributes.
        /// </summary>
        /// <param name="type">The type of object to convert.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        public StringConv(Type type, object[] attributes)
        {
            ObjectType = type;
            Attributes = attributes;
        }

        /// <summary>
        /// Converts an object to its string representation using the DataEditor.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <returns>The string representation of the object.</returns>
        public string GetString(object obj)
        {
            return DataEditor.GetString(obj, obj.GetType(), Attributes);
        }
    }
}
