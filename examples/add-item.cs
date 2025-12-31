// =============================================================================
// EXAMPLE: Adding an Item (ItemData)
// =============================================================================
// This file demonstrates how to create a complete ItemData entry in
// RogueEssence. ItemData defines an item with its use effects, throwing
// behavior, equip effects, and visual properties.
//
// Items can be:
// - Usable (like potions, orbs)
// - Throwable (like seeds, wands)
// - Equippable (like scarves, ribbons)
// - Holdable (passive effects when in inventory)
// - Stackable (like arrows, ammo)
// =============================================================================

using System;
using System.Collections.Generic;
using RogueEssence.Data;
using RogueEssence.Dungeon;
using RogueEssence.Content;
using RogueElements;

namespace RogueEssence.Examples
{
    public class AddItemExample
    {
        /// <summary>
        /// Creates a healing potion item that restores HP when used.
        /// Demonstrates usable item mechanics with UseAction.
        /// </summary>
        public static ItemData CreateHealingPotion()
        {
            // -----------------------------------------------------------------
            // STEP 1: Create the ItemData container
            // -----------------------------------------------------------------
            ItemData item = new ItemData();

            // -----------------------------------------------------------------
            // STEP 2: Set basic metadata
            // -----------------------------------------------------------------

            // Name - The item name shown to players
            item.Name = new LocalText("Super Elixir");

            // Desc - The item description
            item.Desc = new LocalText("A potent healing potion. Restores 100 HP.");

            // Comment - Developer notes (not shown to players)
            item.Comment = "Example healing item";

            // Released - If false, won't appear in the game
            item.Released = true;

            // IndexNum - Internal ordering number
            item.IndexNum = 9999;

            // -----------------------------------------------------------------
            // STEP 3: Set item sprite and sorting
            // -----------------------------------------------------------------

            // Icon - The sprite sheet index for this item's icon
            // SortCategory determines where this appears in inventory sorting
            item.SortCategory = 10;  // Healing items category

            // Sprite - Visual appearance
            // This references a sprite in the item sprite sheet
            item.Sprite = "Potion_Red";

            // -----------------------------------------------------------------
            // STEP 4: Set item pricing
            // -----------------------------------------------------------------

            // Price - Buy price from shops (sell price is usually half)
            item.Price = 500;

            // -----------------------------------------------------------------
            // STEP 5: Set item rarity (for spawn weights)
            // -----------------------------------------------------------------

            // Rarity - Affects spawn frequency (higher = rarer)
            item.Rarity = 3;

            // -----------------------------------------------------------------
            // STEP 6: Configure item states
            // -----------------------------------------------------------------
            // ItemStates are markers that identify item properties.
            // They can also store data used by item events.

            // Mark as a medicine/healing item
            item.ItemStates.Set(new MedicineState());

            // EdibleState marks items that can be eaten
            // (Some abilities trigger on eating items)
            item.ItemStates.Set(new EdibleState());

            // -----------------------------------------------------------------
            // STEP 7: Configure use action
            // -----------------------------------------------------------------
            // UseAction determines what happens when the item is used.
            // Different action types have different effects.

            // For a self-targeting healing item:
            SelfAction useAction = new SelfAction();
            useAction.TargetAlignments = Alignment.Self;
            useAction.CharAnimData = new CharAnimFrameType(GraphicsManager.ChargeAction);

            item.UseAction = useAction;

            // -----------------------------------------------------------------
            // STEP 8: Configure use event (the actual effect)
            // -----------------------------------------------------------------
            // UseEvent is the BattleData that controls what the item does.

            // Element (typically "none" for items)
            item.UseEvent.Element = "";

            // Category (Status for non-damaging effects)
            item.UseEvent.Category = BattleData.SkillCategory.Status;

            // Hit rate (-1 = always hits)
            item.UseEvent.HitRate = -1;

            // Add the healing effect
            // RestoreHPEvent(amount, percent, message_key)
            // amount: flat HP to restore
            // percent: fraction of max HP to restore (0-100)
            // message_key: localization key for the message
            item.UseEvent.OnHits.Add(Priority.Zero, new RestoreHPEvent(100, false));

            // Add use sound effect
            item.UseEvent.IntroFX.Add(new BattleFX { Sound = "DUN_Drink" });

            // -----------------------------------------------------------------
            // STEP 9: Configure explosion data (targeting)
            // -----------------------------------------------------------------
            // Explosion determines who gets hit by the item effect

            item.Explosion.Range = 0;  // No radius (self only)
            item.Explosion.TargetAlignments = Alignment.Self;

            // -----------------------------------------------------------------
            // STEP 10: Configure throwable behavior (optional)
            // -----------------------------------------------------------------
            // Most healing items aren't thrown, but we can make it throwable
            // If thrown at an ally, it could heal them

            // MaxStack - How many can stack in one inventory slot
            // 0 = not stackable, >0 = stackable with max count
            item.MaxStack = 0;  // Not stackable

            // -----------------------------------------------------------------
            // Item is complete!
            // -----------------------------------------------------------------
            return item;
        }

