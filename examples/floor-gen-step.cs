// =============================================================================
// EXAMPLE: Custom Floor Generation Step (GenStep)
// =============================================================================
// This file demonstrates how to create custom floor generation steps in
// RogueEssence. GenSteps are used to procedurally generate dungeon floors
// by placing rooms, corridors, items, enemies, and other features.
//
// The floor generation pipeline consists of multiple GenSteps that run
// in sequence. Each step can add, modify, or process elements of the map.
//
// GenSteps operate on a "GenContext" which holds the map being generated
// and various data structures for tracking rooms, items, etc.
// =============================================================================

using System;
using System.Collections.Generic;
using RogueElements;
using RogueEssence.Data;
using RogueEssence.Dungeon;
using RogueEssence.LevelGen;

namespace RogueEssence.Examples
{
    // =========================================================================
    // EXAMPLE 1: Simple Item Placement Step
    // =========================================================================
    /// <summary>
    /// A GenStep that places a specific item at a random location on the floor.
    /// Demonstrates the basic structure of a GenStep and random tile selection.
    /// </summary>
    [Serializable]
    public class PlaceSpecialItemStep<T> : GenStep<T> where T : BaseMapGenContext
    {
        // ---------------------------------------------------------------------
        // STEP 1: Define configurable properties
        // ---------------------------------------------------------------------
        // These can be set in the data editor when adding this step to a zone.

        /// <summary>
        /// The item ID to place on the floor.
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// The quantity of items to place.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// If true, only place in rooms (not corridors).
        /// </summary>
        public bool RoomsOnly { get; set; }

        // ---------------------------------------------------------------------
        // STEP 2: Implement constructors
        // ---------------------------------------------------------------------

        /// <summary>
        /// Default constructor required for serialization.
        /// Always initialize default values here.
        /// </summary>
        public PlaceSpecialItemStep()
        {
            ItemID = "";
            Quantity = 1;
            RoomsOnly = true;
        }

        /// <summary>
        /// Convenience constructor for easy instantiation.
        /// </summary>
        public PlaceSpecialItemStep(string itemId, int quantity = 1, bool roomsOnly = true)
        {
            ItemID = itemId;
            Quantity = quantity;
            RoomsOnly = roomsOnly;
        }

        // ---------------------------------------------------------------------
        // STEP 3: Implement the Apply method
        // ---------------------------------------------------------------------
        // This is the main method that runs during floor generation.
        // The 'map' parameter is the GenContext containing the map being built.

        public override void Apply(T map)
        {
            // Get the random number generator from the context
            // IMPORTANT: Always use map.Rand for randomness to ensure
            // deterministic generation (same seed = same floor)
            IRandom rand = map.Rand;

            // Collect valid spawn locations
            List<Loc> validLocations = new List<Loc>();

            // Iterate through all tiles on the map
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    Loc loc = new Loc(x, y);

                    // Check if this tile is walkable (ground)
                    if (!map.TileBlocked(loc))
                    {
                        // If RoomsOnly is set, check if we're in a room
                        if (RoomsOnly)
                        {
                            // GetRoomIndex returns the room index at this location
                            // Returns -1 if not in a room (e.g., corridor)
                            int roomIndex = map.RoomPlan.GetRoomIndex(loc);
                            if (roomIndex < 0)
                                continue;
                        }

                        // Check if there's no item already here
                        // (Implementation depends on the context type)
                        validLocations.Add(loc);
                    }
                }
            }

