namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents a reference to an inventory slot, indicating whether it's an equipped item or bag item.
    /// </summary>
    public struct InvSlot
    {
        /// <summary>
        /// The slot index within the inventory or equipment.
        /// </summary>
        public int Slot;

        /// <summary>
        /// Whether this slot refers to an equipped item rather than a bag item.
        /// </summary>
        public bool IsEquipped;

        /// <summary>
        /// Initializes a new InvSlot with the specified type and index.
        /// </summary>
        /// <param name="isEquipped">True if the item is equipped, false if in bag.</param>
        /// <param name="slot">The slot index.</param>
        public InvSlot(bool isEquipped, int slot)
        {
            IsEquipped = isEquipped;
            Slot = slot;
        }




        private static readonly InvSlot invalid = new InvSlot(false, -1);

        /// <summary>
        /// Gets an invalid InvSlot instance with slot index -1.
        /// </summary>
        public static InvSlot Invalid { get { return invalid; } }

        /// <summary>
        /// Determines whether this InvSlot represents a valid slot.
        /// </summary>
        /// <returns>True if the slot index is greater than -1; otherwise, false.</returns>
        public bool IsValid()
        {
            return (Slot > -1);
        }
    }
}