        /// <summary>
        /// Creates a throwable damaging item (like a throwing star).
        /// Demonstrates projectile items and throwing mechanics.
        /// </summary>
        public static ItemData CreateThrowableWeapon()
        {
            ItemData item = new ItemData();

            item.Name = new LocalText("Iron Spike");
            item.Desc = new LocalText("A sharp metal spike. Throw it at enemies to deal damage.");
            item.Released = true;
            item.Price = 50;
            item.Rarity = 1;
            item.SortCategory = 20;  // Throwing items category
            item.Sprite = "Spike_Iron";

            // Mark as a throwing item
            item.ItemStates.Set(new AmmoState());

            // Stackable - can hold multiple in one slot
            item.MaxStack = 99;

            // -----------------------------------------------------------------
            // THROWING CONFIGURATION
            // -----------------------------------------------------------------

            // UseAction for thrown projectile
            ProjectileAction throwAction = new ProjectileAction();
            throwAction.Range = 10;           // Max throw distance
            throwAction.Speed = 14;           // Projectile speed
            throwAction.StopAtWall = true;    // Stops at walls
            throwAction.StopAtHit = true;     // Stops after hitting
            throwAction.CharAnimData = new CharAnimFrameType(GraphicsManager.ChargeAction);
            // throwAction.Anim = new AnimData("Spike_Throw", 3);  // Thrown animation

            item.UseAction = throwAction;

            // Throw effect - deals damage
            item.UseEvent.Element = "none";
            item.UseEvent.Category = BattleData.SkillCategory.Physical;
            item.UseEvent.HitRate = 90;  // Can miss

            // Base power for thrown damage
            item.UseEvent.SkillStates.Set(new BasePowerState(50));

            // Deal damage on hit
            item.UseEvent.OnHits.Add(Priority.Zero, new DamageEvent());

            // Sound effect on hit
            item.UseEvent.HitFX.Sound = "DUN_Hit_Neutral";

            // Targets foes when thrown
            item.Explosion.TargetAlignments = Alignment.Foe;

            return item;
        }

