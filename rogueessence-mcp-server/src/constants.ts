import { fileURLToPath } from "url";
import { dirname, resolve } from "path";

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

// Project root - navigate from dist/constants.js to rogueessence-mcp-server to RogueEssence
export const PROJECT_ROOT = resolve(__dirname, "..", "..");

// Key directories
export const DATA_DIR = resolve(PROJECT_ROOT, "Data");
export const DOCS_DIR = resolve(PROJECT_ROOT, "docs", "claude");
export const EXAMPLES_DIR = resolve(PROJECT_ROOT, "examples");
export const SOURCE_DIRS = [
  resolve(PROJECT_ROOT, "RogueEssence"),
  resolve(PROJECT_ROOT, "RogueEssence.Editor.Avalonia"),
  resolve(PROJECT_ROOT, "WaypointServer")
];

// Response limits
export const CHARACTER_LIMIT = 25000;
export const DEFAULT_LIST_LIMIT = 20;
export const MAX_LIST_LIMIT = 100;

// Data types supported
export const DATA_TYPES = [
  "Monster",
  "Skill",
  "Item",
  "Intrinsic",
  "Status",
  "MapStatus",
  "Terrain",
  "Tile",
  "Zone",
  "Emote",
  "AutoTile",
  "Element",
  "GrowthGroup",
  "SkillGroup",
  "AI",
  "Rank",
  "Skin"
] as const;

export type DataType = typeof DATA_TYPES[number];
