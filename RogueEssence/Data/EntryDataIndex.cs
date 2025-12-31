using System;
using System.Collections.Generic;

namespace RogueEssence.Data
{
    /// <summary>
    /// An index structure that maps entry IDs to their summary data, supporting mod layering.
    /// Each entry can have multiple versions from different mods, with the most recent at the top.
    /// </summary>
    [Serializable]
    public class EntryDataIndex
    {
        /// <summary>
        /// Gets the total number of unique entries in the index.
        /// </summary>
        public int Count { get { return entries.Count; } }

        //TODO: add the modding status of the entry: diff-modded, or not?
        private Dictionary<string, List<(Guid, EntrySummary)>> entries;

        /// <summary>
        /// Initializes a new empty EntryDataIndex.
        /// </summary>
        public EntryDataIndex()
        {
            entries = new Dictionary<string, List<(Guid, EntrySummary)>>();
        }

        /// <summary>
        /// Sets the internal entries dictionary directly.
        /// </summary>
        /// <param name="entries">The entries dictionary to use.</param>
        public void SetEntries(Dictionary<string, List<(Guid, EntrySummary)>> entries)
        {
            this.entries = entries;
        }

        /// <summary>
        /// Gets all entries without their mod GUIDs, returning only the top-priority version of each entry.
        /// </summary>
        /// <returns>A dictionary mapping entry IDs to their summaries.</returns>
        public Dictionary<string, EntrySummary> GetEntriesWithoutGuid()
        {
            Dictionary<string, EntrySummary> result = new Dictionary<string, EntrySummary>();
            foreach (string key in entries.Keys)
            {
                result[key] = entries[key][0].Item2;
            }
            return result;
        }

        /// <summary>
        /// Gets the entry summary for the specified index. Supports namespace:id format for specific mod versions.
        /// </summary>
        /// <param name="index">The entry ID, optionally prefixed with "namespace:" for mod-specific lookup.</param>
        /// <returns>The EntrySummary for the requested entry.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the entry or mod version is not found.</exception>
        public EntrySummary Get(string index)
        {
            string[] components = index.Split(':');
            if (components.Length > 1)
            {
                ModHeader mod = PathMod.GetModFromNamespace(components[0]);
                string asset_id = components[1];

                List<(Guid, EntrySummary)> stack = entries[asset_id];
                foreach ((Guid, EntrySummary) pair in stack)
                {
                    if (pair.Item1 == mod.UUID)
                        return pair.Item2;
                }
                throw new KeyNotFoundException(String.Format("Invalid asset ID: {0}", index));
            }
            else
            {
                string asset_id = components[0];
                List<(Guid, EntrySummary)> stack = entries[asset_id];
                return stack[0].Item2;
            }
        }

        /// <summary>
        /// Iterates through all mod versions of an entry in priority order.
        /// </summary>
        /// <param name="index">The entry ID to iterate.</param>
        /// <returns>An enumerable of (Guid, EntrySummary) tuples for each mod version.</returns>
        public IEnumerable<(Guid, EntrySummary)> IterateKey(string index)
        {
            List<(Guid, EntrySummary)> stack = entries[index];
            foreach ((Guid, EntrySummary) tuple in stack)
                yield return tuple;
        }

        /// <summary>
        /// Sets or updates an entry for a specific mod. Places the entry at the top of the priority stack.
        /// </summary>
        /// <param name="uuid">The mod's unique identifier.</param>
        /// <param name="entryNum">The entry ID.</param>
        /// <param name="entrySummary">The entry summary data.</param>
        public void Set(Guid uuid, string entryNum, EntrySummary entrySummary)
        {
            if (!entries.ContainsKey(entryNum))
                entries[entryNum] = new List<(Guid, EntrySummary)>();
            List<(Guid, EntrySummary)> stack = entries[entryNum];
            for (int ii = 0; ii < stack.Count; ii++)
            {
                if (stack[ii].Item1 == uuid)
                {
                    stack.RemoveAt(ii);
                    break;
                }
            }
            stack.Insert(0, (uuid, entrySummary));
        }

        /// <summary>
        /// Removes a mod's version of an entry from the index.
        /// </summary>
        /// <param name="uuid">The mod's unique identifier.</param>
        /// <param name="entryNum">The entry ID to remove.</param>
        public void Remove(Guid uuid, string entryNum)
        {
            List<(Guid, EntrySummary)> stack = entries[entryNum];
            for (int ii = 0; ii < stack.Count; ii++)
            {
                if (stack[ii].Item1 == uuid)
                {
                    stack.RemoveAt(ii);
                    break;
                }
            }
            if (stack.Count == 0)
                entries.Remove(entryNum);
        }

        /// <summary>
        /// Checks if the index contains an entry with the specified key.
        /// </summary>
        /// <param name="key">The entry ID to check.</param>
        /// <returns>True if the entry exists, false otherwise.</returns>
        public bool ContainsKey(string key)
        {
            return entries.ContainsKey(key);
        }

