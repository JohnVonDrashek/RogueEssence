using System;
using Newtonsoft.Json;
using RogueEssence.Data;
using RogueEssence.Dev;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Identifies a specific monster form including species, form variant, skin, and gender.
    /// Used to uniquely identify character appearances and base stats.
    /// </summary>
    [Serializable]
    public struct MonsterID
    {
        /// <summary>
        /// The species identifier for the monster.
        /// </summary>
        [JsonConverter(typeof(MonsterConverter))]
        public string Species;

        /// <summary>
        /// The form variant index (e.g., alternate forms of the same species).
        /// </summary>
        public int Form;

        /// <summary>
        /// The skin identifier for visual variants (shiny, shadow, etc.).
        /// </summary>
        [JsonConverter(typeof(SkinConverter))]
        public string Skin;

        /// <summary>
        /// The gender of the monster.
        /// </summary>
        public Gender Gender;

        /// <summary>
        /// Initializes a new MonsterID with all identification properties.
        /// </summary>
        /// <param name="species">The species identifier.</param>
        /// <param name="form">The form variant index.</param>
        /// <param name="skin">The skin identifier.</param>
        /// <param name="gender">The gender.</param>
        public MonsterID(string species, int form, string skin, Gender gender)
        {
            Species = species;
            Form = form;
            Skin = skin;
            Gender = gender;
        }

        /// <summary>
        /// An invalid MonsterID instance.
        /// </summary>
        public static readonly MonsterID Invalid = new MonsterID("", -1, "", Gender.Unknown);

        /// <summary>
        /// Determines whether this MonsterID represents a valid monster.
        /// </summary>
        /// <returns>True if the species is not null or empty; otherwise, false.</returns>
        public bool IsValid()
        {
            return !String.IsNullOrEmpty(Species);
        }

        /// <summary>
        /// Determines whether this MonsterID equals another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the object is a MonsterID with the same values; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is MonsterID) && Equals((MonsterID)obj);
        }

        /// <summary>
        /// Determines whether this MonsterID equals another MonsterID.
        /// </summary>
        /// <param name="other">The MonsterID to compare.</param>
        /// <returns>True if all properties match; otherwise, false.</returns>
        public bool Equals(MonsterID other)
        {
            if (Species != other.Species)
                return false;
            if (Form != other.Form)
                return false;
            if (Skin != other.Skin)
                return false;
            if (Gender != other.Gender)
                return false;

            return true;
        }

        /// <summary>
        /// Gets the hash code for this MonsterID.
        /// </summary>
        /// <returns>A hash code based on all properties.</returns>
        public override int GetHashCode()
        {
            return Species.GetHashCode() ^ Form.GetHashCode() ^ Skin.GetHashCode() ^ Gender.GetHashCode();
        }

        /// <summary>
        /// Tests equality between two MonsterID values.
        /// </summary>
        public static bool operator ==(MonsterID value1, MonsterID value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Tests inequality between two MonsterID values.
        /// </summary>
        public static bool operator !=(MonsterID value1, MonsterID value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// Converts this MonsterID to a CharID for graphics lookup.
        /// </summary>
        /// <returns>A CharID with numeric indices for the graphics system.</returns>
        public Content.CharID ToCharID()
        {
            int mon = DataManager.Instance.DataIndices[DataManager.DataType.Monster].Get(Species).SortOrder;
            int skin = DataManager.Instance.DataIndices[DataManager.DataType.Skin].Get(Skin).SortOrder;
            return new Content.CharID(mon, Form, skin, (int)Gender);
        }

        /// <summary>
        /// Returns a string representation of this MonsterID.
        /// </summary>
        /// <returns>A string containing all identification properties.</returns>
        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3}", Species, Form, Skin, Gender);
        }
    }
}
