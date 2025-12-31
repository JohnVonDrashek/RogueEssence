using System;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Abstract base class for tile effect states that affect gameplay behavior.
    /// Tile states are attached to effect tiles like traps and wonder tiles.
    /// </summary>
    [Serializable]
    public abstract class TileState : GameplayState
    {

    }

}
