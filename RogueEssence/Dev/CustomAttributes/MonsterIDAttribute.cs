using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Specifies validation options for MonsterID fields in the editor.
    /// Controls which components (species, form, skin, gender) can be set to invalid/default values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MonsterIDAttribute : PassableAttribute
    {
        public readonly bool InvalidSpecies;
        public readonly bool InvalidForm;
        public readonly bool InvalidSkin;
        public readonly bool InvalidGender;

        public MonsterIDAttribute(int flags, bool invalidSpecies, bool invalidForm, bool invalidSkin, bool invalidGender) : base(flags)
        {
            InvalidSpecies = invalidSpecies;
            InvalidForm = invalidForm;
            InvalidSkin = invalidSkin;
            InvalidGender = invalidGender;
        }
    }
}
