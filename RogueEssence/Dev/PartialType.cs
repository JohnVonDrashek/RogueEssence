using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Represents a partially specified type with generic arguments and search assemblies.
    /// Used for type resolution when working with generic types in the editor.
    /// </summary>
    public class PartialType
    {
        /// <summary>
        /// The base type being represented.
        /// </summary>
        public Type Type;

        /// <summary>
        /// The assemblies to search when resolving derived types.
        /// </summary>
        public Assembly[] SearchAssemblies;

        /// <summary>
        /// The generic type arguments for this partial type.
        /// </summary>
        public Type[] GenericArgs;

        /// <summary>
        /// Initializes a new instance of the PartialType class.
        /// </summary>
        /// <param name="baseType">The base type to represent.</param>
        /// <param name="searchAssemblies">The assemblies to search for derived types.</param>
        /// <param name="genericArgs">The generic type arguments.</param>
        public PartialType(Type baseType, Assembly[] searchAssemblies, params Type[] genericArgs)
        {
            Type = baseType;
            SearchAssemblies = searchAssemblies;
            GenericArgs = genericArgs;
        }

        /// <summary>
        /// Returns a string representation of the type.
        /// </summary>
        /// <returns>The string representation of the base type.</returns>
        public override string ToString()
        {
            return Type.ToString();
        }
    }
}
