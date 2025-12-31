# Licenses

This directory contains license files for third-party libraries and assets used by Rogue Essence. Each license file corresponds to a dependency that must be attributed according to its respective terms.

## License Files

| File | Library | License Type |
|------|---------|--------------|
| `AABB.LICENSE` | AABB collision library | MIT |
| `FNA.LICENSE` | FNA (XNA reimplementation) | Microsoft Public License (Ms-PL) |
| `Mediator.md` | Mediator pattern library | (See file for details) |
| `Middleclass.LICENSE` | Lua class library | MIT |
| `NLua.LICENSE` | NLua (.NET Lua bridge) | MIT |
| `Primitive2D.LICENSE` | 2D primitive rendering | MIT |
| `QuadTrees.LICENSE` | QuadTree spatial indexing | (See file for details) |
| `RogueElements.LICENSE` | Dungeon generation library | MIT |
| `Serpent.LICENSE` | Lua serialization library | MIT |
| `unxwb.LICENSE` | XWB audio extraction tool | (See file for details) |

## Relationships

- **RogueEssence**: The main game project depends on these libraries for graphics (FNA), scripting (NLua, Lua libraries), procedural generation (RogueElements), and spatial queries (QuadTrees, AABB)
- **Distribution**: When distributing Rogue Essence, these license files should be included to comply with attribution requirements
- **FNA submodule**: The FNA directory in the repository root contains the actual FNA source; this file provides the license reference

## Usage

When packaging or distributing Rogue Essence:

1. Include this entire `Licenses/` directory in the distribution
2. Reference these licenses in any "About" or "Credits" section
3. Ensure compliance with each license's specific attribution requirements

For Ms-PL licensed code (FNA), note that source code modifications must be released under the same license if distributed.

---

![Repobeats analytics](https://repobeats.axiom.co/api/embed/placeholder.svg "Repobeats analytics image")
