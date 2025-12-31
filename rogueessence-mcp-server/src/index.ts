#!/usr/bin/env node
/**
 * RogueEssence MCP Server
 *
 * Provides tools for Claude to interact with the RogueEssence game engine:
 * - Query game data (monsters, skills, items, zones, etc.)
 * - Search and explore the codebase
 * - Access AI-optimized documentation and examples
 */

import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import { registerDataTools } from "./tools/data-tools.js";
import { registerCodeTools } from "./tools/code-tools.js";
import { registerDocsTools } from "./tools/docs-tools.js";
import { PROJECT_ROOT } from "./constants.js";

// Create MCP server instance
const server = new McpServer({
  name: "rogueessence-mcp-server",
  version: "1.0.0"
});

// Register all tool groups
registerDataTools(server);
registerCodeTools(server);
registerDocsTools(server);

// Main function
async function main() {
  // Log to stderr (stdout is reserved for MCP protocol)
  console.error(`RogueEssence MCP Server v1.0.0`);
  console.error(`Project root: ${PROJECT_ROOT}`);

  const transport = new StdioServerTransport();
  await server.connect(transport);

  console.error("MCP server running via stdio");
}

main().catch((error) => {
  console.error("Server error:", error);
  process.exit(1);
});
