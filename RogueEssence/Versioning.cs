using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RogueEssence
{
    /// <summary>
    /// Provides utilities for retrieving version and runtime information.
    /// </summary>
    public static class Versioning
    {
        /// <summary>
        /// Gets the version of the entry assembly (the game).
        /// </summary>
        /// <returns>The version of the game.</returns>
        public static Version GetVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version;
        }

        /// <summary>
        /// Gets information about the .NET runtime.
        /// </summary>
        /// <returns>A string describing the current .NET framework.</returns>
        public static string GetDotNetInfo()
        {
            return RuntimeInformation.FrameworkDescription;
        }
    }
}