        /// <summary>
        /// Creates an equippable item with passive effects.
        /// Demonstrates held item mechanics.
        /// </summary>
        public static ItemData CreateEquippableAccessory()
        {
            ItemData item = new ItemData();

            item.Name = new LocalText("Power Scarf");
            item.Desc = new LocalText("Boosts Attack when equipped.");
            item.Released = true;
            item.Price = 1000;
            item.Rarity = 4;
            item.SortCategory = 30;  // Equipment category
            item.Sprite = "Scarf_Red";

            // Mark as an equippable item
            item.ItemStates.Set(new EquipState());

            // -----------------------------------------------------------------
            // PASSIVE EFFECTS (When held/equipped)
            // -----------------------------------------------------------------
            // These events trigger based on conditions while holding the item.
            // They use the same event system as intrinsics and statuses.

            // OnRefresh - Modify stats when equipped
            // This is called whenever the character's stats are recalculated
            item.OnRefresh.Add(Priority.Zero, new StatBoostEvent(Stat.Attack, 10));

            // BeforeHittings - Events when the holder hits someone
            // item.BeforeHittings.Add(Priority.Zero, new DamageBonusEvent(10));

            // BeforeBeingHits - Events when the holder is hit
            // item.BeforeBeingHits.Add(Priority.Zero, new DamageReductionEvent(5));

            // OnTurnStarts - Events at the start of the holder's turn
            // item.OnTurnStarts.Add(Priority.Zero, new HealOverTimeEvent(5));

            // OnTurnEnds - Events at the end of the holder's turn
            // item.OnTurnEnds.Add(Priority.Zero, new SomePeriodicEvent());

            // -----------------------------------------------------------------
            // PROXIMITY EFFECTS (Affects nearby allies/foes)
            // -----------------------------------------------------------------
            // ProximityEvent allows items to affect characters within a radius

            // item.ProximityEvent.Radius = 1;  // 1 tile radius
            // item.ProximityEvent.TargetAlignments = Alignment.Friend;
            // item.ProximityEvent.OnRefresh.Add(Priority.Zero, new DefenseAuraEvent(5));

            // No use action (not usable, only equippable)
            item.UseAction = new SelfAction();
            item.UseEvent.HitRate = -1;

            return item;
        }

        /// <summary>
        /// Creates an orb item that affects the entire room/floor.
        /// Demonstrates area-effect items.
        /// </summary>
        public static ItemData CreateRoomOrb()
        {
            ItemData item = new ItemData();

            item.Name = new LocalText("Escape Orb");
            item.Desc = new LocalText("Instantly teleports the team out of the dungeon.");
            item.Released = true;
            item.Price = 1000;
            item.Rarity = 5;
            item.SortCategory = 15;  // Orb category
            item.Sprite = "Orb_Blue";

            // Mark as an orb
            item.ItemStates.Set(new OrbState());

            // Use action targets the entire floor
            AreaAction orbAction = new AreaAction();
            orbAction.Range = -1;  // -1 = entire floor
            orbAction.HitSelf = true;
            orbAction.TargetAlignments = Alignment.Self | Alignment.Friend;
            orbAction.CharAnimData = new CharAnimFrameType(GraphicsManager.ChargeAction);

            item.UseAction = orbAction;

            // Orb effect - escape dungeon
            item.UseEvent.Element = "";
            item.UseEvent.Category = BattleData.SkillCategory.Status;
            item.UseEvent.HitRate = -1;

            // Warp the team out
            // Note: The actual escape effect would need a custom event
            // This is a placeholder showing the structure
            item.UseEvent.OnHits.Add(Priority.Zero, new WarpToEndEvent());

            item.UseEvent.IntroFX.Add(new BattleFX { Sound = "DUN_Orb" });

            item.Explosion.TargetAlignments = Alignment.Self | Alignment.Friend;

            return item;
        }

        /// <summary>
        /// Creates a food item that restores belly.
        /// Demonstrates food mechanics.
        /// </summary>
        public static ItemData CreateFoodItem()
        {
            ItemData item = new ItemData();

            item.Name = new LocalText("Big Apple");
            item.Desc = new LocalText("A large, delicious apple. Fills belly by 100.");
            item.Released = true;
            item.Price = 100;
            item.Rarity = 2;
            item.SortCategory = 5;  // Food category
            item.Sprite = "Apple_Big";

            // Mark as food
            item.ItemStates.Set(new FoodState());
            item.ItemStates.Set(new EdibleState());

            // Self-targeting use action
            SelfAction eatAction = new SelfAction();
            eatAction.TargetAlignments = Alignment.Self;
            eatAction.CharAnimData = new CharAnimFrameType(GraphicsManager.ChargeAction);

            item.UseAction = eatAction;

            // Food effect - restore belly
            item.UseEvent.Element = "";
            item.UseEvent.Category = BattleData.SkillCategory.Status;
            item.UseEvent.HitRate = -1;

            // Restore 100 belly
            item.UseEvent.OnHits.Add(Priority.Zero, new RestoreBellyEvent(100, false));

            // Eating sound
            item.UseEvent.IntroFX.Add(new BattleFX { Sound = "DUN_Drink" });

            item.Explosion.TargetAlignments = Alignment.Self;

            return item;
        }

