using System;
using System.Collections.Generic;
using System.Text;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Defines the available tile editing modes in the map editor.
    /// </summary>
    public enum TileEditMode
    {
        /// <summary>Draw individual tiles.</summary>
        Draw = 0,
        /// <summary>Draw rectangular regions of tiles.</summary>
        Rectangle = 1,
        /// <summary>Flood fill with tiles.</summary>
        Fill = 2,
        /// <summary>Pick up tile information from the map.</summary>
        Eyedrop = 3,
    }

    /// <summary>
    /// Defines the available entity editing modes in the map editor.
    /// </summary>
    public enum EntEditMode
    {
        /// <summary>Select and modify existing entities.</summary>
        SelectEntity = 0,
        /// <summary>Place new entities on the map.</summary>
        PlaceEntity = 1,
    }

}
