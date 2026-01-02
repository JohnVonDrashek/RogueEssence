#!/usr/bin/env node
/**
 * RogueEssence MCP Server
 *
 * Provides tools for Claude to interact with the RogueEssence game engine:
 * - Query game data (monsters, skills, items, zones, etc.)
 * - Search and explore the codebase with tree-sitter AST parsing
 * - Access AI-optimized documentation and examples
 * - Browse class categories with XML doc extraction
 * - Scaffold new classes with proper boilerplate
 *
 * Uses tree-sitter for proper AST-based C# parsing.
 */

import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import { z } from "zod";
import * as fs from "fs";
import * as path from "path";
import { fileURLToPath } from "url";
import { Parser, Language, Node } from "web-tree-sitter";

// Get __dirname equivalent for ES modules
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// =============================================================================
// PROJECT PATHS
// =============================================================================

function findProjectRoot(): string {
  const candidates = [
    path.resolve(__dirname, "../.."),
    path.resolve(__dirname, ".."),
    path.resolve(process.cwd()),
  ];

  for (const dir of candidates) {
    if (fs.existsSync(path.join(dir, "RogueEssence.sln"))) {
      return dir;
    }
  }

  return process.cwd();
}

function findDocsDir(): string {
  const root = findProjectRoot();
  const candidates = [
    path.join(root, "docs/claude"),
    path.resolve(__dirname, "../../docs/claude"),
  ];

  for (const dir of candidates) {
    if (fs.existsSync(dir)) {
      return dir;
    }
  }

  return path.join(root, "docs/claude");
}

function findExamplesDir(): string {
  const root = findProjectRoot();
  return path.join(root, "examples");
}

function findDataDir(): string {
  const root = findProjectRoot();
  const candidates = [
    path.join(root, "PMDO/Data"),
    path.join(root, "Data"),
  ];

  for (const dir of candidates) {
    if (fs.existsSync(dir)) {
      return dir;
    }
  }

  return path.join(root, "Data");
}

export const PROJECT_ROOT = findProjectRoot();
const DOCS_DIR = findDocsDir();
const EXAMPLES_DIR = findExamplesDir();
const DATA_DIR = findDataDir();
const ROGUEESSENCE_DIR = path.join(PROJECT_ROOT, "RogueEssence");

// =============================================================================
// TREE-SITTER INITIALIZATION
// =============================================================================

let csharpParser: Parser | null = null;
let csharpLanguage: Language | null = null;

async function initializeParser(): Promise<void> {
  if (csharpParser) return;

  await Parser.init();
  csharpParser = new Parser();

  // Load C# grammar from node_modules
  const wasmPath = path.resolve(__dirname, "../node_modules/tree-sitter-c-sharp/tree-sitter-c_sharp.wasm");
  csharpLanguage = await Language.load(wasmPath);
  csharpParser.setLanguage(csharpLanguage);
}

// =============================================================================
// TREE-SITTER HELPERS
// =============================================================================

function findNodesByType(node: Node, type: string, results: Node[] = []): Node[] {
  if (node.type === type) results.push(node);
  for (let i = 0; i < node.childCount; i++) {
    const child = node.child(i);
    if (child) findNodesByType(child, type, results);
  }
  return results;
}

function getDocComments(node: Node): string {
  const comments: string[] = [];
  let prev = node.previousSibling;

  while (prev && prev.type === "comment") {
    comments.unshift(prev.text);
    prev = prev.previousSibling;
  }

  return comments.join("\n");
}

function parseDocComment(docText: string): { summary: string; remarks: string; inheritdoc: boolean } {
  const summaryMatch = docText.match(/<summary>\s*([\s\S]*?)\s*<\/summary>/);
  const summary = summaryMatch
    ? summaryMatch[1].replace(/^\s*\/\/\/\s*/gm, "").trim()
    : "";

  const remarksMatch = docText.match(/<remarks>\s*([\s\S]*?)\s*<\/remarks>/);
  const remarks = remarksMatch
    ? remarksMatch[1].replace(/^\s*\/\/\/\s*/gm, "").trim()
    : "";

  const inheritdoc = docText.includes("<inheritdoc");

  return { summary, remarks, inheritdoc };
}

function getFieldText(node: Node, fieldName: string): string {
  const field = node.childForFieldName(fieldName);
  return field ? field.text : "";
}

// =============================================================================
// CLASS CATEGORIES (RogueEssence-specific)
// =============================================================================

const CLASS_CATEGORIES = {
  "game-events": {
    dir: "Dungeon/GameEffects",
    baseClasses: ["GameEvent", "BattleEvent", "SingleCharEvent"],
    description: "Battle effects, status changes, and game events"
  },
  "gameplay-states": {
    dir: "Dungeon/GameEffects",
    baseClasses: ["GameplayState", "CharState", "StatusState", "ContextState", "SkillState", "ItemState"],
    description: "Mutable state data for characters, statuses, and context"
  },
  "character-system": {
    dir: "Dungeon/Characters",
    baseClasses: ["CharAction", "CharAnimation"],
    description: "Character actions, animations, and behavior"
  },
  "menus-ui": {
    dir: "Menu",
    baseClasses: ["MenuBase", "InteractableMenu", "ChoiceMenu", "DialogueBox"],
    description: "Game menus, dialogs, and UI components"
  },
  "level-generation": {
    dir: "LevelGen",
    baseClasses: ["GenStep", "RoomGen", "TeamSpawner", "FloorMapGen"],
    description: "Procedural dungeon and floor generation"
  },
  "zone-generation": {
    dir: "LevelGen/Zones",
    baseClasses: ["ZoneStep", "ZoneSegmentBase"],
    description: "Zone-level generation and segment configuration"
  },
  "game-data": {
    dir: "Data",
    baseClasses: ["BaseData", "ActiveEffect"],
    description: "Game data types (monsters, skills, items, etc.)"
  },
  "animation-effects": {
    dir: "Content/Animation",
    baseClasses: ["BaseAnim", "BaseEmitter", "AnimDataBase"],
    description: "Visual effects, particles, and animations"
  },
  "ground-system": {
    dir: "Ground",
    baseClasses: ["GroundEntity", "GroundTask", "GroundAction", "GroundAI"],
    description: "Overworld gameplay, AABB collision, AI movement"
  },
  "scenes": {
    dir: "Scene",
    baseClasses: ["BaseScene", "YieldInstruction"],
    description: "Scene management, transitions, coroutines"
  },
  "dungeon-maps": {
    dir: "Dungeon/Maps",
    baseClasses: ["BaseMap", "MapStatus", "MapItem"],
    description: "Dungeon tiles, entities, spatial structures"
  },
  "editor-tools": {
    dir: "Dev",
    baseClasses: ["Undoable", "CharSheetOp", "CanvasStroke"],
    description: "Editor utilities, undo/redo, development tools"
  }
} as const;

