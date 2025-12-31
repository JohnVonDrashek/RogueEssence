using System;
using Newtonsoft.Json;
using RogueEssence.Data;
using RogueEssence.Dev;

namespace RogueEssence.Content
{
    /// <summary>
    /// Uniquely identifies a character sprite by its species, form, skin, and gender.
    /// Used as a key for loading and caching character sprite sheets.
    /// </summary>
    public struct CharID
    {
        /// <summary>
        /// The species ID of the character.
        /// </summary>
        public int Species;

        /// <summary>
        /// The form variant of the character (e.g., alternate forms).
        /// </summary>
        public int Form;

        /// <summary>
        /// The skin/color variant of the character.
        /// </summary>
        public int Skin;

        /// <summary>
        /// The gender variant of the character.
        /// </summary>
        public int Gender;

        /// <summary>
        /// Creates a new CharID with the specified identifiers.
        /// </summary>
        /// <param name="species">The species ID.</param>
        /// <param name="form">The form variant.</param>
        /// <param name="skin">The skin variant.</param>
        /// <param name="gender">The gender variant.</param>
        public CharID(int species, int form, int skin, int gender)
        {
            Species = species;
            Form = form;
            Skin = skin;
            Gender = gender;
        }

        /// <summary>
        /// Represents an invalid CharID with all fields set to -1.
        /// </summary>
        public static readonly CharID Invalid = new CharID(-1, -1, -1, -1);

        /// <summary>
        /// Checks if this CharID represents a valid character.
        /// </summary>
        /// <returns>True if the Species is greater than -1, false otherwise.</returns>
        public bool IsValid()
        {
            return (Species > -1);
        }

        /// <summary>
        /// Returns a string representation of this CharID.
        /// </summary>
        /// <returns>A space-separated string of Species, Form, Skin, and Gender.</returns>
        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3}", Species, Form, Skin, Gender);
        }

        /// <summary>
        /// Determines whether the specified object equals this CharID.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the object is a CharID with matching fields.</returns>
        public override bool Equals(object obj)
        {
            return (obj is CharID) && Equals((CharID)obj);
        }

        /// <summary>
        /// Determines whether this CharID equals another CharID.
        /// </summary>
        /// <param name="other">The CharID to compare.</param>
        /// <returns>True if all fields match.</returns>
        public bool Equals(CharID other)
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
        /// Returns a hash code for this CharID.
        /// </summary>
        /// <returns>A hash code combining all field values.</returns>
        public override int GetHashCode()
        {
            return Species.GetHashCode() ^ Form.GetHashCode() ^ Skin.GetHashCode() ^ Gender.GetHashCode();
        }

        public static bool operator ==(CharID value1, CharID value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(CharID value1, CharID value2)
        {
            return !(value1 == value2);
        }
    }
}