        /// <summary>
        /// Compares two keys by their sort order, then alphabetically.
        /// </summary>
        /// <param name="key1">The first key to compare.</param>
        /// <param name="key2">The second key to compare.</param>
        /// <returns>A negative value if key1 comes first, positive if key2, zero if equal.</returns>
        public int CompareWithSort(string key1, string key2)
        {
            int cmp = Math.Sign(Get(key1).SortOrder - Get(key2).SortOrder);
            if (cmp != 0)
                return cmp;
            return String.Compare(key1, key2);
        }

        /// <summary>
        /// Maps one index to one key.
        /// </summary>
        /// <returns>List may contain null.</returns>
        public List<string> GetMappedKeys()
        {
            List<string> keys = new List<string>();

            foreach (string key in entries.Keys)
            {
                int idx = Get(key).SortOrder;
                while (idx >= keys.Count)
                    keys.Add(null);
                keys[idx] = key;
            }

            return keys;
        }

        /// <summary>
        /// Keys ordered alphabetically or numerically
        /// </summary>
        /// <param name="numeric"></param>
        /// <returns></returns>
        public List<string> GetOrderedKeys(bool numeric)
        {
            List<string> keys = new List<string>();

            foreach (string key in entries.Keys)
                keys.Add(key);

            if (numeric)
                keys.Sort(CompareWithSort);
            else
                keys.Sort();

            return keys;
        }

        /// <summary>
        /// Gets all entries that belong to a specific mod.
        /// </summary>
        /// <param name="uuid">The mod's unique identifier.</param>
        /// <returns>A dictionary of entry IDs to summaries for the specified mod.</returns>
        public Dictionary<string, EntrySummary> GetModIndex(Guid uuid)
        {
            Dictionary<string, EntrySummary> result = new Dictionary<string, EntrySummary>();
            foreach (string key in entries.Keys)
            {
                List<(Guid, EntrySummary)> stack = entries[key];
                foreach ((Guid, EntrySummary) entry in stack)
                {
                    if (entry.Item1 == uuid)
                        result[key] = entry.Item2;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets a dictionary of all entry IDs to their localized display strings.
        /// </summary>
        /// <param name="verbose">If true, includes developer comments in the output.</param>
        /// <returns>A dictionary mapping entry IDs to their display strings.</returns>
        public Dictionary<string, string> GetLocalStringArray(bool verbose = false)
        {
            Dictionary<string, string> names = new Dictionary<string, string>();

            List<string> curNames = new List<string>();
            foreach (string key in entries.Keys)
                curNames.Add(key);
            curNames.Sort();

            foreach (string name in curNames)
                names[name] = Get(name).GetLocalString(verbose);

            return names;
        }
    }


    /// <summary>
    /// Contains summary information about a data entry for indexing and display purposes.
    /// Used to show entry information without loading the full data file.
    /// </summary>
    [Serializable]
    public class EntrySummary
    {
        /// <summary>
        /// The localized display name of the entry.
        /// </summary>
        public LocalText Name;

        /// <summary>
        /// Indicates whether this entry is released and available in gameplay.
        /// </summary>
        public bool Released;

        /// <summary>
        /// Developer comment describing this entry.
        /// </summary>
        public string Comment;

        /// <summary>
        /// The sort order for this entry in lists and menus.
        /// </summary>
        public int SortOrder;

        /// <summary>
        /// Initializes a new instance of the EntrySummary class with default values.
        /// </summary>
        public EntrySummary()
        {
            Name = new LocalText();
            Comment = "";
        }

        /// <summary>
        /// Initializes a new instance of the EntrySummary class with the specified values.
        /// </summary>
        /// <param name="name">The localized name of the entry.</param>
        /// <param name="released">Whether the entry is released for gameplay.</param>
        /// <param name="comment">Developer comment for this entry.</param>
        /// <param name="sort">The sort order for this entry.</param>
        public EntrySummary(LocalText name, bool released, string comment, int sort = 0)
        {
            Name = name;
            Released = released;
            Comment = comment;
            SortOrder = sort;
        }

        /// <summary>
        /// Gets the display name with green color formatting.
        /// </summary>
        /// <returns>The formatted name string with color tags.</returns>
        public virtual string GetColoredName()
        {
            return String.Format("[color=#00FF00]{0}[color]", Name.ToLocal());
        }

        /// <summary>
        /// Gets the localized string representation of this entry.
        /// </summary>
        /// <param name="verbose">If true, includes developer comments in the output.</param>
        /// <returns>The formatted display string.</returns>
        public string GetLocalString(bool verbose)
        {
            string result = Name.ToLocal();
            if (!Released)
                result = "*" + result;
            if (verbose && Comment != "")
            {
                result += "  #";
                string[] lines = Comment.Split('\n', StringSplitOptions.None);
                result += lines[0];
            }

            return result;
        }
    }


}