type ClassCategory = keyof typeof CLASS_CATEGORIES;

// =============================================================================
// CLASS DOCUMENTATION PARSING
// =============================================================================

interface ClassDoc {
  name: string;
  namespace: string;
  baseClass: string;
  summary: string;
  remarks: string;
  properties: Array<{ name: string; type: string; summary: string }>;
  methods: Array<{ name: string; signature: string; summary: string }>;
  filePath: string;
}

async function parseClassFile(filePath: string): Promise<ClassDoc[]> {
  try {
    await initializeParser();
    if (!csharpParser) return [];

    let content = fs.readFileSync(filePath, "utf-8");

    // Strip UTF-8 BOM if present
    if (content.charCodeAt(0) === 0xFEFF) {
      content = content.slice(1);
    }

    const tree = csharpParser.parse(content);
    if (!tree) return [];

    const results: ClassDoc[] = [];

    // Extract namespace
    const namespaceDecls = findNodesByType(tree.rootNode, "namespace_declaration");
    const namespace = namespaceDecls.length > 0 ? getFieldText(namespaceDecls[0], "name") : "";

    // Find all class declarations
    const classDecls = findNodesByType(tree.rootNode, "class_declaration");

    for (const classNode of classDecls) {
      const className = getFieldText(classNode, "name");
      if (!className) continue;

      // Get base class from base_list
      const baseLists = findNodesByType(classNode, "base_list");
      let baseClass = "";
      if (baseLists.length > 0) {
        const baseList = baseLists[0];
        for (let i = 0; i < baseList.childCount; i++) {
          const child = baseList.child(i);
          if (child && child.type !== ":") {
            baseClass = child.text.split(",")[0].trim();
            break;
          }
        }
      }

      // Get doc comments for the class
      const classDoc = getDocComments(classNode);
      const { summary, remarks } = parseDocComment(classDoc);

      // Extract fields
      const properties: ClassDoc["properties"] = [];
      const fieldDecls = findNodesByType(classNode, "field_declaration");

      for (const field of fieldDecls) {
        const fieldDoc = getDocComments(field);
        const { summary: fieldSummary } = parseDocComment(fieldDoc);

        const varDecl = findNodesByType(field, "variable_declaration")[0];
        let fieldType = "unknown";
        if (varDecl) {
          const typeNode = varDecl.childForFieldName("type");
          if (typeNode) {
            fieldType = typeNode.text;
          } else {
            for (let i = 0; i < varDecl.childCount; i++) {
              const child = varDecl.child(i);
              if (child && child.type !== "variable_declarator" && child.type !== "," && child.type !== ";") {
                fieldType = child.text;
                break;
              }
            }
          }
        }

        const variableDeclarators = findNodesByType(field, "variable_declarator");
        for (const declarator of variableDeclarators) {
          const varName = getFieldText(declarator, "name") || declarator.text.split("=")[0].trim();
          properties.push({
            name: varName,
            type: fieldType,
            summary: fieldSummary || ""
          });
        }
      }

      // Extract methods
      const methods: ClassDoc["methods"] = [];
      const methodDecls = findNodesByType(classNode, "method_declaration");

      for (const method of methodDecls) {
        const methodDoc = getDocComments(method);
        const { summary: methodSummary, inheritdoc } = parseDocComment(methodDoc);

        const methodName = getFieldText(method, "name");
        const returnType = getFieldText(method, "type") || "void";
        const paramsNode = method.childForFieldName("parameters");
        const params = paramsNode ? paramsNode.text : "()";

        const signature = `${returnType} ${methodName}${params}`;

        methods.push({
          name: methodName,
          signature,
          summary: inheritdoc ? "(inherited documentation)" : (methodSummary || "")
        });
      }

      results.push({
        name: className,
        namespace,
        baseClass,
        summary,
        remarks,
        properties,
        methods,
        filePath
      });
    }

    return results;
  } catch {
    // Silently skip files that fail to parse
    return [];
  }
}

function matchesBaseClass(classBaseClass: string, targetBaseClasses: readonly string[], className?: string): boolean {
  // Also match if this IS a base class itself
  if (className && targetBaseClasses.includes(className)) return true;

  if (!classBaseClass) return false;

  for (const target of targetBaseClasses) {
    // Direct match
    if (classBaseClass === target) return true;

    // Handle generic base classes
    const baseWithoutGenerics = classBaseClass.replace(/<[^>]+>/, "");
    if (baseWithoutGenerics === target) return true;

    // Inheritance chain check
    if (classBaseClass.includes(target)) return true;
  }

  return false;
}

async function findClassesInCategory(category: ClassCategory): Promise<ClassDoc[]> {
  const categoryInfo = CLASS_CATEGORIES[category];
  const categoryDir = path.join(ROGUEESSENCE_DIR, categoryInfo.dir);

  if (!fs.existsSync(categoryDir)) {
    return [];
  }

  const classes: ClassDoc[] = [];
  const filesToParse: string[] = [];

  function collectFiles(dir: string) {
    try {
      const entries = fs.readdirSync(dir, { withFileTypes: true });

      for (const entry of entries) {
        const fullPath = path.join(dir, entry.name);

        if (entry.isDirectory() && !entry.name.startsWith(".") && entry.name !== "obj" && entry.name !== "bin") {
          collectFiles(fullPath);
        } else if (entry.isFile() && entry.name.endsWith(".cs")) {
          filesToParse.push(fullPath);
        }
      }
    } catch {
      // Ignore permission errors
    }
  }

  collectFiles(categoryDir);

  // Parse all files
  for (const filePath of filesToParse) {
    const fileDocs = await parseClassFile(filePath);
    for (const classDoc of fileDocs) {
      if (matchesBaseClass(classDoc.baseClass, categoryInfo.baseClasses, classDoc.name)) {
        classes.push(classDoc);
      }
    }
  }

  return classes;
}

async function findClassByName(className: string): Promise<ClassDoc | null> {
  for (const category of Object.keys(CLASS_CATEGORIES) as ClassCategory[]) {
    const classes = await findClassesInCategory(category);
    const found = classes.find(c => c.name.toLowerCase() === className.toLowerCase());
    if (found) return found;
  }
  return null;
}

