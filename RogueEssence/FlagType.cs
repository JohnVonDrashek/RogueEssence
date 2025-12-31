using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace RogueEssence
{
    /// <summary>
    /// Represents a serializable wrapper for a Type that can be persisted and reconstructed.
    /// Stores assembly and type name information for later resolution.
    /// </summary>
    [Serializable]
    public class FlagType
    {
        [NonSerialized]
        private Type fullType;

        /// <summary>
        /// Gets the resolved Type object.
        /// </summary>
        public Type FullType => fullType;

        private string assembly;
        private string type;

        /// <summary>
        /// Initializes a new instance of the FlagType class with System.Object as the default type.
        /// </summary>
        public FlagType()
        {
            fullType = typeof(object);
            assembly = fullType.Assembly.FullName;
            this.type = fullType.FullName;
        }

        /// <summary>
        /// Initializes a new instance of the FlagType class with the specified type.
        /// </summary>
        /// <param name="type">The type to wrap.</param>
        public FlagType(Type type)
        {
            fullType = type;
            this.assembly = type.Assembly.FullName;
            this.type = type.FullName;
        }

        /// <summary>
        /// Called after deserialization to reconstruct the Type object from stored assembly and type names.
        /// </summary>
        /// <param name="context">The streaming context for deserialization.</param>
        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (type != null)
            {
                fullType = Type.GetType(String.Format("{0}, {1}", type, assembly), versionlessResolve, null);
                if (fullType == null)
                {
                    throw new TypeInitializationException(type, null);
                }
            }
        }

        /// <summary>
        /// Resolves an assembly without version constraints.
        /// </summary>
        /// <param name="name">The assembly name to resolve.</param>
        /// <returns>The loaded assembly.</returns>
        private static Assembly versionlessResolve(AssemblyName name)
        {
            name.Version = null;
            return Assembly.Load(name);
        }

        /// <summary>
        /// Returns the full type name as a string.
        /// </summary>
        /// <returns>The full type name.</returns>
        public override string ToString()
        {
            return type;
        }

        /// <summary>
        /// Returns a hash code based on assembly and type names.
        /// </summary>
        /// <returns>A hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return (assembly == null ? 0 : assembly.GetHashCode()) ^ (type == null ? 0 : type.GetHashCode());
        }
    }
}
