import { z } from "zod";
import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { searchCode, getClassDefinition, findFiles, getFileContent } from "../services/code-search.js";
import { ResponseFormat, ResponseFormatSchema } from "../schemas/common.js";

const SearchCodeInputSchema = z.object({
  pattern: z.string()
    .min(2)
    .describe("Search pattern (regex supported, e.g., 'class.*Manager', 'yield return')"),
  filePattern: z.string()
    .default("*.cs")
    .describe("File glob pattern (e.g., '*.cs', '*.lua', '*Scene*.cs')"),
  caseSensitive: z.boolean()
    .default(false)
    .describe("Whether the search is case-sensitive"),
  limit: z.number()
    .int()
    .min(1)
    .max(100)
    .default(50)
    .describe("Maximum results to return"),
  contextLines: z.number()
    .int()
    .min(0)
    .max(10)
    .default(2)
    .describe("Number of context lines before/after each match"),
  response_format: ResponseFormatSchema
}).strict();

const GetClassInputSchema = z.object({
  className: z.string()
    .min(1)
    .describe("Name of the class to find (e.g., 'DungeonScene', 'Character', 'DataManager')"),
  response_format: ResponseFormatSchema
}).strict();

const FindFilesInputSchema = z.object({
  pattern: z.string()
    .min(1)
    .describe("File glob pattern (e.g., '*Manager.cs', 'Scene/*.cs', '*.lua')"),
  limit: z.number()
    .int()
    .min(1)
    .max(100)
    .default(50)
    .describe("Maximum results to return"),
  response_format: ResponseFormatSchema
}).strict();

const ReadFileInputSchema = z.object({
  path: z.string()
    .min(1)
    .describe("Path to file (relative to project root or absolute)"),
  response_format: ResponseFormatSchema
}).strict();

type SearchCodeInput = z.infer<typeof SearchCodeInputSchema>;
type GetClassInput = z.infer<typeof GetClassInputSchema>;
type FindFilesInput = z.infer<typeof FindFilesInputSchema>;
type ReadFileInput = z.infer<typeof ReadFileInputSchema>;