function levenshteinDistance(a: string, b: string): number {
  const matrix: number[][] = [];

  for (let i = 0; i <= b.length; i++) {
    matrix[i] = [i];
  }
  for (let j = 0; j <= a.length; j++) {
    matrix[0][j] = j;
  }

  for (let i = 1; i <= b.length; i++) {
    for (let j = 1; j <= a.length; j++) {
      if (b.charAt(i - 1) === a.charAt(j - 1)) {
        matrix[i][j] = matrix[i - 1][j - 1];
      } else {
        matrix[i][j] = Math.min(
          matrix[i - 1][j - 1] + 1,
          matrix[i][j - 1] + 1,
          matrix[i - 1][j] + 1
        );
      }
    }
  }

  return matrix[b.length][a.length];
}

async function findSimilarClasses(searchName: string, limit: number = 5): Promise<Array<{ name: string; category: string; score: number }>> {
  const searchLower = searchName.toLowerCase();
  const allMatches: Array<{ name: string; category: string; score: number }> = [];

  for (const category of Object.keys(CLASS_CATEGORIES) as ClassCategory[]) {
    const classes = await findClassesInCategory(category);

    for (const cls of classes) {
      const nameLower = cls.name.toLowerCase();
      let score = levenshteinDistance(searchLower, nameLower);

      if (nameLower.includes(searchLower) || searchLower.includes(nameLower)) {
        score = Math.max(0, score - 5);
      }

      const searchParts = searchLower.replace(/event|action|state|menu|step|gen/gi, "").trim();
      if (searchParts && nameLower.includes(searchParts)) {
        score = Math.max(0, score - 3);
      }

      allMatches.push({ name: cls.name, category, score });
    }
  }

  return allMatches
    .sort((a, b) => a.score - b.score)
    .slice(0, limit);
}

// =============================================================================
// DATA TYPES (game content)
// =============================================================================

export const DATA_TYPES = [
  "Monster", "Skill", "Item", "Intrinsic", "Status", "MapStatus",
  "Terrain", "Tile", "Zone", "Emote", "AutoTile", "Element",
  "GrowthGroup", "SkillGroup", "AI", "Rank", "Skin"
] as const;

export type DataType = typeof DATA_TYPES[number];

function isValidDataType(type: string): type is DataType {
  return DATA_TYPES.includes(type as DataType);
}

function getDataPath(dataType: DataType): string {
  return path.join(DATA_DIR, dataType);
}

async function getDataById(dataType: DataType, id: string): Promise<Record<string, unknown> | null> {
  const dataPath = getDataPath(dataType);
  const filePath = path.join(dataPath, `${id}.json`);

  if (!fs.existsSync(filePath)) {
    return null;
  }

  try {
    const content = fs.readFileSync(filePath, "utf-8");
    return JSON.parse(content);
  } catch {
    return null;
  }
}

async function listData(dataType: DataType, options: { limit: number; offset: number; search?: string }): Promise<{ items: Array<{ id: string; name?: string }>; total: number; hasMore: boolean; nextOffset?: number }> {
  const dataPath = getDataPath(dataType);

  if (!fs.existsSync(dataPath)) {
    return { items: [], total: 0, hasMore: false };
  }

  try {
    let files = fs.readdirSync(dataPath)
      .filter(f => f.endsWith(".json"))
      .map(f => f.replace(".json", ""));

    if (options.search) {
      const searchLower = options.search.toLowerCase();
      files = files.filter(f => f.toLowerCase().includes(searchLower));
    }

    const total = files.length;
    const items = files
      .slice(options.offset, options.offset + options.limit)
      .map(id => ({ id, name: id }));

    const hasMore = options.offset + options.limit < total;

    return {
      items,
      total,
      hasMore,
      ...(hasMore ? { nextOffset: options.offset + options.limit } : {})
    };
  } catch {
    return { items: [], total: 0, hasMore: false };
  }
}

async function getDataTypeStats(): Promise<Record<string, number>> {
  const stats: Record<string, number> = {};

  for (const dataType of DATA_TYPES) {
    const dataPath = getDataPath(dataType);
    try {
      if (fs.existsSync(dataPath)) {
        const files = fs.readdirSync(dataPath).filter(f => f.endsWith(".json"));
        stats[dataType] = files.length;
      } else {
        stats[dataType] = 0;
      }
    } catch {
      stats[dataType] = 0;
    }
  }

  return stats;
}

// =============================================================================
// DOCUMENTATION HELPERS
// =============================================================================

async function listDocs(): Promise<Array<{ name: string; description: string }>> {
  const docs: Array<{ name: string; description: string }> = [];

  if (fs.existsSync(DOCS_DIR)) {
    const files = fs.readdirSync(DOCS_DIR).filter(f => f.endsWith(".md"));
    for (const file of files) {
      docs.push({
        name: file.replace(".md", ""),
        description: `AI documentation: ${file}`
      });
    }
  }

  return docs;
}

async function getDoc(name: string): Promise<{ name: string; path: string; content: string; truncated: boolean } | null> {
  const filePath = path.join(DOCS_DIR, `${name}.md`);

  if (!fs.existsSync(filePath)) {
    return null;
  }

  const content = fs.readFileSync(filePath, "utf-8");
  const maxLength = 50000;
  const truncated = content.length > maxLength;

  return {
    name,
    path: filePath,
    content: truncated ? content.substring(0, maxLength) + "\n\n... (truncated)" : content,
    truncated
  };
}

async function listExamples(): Promise<Array<{ name: string; description: string }>> {
  const examples: Array<{ name: string; description: string }> = [];

  if (fs.existsSync(EXAMPLES_DIR)) {
    const files = fs.readdirSync(EXAMPLES_DIR);
    for (const file of files) {
      examples.push({
        name: file.replace(/\.(cs|lua|md)$/, ""),
        description: `Example: ${file}`
      });
    }
  }

  return examples;
}

async function getExample(name: string): Promise<{ name: string; path: string; content: string; truncated: boolean } | null> {
  const extensions = [".cs", ".lua", ".md"];

  for (const ext of extensions) {
    const filePath = path.join(EXAMPLES_DIR, `${name}${ext}`);
    if (fs.existsSync(filePath)) {
      const content = fs.readFileSync(filePath, "utf-8");
      const maxLength = 50000;
      const truncated = content.length > maxLength;

      return {
        name,
        path: filePath,
        content: truncated ? content.substring(0, maxLength) + "\n\n... (truncated)" : content,
        truncated
      };
    }
  }

  return null;
}

// =============================================================================
// CREATE MCP SERVER
// =============================================================================

const server = new McpServer({
  name: "rogueessence-mcp-server",
  version: "2.0.0"
});

// =============================================================================
// RESOURCES - Documentation
// =============================================================================

const DOC_FILES = ["architecture", "common-tasks", "api-reference"] as const;

