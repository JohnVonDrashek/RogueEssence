import { z } from "zod";
import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { getDataById, listData, isValidDataType, getDataTypeStats } from "../services/data-reader.js";
import { ResponseFormat, ResponseFormatSchema } from "../schemas/common.js";
import { DATA_TYPES, DataType } from "../constants.js";

const DataTypeSchema = z.enum(DATA_TYPES as unknown as [string, ...string[]])
  .describe(`Game data type: ${DATA_TYPES.join(", ")}`);

const GetDataInputSchema = z.object({
  dataType: DataTypeSchema,
  id: z.string()
    .min(1)
    .describe("ID of the data entry (e.g., 'bulbasaur', 'tackle', 'oran_berry')"),
  response_format: ResponseFormatSchema
}).strict();

const ListDataInputSchema = z.object({
  dataType: DataTypeSchema,
  search: z.string()
    .optional()
    .describe("Optional search string to filter by name or ID"),
  limit: z.number()
    .int()
    .min(1)
    .max(100)
    .default(20)
    .describe("Maximum results to return (1-100)"),
  offset: z.number()
    .int()
    .min(0)
    .default(0)
    .describe("Number of results to skip for pagination"),
  response_format: ResponseFormatSchema
}).strict();

const ListDataTypesInputSchema = z.object({
  response_format: ResponseFormatSchema
}).strict();

type GetDataInput = z.infer<typeof GetDataInputSchema>;
type ListDataInput = z.infer<typeof ListDataInputSchema>;
type ListDataTypesInput = z.infer<typeof ListDataTypesInputSchema>;

export function registerDataTools(server: McpServer): void {
  // Get specific data entry by type and ID
  server.registerTool(
    "rogueessence_get_data",
    {
      title: "Get RogueEssence Game Data",
      description: `Retrieve a specific game data entry by type and ID.

Supports all RogueEssence data types: Monster, Skill, Item, Intrinsic, Status, MapStatus, Terrain, Tile, Zone, Emote, AutoTile, Element, GrowthGroup, SkillGroup, AI, Rank, Skin.

Args:
  - dataType (string): The type of data to retrieve
  - id (string): The ID of the entry (e.g., "bulbasaur", "tackle")
  - response_format ('markdown' | 'json'): Output format

Returns:
  The complete data entry with all fields, or an error if not found.

Examples:
  - Get monster: dataType="Monster", id="pikachu"
  - Get skill: dataType="Skill", id="thunderbolt"
  - Get item: dataType="Item", id="oran_berry"`,
      inputSchema: GetDataInputSchema,
      annotations: {
        readOnlyHint: true,
        destructiveHint: false,
        idempotentHint: true,
        openWorldHint: false
      }
    },
    async (params: GetDataInput) => {
      const { dataType, id, response_format } = params;

      if (!isValidDataType(dataType)) {
        return {
          content: [{
            type: "text",
            text: `Error: Invalid data type '${dataType}'. Valid types: ${DATA_TYPES.join(", ")}`
          }]
        };
      }

      const data = await getDataById(dataType as DataType, id);

      if (!data) {
        return {
          content: [{
            type: "text",
            text: `Error: No ${dataType} found with ID '${id}'. Use rogueessence_list_data to see available entries.`
          }]
        };
      }

      if (response_format === ResponseFormat.JSON) {
        return {
          content: [{
            type: "text",
            text: JSON.stringify(data, null, 2)
          }],
          structuredContent: data
        };
      }

      // Markdown format
      const lines: string[] = [
        `# ${dataType}: ${data.name || id}`,
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
        }],
        structuredContent: data
      };
    }
  );

  // List/search data entries
  server.registerTool(
    "rogueessence_list_data",
    {
      title: "List RogueEssence Game Data",
      description: `List or search game data entries by type.

Supports pagination and optional search filtering.

Args:
  - dataType (string): The type of data to list
  - search (string, optional): Filter results by name or ID
  - limit (number): Maximum results (1-100, default: 20)
  - offset (number): Skip first N results for pagination
  - response_format ('markdown' | 'json'): Output format

Returns:
  List of matching entries with pagination info.

Examples:
  - List all monsters: dataType="Monster"
  - Search fire skills: dataType="Skill", search="fire"
  - Get next page: dataType="Item", offset=20`,
      inputSchema: ListDataInputSchema,
      annotations: {
        readOnlyHint: true,
        destructiveHint: false,
        idempotentHint: true,
        openWorldHint: false
      }
    },
    async (params: ListDataInput) => {
      const { dataType, search, limit, offset, response_format } = params;

      if (!isValidDataType(dataType)) {
        return {
          content: [{
            type: "text",
            text: `Error: Invalid data type '${dataType}'. Valid types: ${DATA_TYPES.join(", ")}`
          }]
        };
      }

      const result = await listData(dataType as DataType, { limit, offset, search });

      const output = {
        dataType,
        total: result.total,
        count: result.items.length,
        offset,
        items: result.items.map(item => ({
          id: item.id,
          name: item.name || item.id
        })),
        hasMore: result.hasMore,
        ...(result.nextOffset !== undefined ? { nextOffset: result.nextOffset } : {})
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
        }],
        structuredContent: output
      };
    }
  );

  // List available data types with counts
  server.registerTool(
    "rogueessence_list_data_types",
    {
      title: "List RogueEssence Data Types",
      description: `List all available game data types with entry counts.

This helps discover what data is available in the RogueEssence engine.

Returns:
  All data types with the number of entries in each category.`,
      inputSchema: ListDataTypesInputSchema,
      annotations: {
        readOnlyHint: true,
        destructiveHint: false,
        idempotentHint: true,
        openWorldHint: false
      }
    },
    async (params: ListDataTypesInput) => {
      const { response_format } = params;
      const stats = await getDataTypeStats();

      const output = {
        dataTypes: Object.entries(stats).map(([type, count]) => ({
          type,
          count
        })),
        total: Object.values(stats).reduce((a, b) => a + b, 0)
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
        "# RogueEssence Data Types",
        "",
        "| Type | Count |",
        "|------|-------|"
      ];

      for (const [type, count] of Object.entries(stats)) {
        lines.push(`| ${type} | ${count} |`);
      }

      lines.push("");
      lines.push(`**Total entries**: ${output.total}`);

      return {
        content: [{
          type: "text",
          text: lines.join("\n")
        }],
        structuredContent: output
      };
    }
  );
}
