namespace RogueEssence.Data
{
    /// <summary>
    /// Extends IEntryData to include a description field for assets that have player-facing descriptions.
    /// </summary>
    public interface IDescribedData : IEntryData
    {
        /// <summary>
        /// The description of the asset as the player sees it, including translations.
        /// </summary>
        LocalText Desc { get; set; }
    }

    /// <summary>
    /// All classes that represent indexed data such as monsters, items, etc. implement this class.
    /// This is used for editor lists that need to load the names of all monsters, items, etc. without actually loading all data files.
    /// Also for localization.
    /// </summary>
    public interface IEntryData
    {
        /// <summary>
        /// The name of the asset as the player sees it, including translations.
        /// </summary>
        LocalText Name { get; set; }

        /// <summary>
        /// If released, this asset can be found/accessed in the game.
        /// </summary>
        bool Released { get; }

        /// <summary>
        /// Developer-only comments for this asset.
        /// </summary>
        string Comment { get; set; }

        /// <summary>
        /// Gets the display name with color formatting for UI display.
        /// </summary>
        /// <returns>The formatted name string with color tags.</returns>
        string GetColoredName();

        /// <summary>
        /// Generates a summary of this entry for indexing purposes.
        /// </summary>
        /// <returns>An EntrySummary containing the entry's metadata.</returns>
        EntrySummary GenerateEntrySummary();
    }
}