for (const docName of DOC_FILES) {
  server.resource(
    `rogueessence-docs-${docName}`,
    `rogueessence://docs/${docName}`,
    async () => {
      const filePath = path.join(DOCS_DIR, `${docName}.md`);
      try {
        const content = fs.readFileSync(filePath, "utf-8");
        return {
          contents: [{
            uri: `rogueessence://docs/${docName}`,
            mimeType: "text/markdown",
            text: content
          }]
        };
      } catch {
        return {
          contents: [{
            uri: `rogueessence://docs/${docName}`,
            mimeType: "text/plain",
            text: `Error: Could not read ${docName}.md from ${DOCS_DIR}`
          }]
        };
      }
    }
  );
}

// =============================================================================
// RESOURCES - Class categories
// =============================================================================

for (const [category, info] of Object.entries(CLASS_CATEGORIES)) {
  server.resource(
    `rogueessence-classes-${category}`,
    `rogueessence://classes/${category}`,
    async () => {
      const classes = await findClassesInCategory(category as ClassCategory);

      const lines = [
        `# ${category} Classes`,
        "",
        `**Description:** ${info.description}`,
        `**Base Classes:** ${info.baseClasses.map(c => `\`${c}\``).join(", ")}`,
        `**Directory:** \`RogueEssence/${info.dir}\``,
        `**Count:** ${classes.length}`,
        "",
        "## Classes",
        ""
      ];

      for (const cls of classes) {
        lines.push(`### ${cls.name}`);
        if (cls.summary) {
          lines.push(cls.summary);
        }
        lines.push(`- **Base:** \`${cls.baseClass}\``);
        if (cls.properties.length > 0) {
          const propNames = cls.properties.slice(0, 5).map(p => p.name).join(", ");
          const more = cls.properties.length > 5 ? `, ... (+${cls.properties.length - 5} more)` : "";
          lines.push(`- **Properties:** ${propNames}${more}`);
        }
        lines.push("");
      }

      return {
        contents: [{
          uri: `rogueessence://classes/${category}`,
          mimeType: "text/markdown",
          text: lines.join("\n")
        }]
      };
    }
  );
}

// =============================================================================
// PROMPTS - Guided workflows
// =============================================================================

server.prompt(
  "create_gameevent",
  "Step-by-step guide for creating a new GameEvent or BattleEvent",
  () => ({
    messages: [{
      role: "user",
      content: {
        type: "text",
        text: `I need to create a new GameEvent for RogueEssence. Please help me through the process:

1. First, use rogueessence_list_classes with category "game-events" to show me existing examples
2. Ask me what effect I want to create
3. Use rogueessence_scaffold_gameevent to generate the boilerplate
4. Explain what I need to fill in based on similar existing events

Remember:
- GameEvents use IEnumerator<YieldInstruction> for async execution
- Always implement Clone() with the copy constructor pattern
- Use context.ContextStates for battle calculation state
- Use DungeonScene.Instance.LogMsg() for in-game messages`
      }
    }]
  })
);

server.prompt(
  "create_genstep",
  "Step-by-step guide for creating a new GenStep for level generation",
  () => ({
    messages: [{
      role: "user",
      content: {
        type: "text",
        text: `I need to create a new GenStep for RogueEssence. Please help me through the process:

1. First, use rogueessence_list_classes with category "level-generation" to show me existing examples
2. Ask me what generation behavior I want to create
3. Help me choose the right context type:
   - ITiledGenContext: Basic tile operations
   - IFloorPlanGenContext: Room-based planning
   - BaseMapGenContext: Full map features
4. Use rogueessence_scaffold_genstep to generate the boilerplate
5. Explain the Apply() pattern based on similar existing steps

Remember:
- GenSteps are queued with Priority levels
- Access map.RoomPlan for room-based operations
- Access map.Map for direct tile/entity manipulation`
      }
    }]
  })
);

server.prompt(
  "create_menu",
  "Step-by-step guide for creating a new Menu",
  () => ({
    messages: [{
      role: "user",
      content: {
        type: "text",
        text: `I need to create a new Menu for RogueEssence. Please help me through the process:

1. First, use rogueessence_list_classes with category "menus-ui" to show me existing examples
2. Ask me what type of menu I want to create (choice menu, dialog, input, etc.)
3. Help me choose the right base class:
   - InteractableMenu: For menus with selectable options
   - ChoiceMenu: For simple choice lists
   - SingleStripMenu: For horizontal/vertical strips
   - DialogueBox: For text display
4. Use rogueessence_scaffold_menu to generate the boilerplate

Remember:
- Menus use the Initialize() pattern
- Use MenuManager.Instance for menu stack operations
- Draw() is called every frame for rendering`
      }
    }]
  })
);

server.prompt(
  "create_animation",
  "Step-by-step guide for creating a new animation or emitter",
  () => ({
    messages: [{
      role: "user",
      content: {
        type: "text",
        text: `I need to create a new animation or particle effect for RogueEssence. Please help me:

1. First, use rogueessence_list_classes with category "animation-effects" to show me existing examples
2. Ask what type of visual effect I want:
   - BaseAnim: One-shot or looping animations
   - BaseEmitter: Particle systems
3. Use rogueessence_scaffold_animation to generate the boilerplate

Remember:
- Animations use Update() for per-frame logic
- Emitters spawn particles over time
- Use GraphicsManager for sprite loading`
      }
    }]
  })
);

// =============================================================================
// TOOLS - Class browsing and search
// =============================================================================