        /// <summary>
        /// Creates a wand item with limited charges.
        /// Demonstrates charged items and projectile targeting.
        /// </summary>
        public static ItemData CreateWandItem()
        {
            ItemData item = new ItemData();

            item.Name = new LocalText("Warp Wand");
            item.Desc = new LocalText("A magic wand that teleports the target to a random location.");
            item.Released = true;
            item.Price = 300;
            item.Rarity = 3;
            item.SortCategory = 25;  // Wand category
            item.Sprite = "Wand_Purple";

            // Mark as a wand
            item.ItemStates.Set(new WandState());

            // Wands are stackable by charges
            item.MaxStack = 10;  // Can have up to 10 charges

            // Projectile action for aimed wand
            ProjectileAction wandAction = new ProjectileAction();
            wandAction.Range = 10;
            wandAction.Speed = 20;
            wandAction.StopAtWall = true;
            wandAction.StopAtHit = true;  // Affects first target hit
            wandAction.CharAnimData = new CharAnimFrameType(GraphicsManager.ChargeAction);

            item.UseAction = wandAction;

            // Wand effect - teleport target
            item.UseEvent.Element = "";
            item.UseEvent.Category = BattleData.SkillCategory.Status;
            item.UseEvent.HitRate = -1;  // Always hits

            // Warp the target
            item.UseEvent.OnHits.Add(Priority.Zero, new RandomWarpEvent());

            item.UseEvent.IntroFX.Add(new BattleFX { Sound = "DUN_Wand" });
            item.UseEvent.HitFX.Sound = "DUN_Warp";

            // Can hit anyone
            item.Explosion.TargetAlignments = Alignment.Foe | Alignment.Friend | Alignment.Self;

            return item;
        }

        // =====================================================================
        // COMMON ITEM STATES REFERENCE
        // =====================================================================
        // The following are commonly used ItemState types:
        //
        // MedicineState - Marks as a healing item
        // EdibleState - Can be eaten (triggers Gluttony, etc.)
        // FoodState - Is a belly-restoring food
        // OrbState - Is an orb item
        // WandState - Is a wand item
        // AmmoState - Is throwable ammo
        // EquipState - Can be equipped
        // CursedState - Item is cursed
        // MaterialState - Is a crafting material
        // KeyState - Is a key item
        // EvoState(evolution_id) - Triggers evolution when used
        //
        // Check RogueEssence/Dungeon/GameEffects/ for more state types

        // =====================================================================
        // COMMON ITEM EVENTS REFERENCE
        // =====================================================================
        // The following are commonly used item effect events:
        //
        // RestoreHPEvent(amount, percent) - Heals HP
        // RestoreBellyEvent(amount, percent) - Restores belly
        // RestorePPEvent(amount, restoreAll) - Restores skill PP
        // StatBoostEvent(stat, amount) - Boosts a stat
        // DamageEvent() - Deals damage (for thrown items)
        // StatusBattleEvent(...) - Applies a status
        // RandomWarpEvent() - Teleports target randomly
        // WarpToEndEvent() - Warps to stairs
        // RevealTrapsEvent() - Reveals traps
        // CleanseEvent() - Removes status conditions
        //
        // Check RogueEssence/Dungeon/GameEffects/ for more event types
    }
}
