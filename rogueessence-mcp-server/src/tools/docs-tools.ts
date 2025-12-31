import { z } from "zod";
import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { listDocs, getDoc, listExamples, getExample, getArchitectureOverview } from "../services/docs-reader.js";
import { ResponseFormat, ResponseFormatSchema } from "../schemas/common.js";

const GetDocsInputSchema = z.object({
  name: z.string()
    .optional()
    .describe("Document name (e.g., 'architecture', 'common-tasks', 'api-reference'). Omit to list all."),
  response_format: ResponseFormatSchema
}).strict();

const GetExampleInputSchema = z.object({
  name: z.string()
    .optional()
    .describe("Example name (e.g., 'monster-creation', 'floor-generation'). Omit to list all."),
  response_format: ResponseFormatSchema
}).strict();

const GetArchitectureInputSchema = z.object({
  response_format: ResponseFormatSchema
}).strict();

type GetDocsInput = z.infer<typeof GetDocsInputSchema>;
type GetExampleInput = z.infer<typeof GetExampleInputSchema>;
type GetArchitectureInput = z.infer<typeof GetArchitectureInputSchema>;

export function registerDocsTools(server: McpServer): void {
  // Get documentation
  server.registerTool(
    "rogueessence_get_docs",
    {
      title: "Get RogueEssence Documentation",
      description: `Access AI-optimized documentation for RogueEssence development.

Available documentation:
- architecture: System overview, subsystems, data flow
- common-tasks: Step-by-step guides for common development tasks
- api-reference: Quick reference for key classes and methods

Args:
  - name (string, optional): Document name. Omit to list all available docs.
  - response_format ('markdown' | 'json'): Output format

Returns:
  Document content or list of available documents.

Examples:
  - List docs: (no name parameter)
  - Get architecture: name="architecture"
  - Get task guide: name="common-tasks"`,
      inputSchema: GetDocsInputSchema,
      annotations: {
        readOnlyHint: true,
        destructiveHint: false,
        idempotentHint: true,
        openWorldHint: false
      }
    },
    async (params: GetDocsInput) => {
      const { name, response_format } = params;

      // If no name, list available docs
      if (!name) {
        const docs = await listDocs();

        const output = {
          count: docs.length,
          documents: docs
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

        const lines: string[] = [
          "# Available RogueEssence Documentation",
          "",
          "| Name | Description |",
          "|------|-------------|"
        ];

        for (const doc of docs) {
          lines.push(`| ${doc.name} | ${doc.description || ""} |`);
        }

        lines.push("");
        lines.push("Use `rogueessence_get_docs` with `name` parameter to read a specific document.");

        return {
          content: [{
            type: "text",
            text: lines.join("\n")
          }],
          structuredContent: output
        };
      }

      // Get specific document
      const doc = await getDoc(name);

      if (!doc) {
        return {
          content: [{
            type: "text",
            text: `Error: Document '${name}' not found. Use rogueessence_get_docs without name to see available documents.`
          }]
        };
      }

      const output = {
        name: doc.name,
        path: doc.path,
        content: doc.content,
        truncated: doc.truncated
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

      return {
        content: [{
          type: "text",
          text: doc.content
        }],
        structuredContent: output
      };
    }
  );

  // Get code examples
  server.registerTool(
    "rogueessence_get_example",
    {
      title: "Get RogueEssence Code Example",
      description: `Access annotated code examples for RogueEssence development.

Examples cover common patterns:
- Monster creation and configuration
- Skill implementation
- Item effects
- Status effects
- Floor generation
- Battle events
- Menu systems
- Lua scripting

Args:
  - name (string, optional): Example name. Omit to list all available examples.
  - response_format ('markdown' | 'json'): Output format

Returns:
  Example code with annotations, or list of available examples.

Examples:
  - List examples: (no name parameter)
  - Get monster example: name="monster-creation"
  - Get skill example: name="skill-implementation"`,
      inputSchema: GetExampleInputSchema,
      annotations: {
        readOnlyHint: true,
        destructiveHint: false,
        idempotentHint: true,
        openWorldHint: false
      }
    },
    async (params: GetExampleInput) => {
      const { name, response_format } = params;

      // If no name, list available examples
      if (!name) {
        const examples = await listExamples();

        const output = {
          count: examples.length,
          examples
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

        const lines: string[] = [
          "# Available RogueEssence Code Examples",
          "",
          "| Name | Description |",
          "|------|-------------|"
        ];

        for (const example of examples) {
          lines.push(`| ${example.name} | ${example.description || ""} |`);
        }

        lines.push("");
        lines.push("Use `rogueessence_get_example` with `name` parameter to read a specific example.");

        return {
          content: [{
            type: "text",
            text: lines.join("\n")
          }],
          structuredContent: output
        };
      }

      // Get specific example
      const example = await getExample(name);

      if (!example) {
        return {
          content: [{
            type: "text",
            text: `Error: Example '${name}' not found. Use rogueessence_get_example without name to see available examples.`
          }]
        };
      }

      const output = {
        name: example.name,
        path: example.path,
        content: example.content,
        truncated: example.truncated
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

      // Determine language for syntax highlighting
      const ext = example.path.split(".").pop() || "";
      const lang = ext === "cs" ? "csharp" : ext === "lua" ? "lua" : ext;

      return {
        content: [{
          type: "text",
          text: `# Example: ${example.name}\n\n\`\`\`${lang}\n${example.content}\n\`\`\``
        }],
        structuredContent: output
      };
    }
  );

  // Get architecture overview
  server.registerTool(
    "rogueessence_get_architecture",
    {
      title: "Get RogueEssence Architecture Overview",
      description: `Get a comprehensive overview of the RogueEssence architecture.

Combines information from CLAUDE.md and docs/claude/architecture.md to provide
a complete understanding of the engine structure.

Returns:
  Complete architecture documentation including:
  - Project structure
  - Key subsystems
  - Data flow
  - Common patterns`,
      inputSchema: GetArchitectureInputSchema,
      annotations: {
        readOnlyHint: true,
        destructiveHint: false,
        idempotentHint: true,
        openWorldHint: false
      }
    },
    async (params: GetArchitectureInput) => {
      const { response_format } = params;

      const overview = await getArchitectureOverview();

      const output = {
        content: overview
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

      return {
        content: [{
          type: "text",
          text: overview
        }],
        structuredContent: output
      };
    }
  );
}