server.tool(
  "rogueessence_search_classes",
  `Search for RogueEssence classes across all categories by name or summary.

Searches class names and XML documentation summaries. Returns matches ranked by relevance.
Use this when you're not sure which category a class belongs to, or to find classes related to a concept.

Categories searched: ${Object.keys(CLASS_CATEGORIES).join(", ")}`,
  {
    query: z.string()
      .min(2)
      .describe("Search query (e.g., 'damage', 'menu', 'spawn')"),
    limit: z.number()
      .min(1)
      .max(50)
      .default(10)
      .describe("Maximum results to return"),
    response_format: z.enum(["markdown", "json"])
      .default("markdown")
      .describe("Output format")
  },
  async ({ query, limit, response_format }) => {
    const queryLower = query.toLowerCase();
    const queryWords = queryLower.split(/\s+/).filter(w => w.length >= 2);
    const results: Array<{
      name: string;
      category: ClassCategory;
      summary: string;
      score: number;
    }> = [];

    for (const category of Object.keys(CLASS_CATEGORIES) as ClassCategory[]) {
      const classes = await findClassesInCategory(category);

      for (const cls of classes) {
        const nameLower = cls.name.toLowerCase();
        const summaryLower = (cls.summary || "").toLowerCase();
        const propNames = cls.properties.map(p => p.name.toLowerCase()).join(" ");
        const methodNames = cls.methods.map(m => m.name.toLowerCase()).join(" ");
        const allText = `${nameLower} ${summaryLower} ${propNames} ${methodNames}`;

        let score = 1000;

        if (nameLower === queryLower) {
          score = 0;
        } else if (nameLower.startsWith(queryLower)) {
          score = 10;
        } else if (nameLower.includes(queryLower)) {
          score = 20;
        } else if (queryWords.length > 1 && queryWords.every(w => nameLower.includes(w))) {
          score = 25;
        } else if (summaryLower.includes(queryLower)) {
          score = 50;
        } else if (queryWords.length > 1 && queryWords.every(w => summaryLower.includes(w))) {
          score = 55;
        } else if (propNames.includes(queryLower) || methodNames.includes(queryLower)) {
          score = 70;
        } else if (queryWords.some(w => allText.includes(w))) {
          score = 80;
        } else {
          const distance = levenshteinDistance(queryLower, nameLower);
          if (distance <= 3) {
            score = 100 + distance;
          } else {
            continue;
          }
        }

        results.push({
          name: cls.name,
          category,
          summary: cls.summary || "(no documentation)",
          score
        });
      }
    }

    const sorted = results.sort((a, b) => a.score - b.score).slice(0, limit);

    if (sorted.length === 0) {
      return {
        content: [{
          type: "text",
          text: `No classes found matching '${query}'. Try a different search term or use rogueessence_list_classes to browse by category.`
        }]
      };
    }

    if (response_format === "json") {
      return {
        content: [{
          type: "text",
          text: JSON.stringify({ query, count: sorted.length, results: sorted }, null, 2)
        }]
      };
    }

    const lines = [
      `# Search Results: "${query}"`,
      "",
      `Found ${sorted.length} matching classes:`,
      "",
      "| Class | Category | Summary |",
      "|-------|----------|---------|"
    ];

    for (const result of sorted) {
      const summary = result.summary.length > 60
        ? result.summary.substring(0, 57) + "..."
        : result.summary;
      lines.push(`| \`${result.name}\` | ${result.category} | ${summary} |`);
    }

    lines.push("");
    lines.push("*Use `rogueessence_get_class_docs` for full documentation on any class.*");

    return {
      content: [{ type: "text", text: lines.join("\n") }]
    };
  }
);

server.tool(
  "rogueessence_list_classes",
  `List all RogueEssence classes in a specific category.

Categories: ${Object.keys(CLASS_CATEGORIES).join(", ")}

Returns class names with brief summaries from XML documentation.`,
  {
    category: z.enum(Object.keys(CLASS_CATEGORIES) as [ClassCategory, ...ClassCategory[]])
      .describe("Category of classes to list"),
    response_format: z.enum(["markdown", "json"])
      .default("markdown")
      .describe("Output format")
  },
  async ({ category, response_format }) => {
    const classes = await findClassesInCategory(category);
    const categoryInfo = CLASS_CATEGORIES[category];

    if (classes.length === 0) {
      return {
        content: [{
          type: "text",
          text: `No classes found in category '${category}' (searched ${categoryInfo.dir})`
        }]
      };
    }

    const output = {
      category,
      description: categoryInfo.description,
      baseClasses: categoryInfo.baseClasses,
      count: classes.length,
      classes: classes.map(c => ({
        name: c.name,
        summary: c.summary || "(no documentation)",
        baseClass: c.baseClass
      }))
    };

    if (response_format === "json") {
      return {
        content: [{ type: "text", text: JSON.stringify(output, null, 2) }]
      };
    }

    const lines = [
      `# ${category} Classes`,
      "",
      `**Description:** ${categoryInfo.description}`,
      `**Base Classes:** ${categoryInfo.baseClasses.map(c => `\`${c}\``).join(", ")}`,
      `**Count:** ${classes.length}`,
      "",
      "| Class | Summary |",
      "|-------|---------|"
    ];

    for (const cls of classes) {
      const summary = cls.summary ? cls.summary.substring(0, 80) : "(no docs)";
      lines.push(`| \`${cls.name}\` | ${summary} |`);
    }

    return {
      content: [{ type: "text", text: lines.join("\n") }]
    };
  }
);

server.tool(
  "rogueessence_get_class_docs",
  `Get detailed XML documentation for a specific RogueEssence class.

Extracts from C# source files:
- Class summary and remarks
- Namespace and base class
- Public properties with their types and documentation
- Public methods with their signatures and documentation`,
  {
    class_name: z.string()
      .min(1)
      .describe("Name of the class to get documentation for"),
    response_format: z.enum(["markdown", "json"])
      .default("markdown")
      .describe("Output format")
  },
  async ({ class_name, response_format }) => {
    // Debug info
    const debugInfo = {
      __dirname,
      PROJECT_ROOT,
      ROGUEESSENCE_DIR,
      rogueessenceDirExists: fs.existsSync(ROGUEESSENCE_DIR),
      parserInitialized: !!csharpParser
    };

    const classDoc = await findClassByName(class_name);

    if (!classDoc) {
      const suggestions = await findSimilarClasses(class_name, 5);

      let errorMsg = `Class '${class_name}' not found in RogueEssence.\n\n`;
      errorMsg += `**Debug Info:**\n\`\`\`json\n${JSON.stringify(debugInfo, null, 2)}\n\`\`\`\n\n`;

      if (suggestions.length > 0) {
        errorMsg += "**Did you mean one of these?**\n\n";
        for (const suggestion of suggestions) {
          errorMsg += `- \`${suggestion.name}\` (${suggestion.category})\n`;
        }
      }

      return {
        content: [{
          type: "text",
          text: errorMsg
        }]
      };
    }

    if (response_format === "json") {
      return {
        content: [{ type: "text", text: JSON.stringify(classDoc, null, 2) }]
      };
    }

    const lines = [
      `# ${classDoc.name}`,
      "",
      `**Namespace:** \`${classDoc.namespace}\``,
      `**Base Class:** \`${classDoc.baseClass}\``,
      `**File:** \`${classDoc.filePath.replace(PROJECT_ROOT, "")}\``,
      ""
    ];

    if (classDoc.summary) {
      lines.push("## Summary", "", classDoc.summary, "");
    }

    if (classDoc.remarks) {
      lines.push("## Remarks", "", classDoc.remarks, "");
    }

    if (classDoc.properties.length > 0) {
      lines.push("## Properties", "");
      for (const prop of classDoc.properties) {
        lines.push(`### \`${prop.name}\` : \`${prop.type}\``);
        if (prop.summary) lines.push(prop.summary);
        lines.push("");
      }
    }

    if (classDoc.methods.length > 0) {
      lines.push("## Methods", "");
      for (const method of classDoc.methods) {
        lines.push(`### \`${method.signature}\``);
        if (method.summary) lines.push(method.summary);
        lines.push("");
      }
    }

    return {
      content: [{ type: "text", text: lines.join("\n") }]
    };
  }
);