            // Place the items
            for (int i = 0; i < Quantity && validLocations.Count > 0; i++)
            {
                // Pick a random valid location
                int index = rand.Next(validLocations.Count);
                Loc spawnLoc = validLocations[index];

                // Remove to prevent placing multiple items in same spot
                validLocations.RemoveAt(index);

                // Create the item
                InvItem item = new InvItem(ItemID);

                // Place it on the map
                // The exact method depends on the context type
                // For StairsMapGenContext (standard dungeon):
                if (map is StairsMapGenContext stairsMap)
                {
                    MapItem mapItem = new MapItem(item);
                    mapItem.TileLoc = spawnLoc;
                    stairsMap.Items.Add(mapItem);
                }
            }
        }

        // ---------------------------------------------------------------------
        // STEP 4: Implement ToString for editor display
        // ---------------------------------------------------------------------
        // This appears in the data editor to identify the step.

        public override string ToString()
        {
            return string.Format("Place {0}x {1}", Quantity, ItemID);
        }
    }

    // =========================================================================
    // EXAMPLE 2: Enemy Spawn Modification Step
    // =========================================================================
    /// <summary>
    /// A GenStep that modifies enemy spawns based on floor conditions.
    /// Demonstrates accessing and modifying spawn lists.
    /// </summary>
    [Serializable]
    public class ConditionalEnemyBoostStep<T> : GenStep<T> where T : StairsMapGenContext
    {
        /// <summary>
        /// The enemy species to boost.
        /// </summary>
        public string TargetSpecies { get; set; }

        /// <summary>
        /// Level boost to apply.
        /// </summary>
        public int LevelBoost { get; set; }

        /// <summary>
        /// Minimum floor number for this to apply.
        /// </summary>
        public int MinFloor { get; set; }

        public ConditionalEnemyBoostStep()
        {
            TargetSpecies = "";
            LevelBoost = 5;
            MinFloor = 5;
        }

        public override void Apply(T map)
        {
            // Check floor condition
            // Note: Accessing current floor info depends on context
            // This is a simplified example

            // Iterate through placed enemies and boost matching ones
            foreach (Team team in map.GenEntrances)
            {
                foreach (Character member in team.Players)
                {
                    // Check if this is the target species
                    if (member.BaseForm.Species == TargetSpecies)
                    {
                        // Boost the level
                        member.Level += LevelBoost;

                        // Recalculate stats for new level
                        member.HP = member.MaxHP;
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Format("Boost {0} by {1} levels after floor {2}",
                TargetSpecies, LevelBoost, MinFloor);
        }
    }

    // =========================================================================
    // EXAMPLE 3: Custom Room Feature Step
    // =========================================================================
    /// <summary>
    /// A GenStep that adds special features to specific room types.
    /// Demonstrates room iteration and tile modification.
    /// </summary>
    [Serializable]
    public class AddRoomFeaturesStep<T> : GenStep<T> where T : StairsMapGenContext
    {
        /// <summary>
        /// Tile to place in the center of matching rooms.
        /// </summary>
        public string CenterTile { get; set; }

        /// <summary>
        /// Terrain to use for room borders.
        /// </summary>
        public string BorderTerrain { get; set; }

        /// <summary>
        /// Chance (0-100) for each room to be affected.
        /// </summary>
        public int ChancePerRoom { get; set; }

        public AddRoomFeaturesStep()
        {
            CenterTile = "";
            BorderTerrain = "";
            ChancePerRoom = 50;
        }

        public override void Apply(T map)
        {
            IRandom rand = map.Rand;

            // Iterate through all rooms in the floor plan
            for (int roomIdx = 0; roomIdx < map.RoomPlan.RoomCount; roomIdx++)
            {
                // Random chance check
                if (rand.Next(100) >= ChancePerRoom)
                    continue;

                // Get the room
                IRoomGen room = map.RoomPlan.GetRoom(roomIdx);

                // Get the room's bounding rectangle
                Rect bounds = room.Draw;

                // ---------------------------------------------------------
                // Add center tile
                // ---------------------------------------------------------
                if (!string.IsNullOrEmpty(CenterTile))
                {
                    // Calculate center position
                    Loc center = new Loc(
                        bounds.X + bounds.Width / 2,
                        bounds.Y + bounds.Height / 2
                    );

                    // Set the tile
                    // Note: Actual tile setting depends on context
                    // This is a simplified example
                    Tile tile = new Tile(CenterTile);
                    map.SetTile(center, tile.CreateFrom(map.RoomTerrain));
                }

                // ---------------------------------------------------------
                // Add border terrain
                // ---------------------------------------------------------
                if (!string.IsNullOrEmpty(BorderTerrain))
                {
                    // Iterate along room edges
                    for (int x = bounds.X; x < bounds.End.X; x++)
                    {
                        // Top edge
                        SetBorderTile(map, new Loc(x, bounds.Y));
                        // Bottom edge
                        SetBorderTile(map, new Loc(x, bounds.End.Y - 1));
                    }
                    for (int y = bounds.Y + 1; y < bounds.End.Y - 1; y++)
                    {
                        // Left edge
                        SetBorderTile(map, new Loc(bounds.X, y));
                        // Right edge
                        SetBorderTile(map, new Loc(bounds.End.X - 1, y));
                    }
                }
            }
        }

        private void SetBorderTile(T map, Loc loc)
        {
            // Check if position is valid and not a door/corridor
            if (loc.X >= 0 && loc.X < map.Width &&
                loc.Y >= 0 && loc.Y < map.Height)
            {
                // Only modify if it's currently ground terrain
                Tile existingTile = map.GetTile(loc);
                if (existingTile.ID == map.RoomTerrain.ID)
                {
                    map.SetTile(loc, new Tile(BorderTerrain));
                }
            }
        }

        public override string ToString()
        {
            return string.Format("Add Room Features ({0}% chance)", ChancePerRoom);
        }
    }

    // =========================================================================
    // EXAMPLE 4: Trap Placement Step
    // =========================================================================
    /// <summary>
    /// A GenStep that places traps in corridors.
    /// Demonstrates corridor-specific placement logic.
    /// </summary>
    [Serializable]
    public class CorridorTrapStep<T> : GenStep<T> where T : StairsMapGenContext
    {
        /// <summary>
        /// List of trap IDs that can be placed.
        /// </summary>
        public SpawnList<string> TrapPool { get; set; }

        /// <summary>
        /// Number of traps to place.
        /// </summary>
        public RandRange TrapCount { get; set; }

        public CorridorTrapStep()
        {
            TrapPool = new SpawnList<string>();
            TrapCount = new RandRange(3, 6);
        }

        public override void Apply(T map)
        {
            IRandom rand = map.Rand;

            // Determine how many traps to place
            int count = TrapCount.Pick(rand);

            // Collect corridor tiles
            List<Loc> corridorTiles = new List<Loc>();

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    Loc loc = new Loc(x, y);

                    // Check if walkable
                    if (map.TileBlocked(loc))
                        continue;

                    // Check if NOT in a room (i.e., in a corridor)
                    int roomIndex = map.RoomPlan.GetRoomIndex(loc);
                    if (roomIndex >= 0)
                        continue;  // In a room, skip

                    // Check if not on stairs or other important tiles
                    Tile tile = map.GetTile(loc);
                    if (tile.Effect.ID != "")
                        continue;  // Has an effect already

                    corridorTiles.Add(loc);
                }
            }

            // Place traps
            for (int i = 0; i < count && corridorTiles.Count > 0; i++)
            {
                // Pick random location
                int locIndex = rand.Next(corridorTiles.Count);
                Loc trapLoc = corridorTiles[locIndex];
                corridorTiles.RemoveAt(locIndex);

                // Pick random trap from pool
                if (TrapPool.Count == 0)
                    continue;

                string trapId = TrapPool.Pick(rand);

                // Place the trap
                EffectTile trap = new EffectTile(trapId, true, trapLoc);
                trap.Revealed = false;  // Hidden trap

                // Set the tile effect
                Tile existingTile = map.GetTile(trapLoc);
                existingTile.Effect = trap;
            }
        }

        public override string ToString()
        {
            return string.Format("Place {0} Corridor Traps", TrapCount);
        }
    }

    // =========================================================================
    // USAGE NOTES
    // =========================================================================
    //
    // To use custom GenSteps in your game:
    //
    // 1. Create the class in your mod/plugin assembly
    // 2. Add the [Serializable] attribute
    // 3. Register the type with the data system (if needed)
    // 4. Add instances to zone floor generation via the Data Editor
    //
    // GenStep execution order is determined by the Priority value
    // when added to the floor generation pipeline. Lower priorities
    // run earlier. Common priority ranges:
    //
    // - Priority -10 to -5: Initial setup steps
    // - Priority -5 to 0: Room/corridor generation
    // - Priority 0 to 5: Basic feature placement (items, stairs)
    // - Priority 5 to 10: Enemy/trap placement
    // - Priority 10+: Final modifications and polish
    //
    // =========================================================================
    // GENSTEP TYPE REFERENCE
    // =========================================================================
    //
    // Common GenStep base classes to inherit from:
    //
    // GenStep<T> - Basic step, T is the context type
    // FloorPlanStep<T> - Works with floor plans (rooms/corridors)
    // GridPlanStep<T> - Works with grid-based layouts
    //
    // Common context types (T parameter):
    //
    // BaseMapGenContext - Minimal map context
    // StairsMapGenContext - Standard dungeon context
    // ListMapGenContext - Context with spawn lists
    //
    // =========================================================================
    // USEFUL CONTEXT PROPERTIES
    // =========================================================================
    //
    // map.Rand - Random number generator (use this!)
    // map.Width, map.Height - Map dimensions
    // map.RoomPlan - Floor plan with rooms and halls
    // map.GetTile(loc) - Get tile at position
    // map.SetTile(loc, tile) - Set tile at position
    // map.TileBlocked(loc) - Check if position is blocked
    // map.Items - List of items on the floor
    // map.GenEntrances - Enemy team spawn points
    //
}
