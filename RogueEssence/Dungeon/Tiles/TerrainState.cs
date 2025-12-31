using System;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Abstract base class for terrain states that affect gameplay behavior.
    /// Terrain states modify how tiles interact with characters and effects.
    /// </summary>
    [Serializable]
    public abstract class TerrainState : GameplayState
    {

    }

}