// =============================================================================
// TOOLS - Game data
// =============================================================================

server.tool(
  "rogueessence_get_data",
  `Retrieve a specific game data entry by type and ID.

Supports all RogueEssence data types: ${DATA_TYPES.join(", ")}.

Examples:
  - Get monster: dataType="Monster", id="pikachu"
  - Get skill: dataType="Skill", id="thunderbolt"
  - Get item: dataType="Item", id="oran_berry"`,
  {
    dataType: z.enum(DATA_TYPES)
      .describe("Game data type"),
    id: z.string()
      .min(1)
      .describe("ID of the data entry"),
    response_format: z.enum(["markdown", "json"])
      .default("markdown")
      .describe("Output format")
  },
  async ({ dataType, id, response_format }) => {
    if (!isValidDataType(dataType)) {
      return {
        content: [{
          type: "text",
          text: `Error: Invalid data type '${dataType}'. Valid types: ${DATA_TYPES.join(", ")}`
        }]
      };
    }

    const data = await getDataById(dataType, id);

    if (!data) {
      return {
        content: [{
          type: "text",
          text: `Error: No ${dataType} found with ID '${id}'. Use rogueessence_list_data to see available entries.`
        }]
      };
    }

    if (response_format === "json") {
      return {
        content: [{
          type: "text",
          text: JSON.stringify(data, null, 2)
        }]
      };
    }

    const lines: string[] = [
      `# ${dataType}: ${(data as Record<string, unknown>).name || id}`,
      "",
      `**ID**: ${id}`,
      ""
    ];

    for (const [key, value] of Object.entries(data)) {
      if (key === "id") continue;
      if (typeof value === "object" && value !== null) {
        lines.push(`## ${key}`);
        lines.push("```json");
        lines.push(JSON.stringify(value, null, 2));
        lines.push("```");
        lines.push("");
      } else {
        lines.push(`**${key}**: ${value}`);
      }
    }

    return {
      content: [{
        type: "text",
        text: lines.join("\n")
      }]
    };
  }
);

server.tool(
  "rogueessence_list_data",
  `List or search game data entries by type.

Supports pagination and optional search filtering.

Examples:
  - List all monsters: dataType="Monster"
  - Search fire skills: dataType="Skill", search="fire"`,
  {
    dataType: z.enum(DATA_TYPES)
      .describe("Game data type"),
    search: z.string()
      .optional()
      .describe("Optional search string to filter by name or ID"),
    limit: z.number()
      .int()
      .min(1)
      .max(100)
      .default(20)
      .describe("Maximum results to return"),
    offset: z.number()
      .int()
      .min(0)
      .default(0)
      .describe("Number of results to skip for pagination"),
    response_format: z.enum(["markdown", "json"])
      .default("markdown")
      .describe("Output format")
  },
  async ({ dataType, search, limit, offset, response_format }) => {
    if (!isValidDataType(dataType)) {
      return {
        content: [{
          type: "text",
          text: `Error: Invalid data type '${dataType}'. Valid types: ${DATA_TYPES.join(", ")}`
        }]
      };
    }

    const result = await listData(dataType, { limit, offset, search });

    if (response_format === "json") {
      return {
        content: [{
          type: "text",
          text: JSON.stringify(result, null, 2)
        }]
      };
    }

    const lines: string[] = [
      `# ${dataType} List${search ? ` (search: "${search}")` : ""}`,
      "",
      `Found ${result.total} entries (showing ${result.items.length})`,
      ""
    ];

    for (const item of result.items) {
      lines.push(`- **${item.name || item.id}** (${item.id})`);
    }

    if (result.hasMore) {
      lines.push("");
      lines.push(`*More results available. Use offset=${result.nextOffset} to see next page.*`);
    }

    return {
      content: [{
        type: "text",
        text: lines.join("\n")
      }]
    };
  }
);

server.tool(
  "rogueessence_list_data_types",
  `List all available game data types with entry counts.`,
  {
    response_format: z.enum(["markdown", "json"])
      .default("markdown")
      .describe("Output format")
  },
  async ({ response_format }) => {
    const stats = await getDataTypeStats();

    if (response_format === "json") {
      return {
        content: [{
          type: "text",
          text: JSON.stringify(stats, null, 2)
        }]
      };
    }

    const lines: string[] = [
      "# RogueEssence Data Types",
      "",
      "| Type | Count |",
      "|------|-------|"
    ];

    for (const [type, count] of Object.entries(stats)) {
      lines.push(`| ${type} | ${count} |`);
    }

    return {
      content: [{
        type: "text",
        text: lines.join("\n")
      }]
    };
  }
);

// =============================================================================
// TOOLS - Documentation
// =============================================================================

server.tool(
  "rogueessence_get_docs",
  `Access AI-optimized documentation for RogueEssence development.

Available: architecture, common-tasks, api-reference`,
  {
    name: z.string()
      .optional()
      .describe("Document name. Omit to list all."),
    response_format: z.enum(["markdown", "json"])
      .default("markdown")
      .describe("Output format")
  },
  async ({ name, response_format }) => {
    if (!name) {
      const docs = await listDocs();
      const output = { count: docs.length, documents: docs };

      if (response_format === "json") {
        return {
          content: [{ type: "text", text: JSON.stringify(output, null, 2) }]
        };
      }

      const lines = [
        "# Available Documentation",
        "",
        "| Name | Description |",
        "|------|-------------|"
      ];

      for (const doc of docs) {
        lines.push(`| ${doc.name} | ${doc.description} |`);
      }

      return { content: [{ type: "text", text: lines.join("\n") }] };
    }

    const doc = await getDoc(name);

    if (!doc) {
      return {
        content: [{
          type: "text",
          text: `Error: Document '${name}' not found.`
        }]
      };
    }

    if (response_format === "json") {
      return {
        content: [{ type: "text", text: JSON.stringify(doc, null, 2) }]
      };
    }

    return {
      content: [{ type: "text", text: doc.content }]
    };
  }
);