export function registerCodeTools(server: McpServer): void {
  // Search code across the codebase
  server.registerTool(
    "rogueessence_search_code",
    {
      title: "Search RogueEssence Code",
      description: `Search the RogueEssence codebase for patterns using regex.

Searches across RogueEssence, RogueEssence.Editor.Avalonia, and WaypointServer projects.

Args:
  - pattern (string): Search pattern (regex supported)
  - filePattern (string): File glob pattern (default: "*.cs")
  - caseSensitive (boolean): Case-sensitive search (default: false)
  - limit (number): Maximum results (default: 50)
  - contextLines (number): Lines of context around matches (default: 2)
  - response_format ('markdown' | 'json'): Output format

Returns:
  List of matches with file paths, line numbers, and context.

Examples:
  - Find all managers: pattern="class.*Manager"
  - Find yield statements: pattern="yield return"
  - Find in Lua files: pattern="function", filePattern="*.lua"`,
      inputSchema: SearchCodeInputSchema,
      annotations: {
        readOnlyHint: true,
        destructiveHint: false,
        idempotentHint: true,
        openWorldHint: false
      }
    },
    async (params: SearchCodeInput) => {
      const { pattern, filePattern, caseSensitive, limit, contextLines, response_format } = params;

      try {
        const results = await searchCode(pattern, { filePattern, caseSensitive, limit, contextLines });

        const output = {
          pattern,
          filePattern,
          count: results.length,
          results: results.map(r => ({
            file: r.file,
            line: r.line,
            content: r.content,
            context: r.context
          }))
        };

        if (response_format === ResponseFormat.JSON) {
          return {
            content: [{
              type: "text",
              text: JSON.stringify(output, null, 2)
            }],
            structuredContent: output
          };
        }

        // Markdown format
        const lines: string[] = [
          `# Search Results: \`${pattern}\``,
          "",
          `Found ${results.length} matches in ${filePattern} files`,
          ""
        ];

        for (const result of results) {
          lines.push(`## ${result.file}:${result.line}`);
          lines.push("```csharp");
          if (result.context) {
            lines.push(result.context.join("\n"));
          } else {
            lines.push(result.content);
          }
          lines.push("```");
          lines.push("");
        }

        return {
          content: [{
            type: "text",
            text: lines.join("\n")
          }],
          structuredContent: output
        };
      } catch (error) {
        return {
          content: [{
            type: "text",
            text: `Error searching code: ${error instanceof Error ? error.message : String(error)}`
          }]
        };
      }
    }
  );

  // Get class definition
  server.registerTool(
    "rogueessence_get_class",
    {
      title: "Get RogueEssence Class Definition",
      description: `Retrieve the full definition of a C# class from the codebase.

Includes namespace, base class, interfaces, XML documentation, and the complete class body.

Args:
  - className (string): Name of the class to find
  - response_format ('markdown' | 'json'): Output format

Returns:
  Complete class definition with metadata, or error if not found.

Examples:
  - Get scene class: className="DungeonScene"
  - Get manager: className="DataManager"
  - Get character: className="Character"`,
      inputSchema: GetClassInputSchema,
      annotations: {
        readOnlyHint: true,
        destructiveHint: false,
        idempotentHint: true,
        openWorldHint: false
      }
    },
    async (params: GetClassInput) => {
      const { className, response_format } = params;

      const classInfo = await getClassDefinition(className);

      if (!classInfo) {
        return {
          content: [{
            type: "text",
            text: `Error: Class '${className}' not found. Try rogueessence_search_code with pattern="class ${className}".`
          }]
        };
      }

      const output = {
        name: classInfo.name,
        file: classInfo.file,
        line: classInfo.line,
        namespace: classInfo.namespace,
        baseClass: classInfo.baseClass,
        interfaces: classInfo.interfaces,
        summary: classInfo.summary,
        content: classInfo.content
      };

      if (response_format === ResponseFormat.JSON) {
        return {
          content: [{
            type: "text",
            text: JSON.stringify(output, null, 2)
          }],
          structuredContent: output
        };
      }

      // Markdown format
      const lines: string[] = [
        `# Class: ${classInfo.name}`,
        "",
        `**File**: ${classInfo.file}:${classInfo.line}`
      ];

      if (classInfo.namespace) {
        lines.push(`**Namespace**: ${classInfo.namespace}`);
      }
      if (classInfo.baseClass) {
        lines.push(`**Base Class**: ${classInfo.baseClass}`);
      }
      if (classInfo.interfaces?.length) {
        lines.push(`**Interfaces**: ${classInfo.interfaces.join(", ")}`);
      }
      if (classInfo.summary) {
        lines.push("");
        lines.push("## Summary");
        lines.push(classInfo.summary);
      }

      lines.push("");
      lines.push("## Definition");
      lines.push("```csharp");
      lines.push(classInfo.content);
      lines.push("```");

      return {
        content: [{
          type: "text",
          text: lines.join("\n")
        }],
        structuredContent: output
      };
    }
  );

  // Find files by pattern
  server.registerTool(
    "rogueessence_find_files",
    {
      title: "Find RogueEssence Files",
      description: `Find files in the codebase matching a glob pattern.

Searches across all source directories (RogueEssence, Editor, WaypointServer).

Args:
  - pattern (string): Glob pattern (e.g., "*Scene*.cs", "*.lua")
  - limit (number): Maximum results (default: 50)
  - response_format ('markdown' | 'json'): Output format

Returns:
  List of matching files with paths and sizes.

Examples:
  - Find scene files: pattern="*Scene*.cs"
  - Find Lua scripts: pattern="*.lua"
  - Find managers: pattern="*Manager.cs"`,
      inputSchema: FindFilesInputSchema,
      annotations: {
        readOnlyHint: true,
        destructiveHint: false,
        idempotentHint: true,
        openWorldHint: false
      }
    },
    async (params: FindFilesInput) => {
      const { pattern, limit, response_format } = params;

      const files = await findFiles(pattern, { limit });

      const output = {
        pattern,
        count: files.length,
        files: files.map(f => ({
          path: f.relativePath,
          size: f.size,
          extension: f.extension
        }))
      };

      if (response_format === ResponseFormat.JSON) {
        return {
          content: [{
            type: "text",
            text: JSON.stringify(output, null, 2)
          }],
          structuredContent: output
        };
      }

      // Markdown format
      const lines: string[] = [
        `# Files Matching: \`${pattern}\``,
        "",
        `Found ${files.length} files`,
        "",
        "| File | Size |",
        "|------|------|"
      ];

      for (const file of files) {
        const sizeKB = (file.size / 1024).toFixed(1);
        lines.push(`| ${file.relativePath} | ${sizeKB} KB |`);
      }

      return {
        content: [{
          type: "text",
          text: lines.join("\n")
        }],
        structuredContent: output
      };
    }
  );

  // Read file content
  server.registerTool(
    "rogueessence_read_file",
    {
      title: "Read RogueEssence File",
      description: `Read the contents of a file from the codebase.

Supports both relative paths (from project root) and absolute paths.
Large files are automatically truncated.

Args:
  - path (string): Path to the file
  - response_format ('markdown' | 'json'): Output format

Returns:
  File contents (may be truncated for large files).

Examples:
  - Read a class: path="RogueEssence/Dungeon/DungeonScene.cs"
  - Read config: path="CLAUDE.md"`,
      inputSchema: ReadFileInputSchema,
      annotations: {
        readOnlyHint: true,
        destructiveHint: false,
        idempotentHint: true,
        openWorldHint: false
      }
    },
    async (params: ReadFileInput) => {
      const { path, response_format } = params;

      const content = await getFileContent(path);

      if (content === null) {
        return {
          content: [{
            type: "text",
            text: `Error: File not found: ${path}`
          }]
        };
      }

      const output = {
        path,
        content,
        truncated: content.includes("// ... truncated")
      };

      if (response_format === ResponseFormat.JSON) {
        return {
          content: [{
            type: "text",
            text: JSON.stringify(output, null, 2)
          }],
          structuredContent: output
        };
      }

      // Markdown format
      const ext = path.split(".").pop() || "";
      const lang = ext === "cs" ? "csharp" : ext === "lua" ? "lua" : ext;

      return {
        content: [{
          type: "text",
          text: `# File: ${path}\n\n\`\`\`${lang}\n${content}\n\`\`\``
        }],
        structuredContent: output
      };
    }
  );
}