server.tool(
  "rogueessence_get_example",
  `Access annotated code examples for RogueEssence development.`,
  {
    name: z.string()
      .optional()
      .describe("Example name. Omit to list all."),
    response_format: z.enum(["markdown", "json"])
      .default("markdown")
      .describe("Output format")
  },
  async ({ name, response_format }) => {
    if (!name) {
      const examples = await listExamples();
      const output = { count: examples.length, examples };

      if (response_format === "json") {
        return {
          content: [{ type: "text", text: JSON.stringify(output, null, 2) }]
        };
      }

      const lines = [
        "# Available Examples",
        "",
        "| Name | Description |",
        "|------|-------------|"
      ];

      for (const ex of examples) {
        lines.push(`| ${ex.name} | ${ex.description} |`);
      }

      return { content: [{ type: "text", text: lines.join("\n") }] };
    }

    const example = await getExample(name);

    if (!example) {
      return {
        content: [{
          type: "text",
          text: `Error: Example '${name}' not found.`
        }]
      };
    }

    if (response_format === "json") {
      return {
        content: [{ type: "text", text: JSON.stringify(example, null, 2) }]
      };
    }

    const ext = example.path.split(".").pop() || "";
    const lang = ext === "cs" ? "csharp" : ext === "lua" ? "lua" : ext;

    return {
      content: [{
        type: "text",
        text: `# Example: ${example.name}\n\n\`\`\`${lang}\n${example.content}\n\`\`\``
      }]
    };
  }
);

// =============================================================================
// TOOLS - Scaffolding
// =============================================================================

server.tool(
  "rogueessence_scaffold_gameevent",
  `Generate boilerplate C# code for a new GameEvent or BattleEvent.

Creates a properly structured class with Clone(), Apply(), and constructor pattern.`,
  {
    name: z.string()
      .min(1)
      .describe("Name for the event class"),
    description: z.string()
      .describe("What this event does"),
    event_type: z.enum(["GameEvent", "BattleEvent", "SingleCharEvent"])
      .default("BattleEvent")
      .describe("Base event type")
  },
  async ({ name, description, event_type }) => {
    const className = name.endsWith("Event") ? name : `${name}Event`;

    const code = `using System;
using System.Collections.Generic;
using RogueEssence.Dungeon;
using RogueEssence.Data;
using RogueElements;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// ${description}
    /// </summary>
    [Serializable]
    public class ${className} : ${event_type}
    {
        // Add your configuration properties here
        // public int SomeValue;
        // public string StatusID;

        public ${className}() { }

        protected ${className}(${className} other)
        {
            // Copy all fields from other
        }

        public override GameEvent Clone() { return new ${className}(this); }

        public override IEnumerator<YieldInstruction> Apply(
            GameEventOwner owner,
            Character ownerChar,
            BattleContext context)
        {
            // TODO: Implement your effect logic here
            yield break;
        }
    }
}`;

    return {
      content: [{ type: "text", text: code }]
    };
  }
);

server.tool(
  "rogueessence_scaffold_genstep",
  `Generate boilerplate C# code for a new GenStep.`,
  {
    name: z.string()
      .min(1)
      .describe("Name for the GenStep class"),
    description: z.string()
      .describe("What this generation step does"),
    context_type: z.enum(["ITiledGenContext", "IFloorPlanGenContext", "BaseMapGenContext"])
      .default("BaseMapGenContext")
      .describe("The context interface this step requires")
  },
  async ({ name, description, context_type }) => {
    const className = name.endsWith("Step") ? name : `${name}Step`;

    const code = `using System;
using System.Collections.Generic;
using RogueElements;
using RogueEssence.LevelGen;
using RogueEssence.Dungeon;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// ${description}
    /// </summary>
    [Serializable]
    public class ${className}<T> : GenStep<T> where T : class, ${context_type}
    {
        public ${className}() { }

        public override void Apply(T map)
        {
            // TODO: Implement your generation logic here
        }
    }
}`;

    return {
      content: [{ type: "text", text: code }]
    };
  }
);

server.tool(
  "rogueessence_scaffold_menu",
  `Generate boilerplate C# code for a new Menu.`,
  {
    name: z.string()
      .min(1)
      .describe("Name for the Menu class"),
    description: z.string()
      .describe("What this menu does"),
    menu_type: z.enum(["InteractableMenu", "ChoiceMenu", "SingleStripMenu", "DialogueBox"])
      .default("InteractableMenu")
      .describe("Base menu type")
  },
  async ({ name, description, menu_type }) => {
    const className = name.endsWith("Menu") ? name : `${name}Menu`;

    const code = `using System;
using System.Collections.Generic;
using RogueEssence.Content;
using RogueElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueEssence.Menu
{
    /// <summary>
    /// ${description}
    /// </summary>
    public class ${className} : ${menu_type}
    {
        public ${className}()
        {
            // Initialize menu bounds
            Bounds = new Rect(0, 0, 200, 150);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw menu background
            if (!Visible)
                return;

            base.Draw(spriteBatch);

            // TODO: Draw your menu content here
        }

        public override void Update(InputManager input)
        {
            // Handle input
            if (input.JustPressed(FrameInput.InputType.Cancel))
            {
                MenuManager.Instance.RemoveMenu();
                return;
            }

            base.Update(input);
        }
    }
}`;

    return {
      content: [{ type: "text", text: code }]
    };
  }
);

server.tool(
  "rogueessence_scaffold_animation",
  `Generate boilerplate C# code for a new animation or emitter.`,
  {
    name: z.string()
      .min(1)
      .describe("Name for the animation class"),
    description: z.string()
      .describe("What this animation does"),
    anim_type: z.enum(["BaseAnim", "LoopingAnim", "BaseEmitter"])
      .default("BaseAnim")
      .describe("Base animation type")
  },
  async ({ name, description, anim_type }) => {
    const className = name.endsWith("Anim") || name.endsWith("Emitter") ? name : `${name}Anim`;

    if (anim_type === "BaseEmitter") {
      const code = `using System;
using System.Collections.Generic;
using RogueElements;
using Microsoft.Xna.Framework;

namespace RogueEssence.Content
{
    /// <summary>
    /// ${description}
    /// </summary>
    [Serializable]
    public class ${className} : FiniteEmitter
    {
        /// <summary>
        /// Number of particles to emit.
        /// </summary>
        public int ParticleCount;

        /// <summary>
        /// Animation to spawn for each particle.
        /// </summary>
        public IEmittable Anim;

        private bool finished;
        public override bool Finished { get { return finished; } }

        public ${className}() { }

        public ${className}(IEmittable anim, int particleCount)
        {
            Anim = anim;
            ParticleCount = particleCount;
        }

        protected ${className}(${className} other)
        {
            ParticleCount = other.ParticleCount;
            if (other.Anim != null)
                Anim = other.Anim.CloneIEmittable();
        }

        public override BaseEmitter Clone() { return new ${className}(this); }

        public override void Update(BaseScene scene, FrameTick elapsedTime)
        {
            // TODO: Spawn particles using scene.Anims.Add()
            // Use Origin, Destination, Dir from base class (set by SetupEmit)
            // Mark finished = true when done emitting
            finished = true;
        }
    }
}`;
      return { content: [{ type: "text", text: code }] };
    }

    const code = `using System;
using RogueElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueEssence.Content
{
    /// <summary>
    /// ${description}
    /// </summary>
    [Serializable]
    public class ${className} : ${anim_type}, IFinishableSprite
    {
        public Loc MapLoc { get { return mapLoc; } }
        public int LocHeight { get { return locHeight; } }
        public bool Finished { get { return finished; } }

        public ${className}() { }

        public ${className}(Loc pos, int height)
        {
            mapLoc = pos;
            locHeight = height;
        }

        protected ${className}(${className} other)
        {
            mapLoc = other.mapLoc;
            locHeight = other.locHeight;
        }

        public override void Update(BaseScene scene, FrameTick elapsedTime)
        {
            // TODO: Update animation state
            // Set finished = true when animation is complete
        }

        public override void Draw(SpriteBatch spriteBatch, Loc offset)
        {
            if (Finished)
                return;

            // TODO: Draw animation using GraphicsManager.GetSprite()
            Loc drawLoc = GetDrawLoc(offset);
        }

        public override Loc GetDrawLoc(Loc offset)
        {
            return new Loc(mapLoc.X - offset.X, mapLoc.Y - locHeight - offset.Y);
        }

        public override Loc GetSheetOffset() { return Loc.Zero; }

        public override Loc GetDrawSize()
        {
            // TODO: Return actual sprite size
            return new Loc(GraphicsManager.TileSize, GraphicsManager.TileSize);
        }

        public void DrawDebug(SpriteBatch spriteBatch, Loc offset) { }
    }
}`;

    return {
      content: [{ type: "text", text: code }]
    };
  }
);

server.tool(
  "rogueessence_scaffold_roomgen",
  `Generate boilerplate C# code for a new RoomGen.`,
  {
    name: z.string()
      .min(1)
      .describe("Name for the RoomGen class (e.g., 'LShape', 'Diamond')"),
    description: z.string()
      .describe("What shape this room generator creates")
  },
  async ({ name, description }) => {
    const className = name.startsWith("RoomGen") ? name : `RoomGen${name}`;

    const code = `using System;
using System.Collections.Generic;
using RogueElements;
using RogueEssence.LevelGen;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// ${description}
    /// </summary>
    [Serializable]
    public class ${className}<T> : RoomGen<T> where T : ITiledGenContext
    {
        /// <summary>
        /// Width of the room.
        /// </summary>
        public RandRange Width;

        /// <summary>
        /// Height of the room.
        /// </summary>
        public RandRange Height;

        public ${className}() { }

        public ${className}(RandRange width, RandRange height)
        {
            Width = width;
            Height = height;
        }

        protected ${className}(${className}<T> other)
        {
            Width = other.Width;
            Height = other.Height;
        }

        public override RoomGen<T> Copy() { return new ${className}<T>(this); }

        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(Width.Pick(rand), Height.Pick(rand));
        }

        public override void DrawOnMap(T map)
        {
            Rect roomRect = new Rect(Draw.Start, Draw.Size);

            // TODO: Draw your room shape here
            for (int x = roomRect.X; x < roomRect.End.X; x++)
            {
                for (int y = roomRect.Y; y < roomRect.End.Y; y++)
                {
                    map.SetTile(new Loc(x, y), map.RoomTerrain.Copy());
                }
            }

            SetRoomBorders(map);
        }

        protected override void PrepareFulfillableBorders(IRandom rand)
        {
            // Mark which border tiles can accept hall connections
            // FulfillableBorder is a bool array indexed by tile position
            for (int ii = 0; ii < Draw.Width; ii++)
            {
                // TODO: Set to true only for tiles that are walkable on border
                FulfillableBorder[Dir4.Up][ii] = true;
                FulfillableBorder[Dir4.Down][ii] = true;
            }

            for (int ii = 0; ii < Draw.Height; ii++)
            {
                FulfillableBorder[Dir4.Left][ii] = true;
                FulfillableBorder[Dir4.Right][ii] = true;
            }
        }
    }
}`;

    return {
      content: [{ type: "text", text: code }]
    };
  }
);

server.tool(
  "rogueessence_scaffold_groundentity",
  `Generate boilerplate C# code for a new GroundEntity.`,
  {
    name: z.string()
      .min(1)
      .describe("Name for the GroundEntity class"),
    description: z.string()
      .describe("What this ground entity does")
  },
  async ({ name, description }) => {
    const className = name;

    const code = `using System;
using System.Runtime.Serialization;
using RogueElements;
using RogueEssence.Script;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueEssence.Ground
{
    /// <summary>
    /// ${description}
    /// </summary>
    [Serializable]
    public class ${className} : GroundEntity
    {
        public override Color DevEntColoring => Color.Cyan;

        public ${className}() { }

        public ${className}(string name, Loc pos, Dir8 dir)
        {
            EntName = name;
            Position = pos;
            Direction = dir;
            Bounds = new Rect(pos.X, pos.Y, GroundAction.HITBOX_WIDTH, GroundAction.HITBOX_HEIGHT);
        }

        protected ${className}(${className} other) : base(other) { }

        public override GroundEntity Clone() { return new ${className}(this); }

        public override EEntTypes GetEntityType()
        {
            return EEntTypes.Object;
        }

        public override bool DevHasGraphics()
        {
            // Return true if this entity has a visual representation
            return false;
        }

        public override void Draw(SpriteBatch spriteBatch, Loc offset)
        {
            // TODO: Draw entity if DevHasGraphics returns true
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext cntxt)
        {
            // Restore any transient state after deserialization
        }
    }
}`;

    return {
      content: [{ type: "text", text: code }]
    };
  }
);

// =============================================================================
// SERVER STARTUP
// =============================================================================

async function main() {
  console.error(`RogueEssence MCP Server v2.0.0`);
  console.error(`Project root: ${PROJECT_ROOT}`);
  console.error(`Data dir: ${DATA_DIR}`);

  // Initialize parser at startup (before stdio transport)
  // This ensures WASM loading completes before stdio is connected
  try {
    console.error("Initializing tree-sitter parser...");
    await initializeParser();
    console.error("Parser initialized successfully");
  } catch (err) {
    console.error("Parser initialization failed:", err);
    // Continue without parser - code search won't work but other features will
  }

  const transport = new StdioServerTransport();
  await server.connect(transport);
  console.error("MCP server running via stdio");
}

main().catch((error) => {
  console.error("Server error:", error);
  process.exit(1);
});
