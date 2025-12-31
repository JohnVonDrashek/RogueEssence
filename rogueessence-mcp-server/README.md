# RogueEssence MCP Server

[![MCP](https://img.shields.io/badge/MCP-1.0-blue)](https://modelcontextprotocol.io/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.7-blue)](https://www.typescriptlang.org/)
[![Node.js](https://img.shields.io/badge/Node.js-18+-green)](https://nodejs.org/)

An MCP (Model Context Protocol) server that enables Claude to interact with the RogueEssence game engine. Query game data, search the codebase, and access AI-optimized documentation.

## Features

### Data Query Tools
- `rogueessence_get_data` - Get specific game data by type and ID
- `rogueessence_list_data` - List/search game data with pagination
- `rogueessence_list_data_types` - List all data types with counts

### Code Search Tools
- `rogueessence_search_code` - Search codebase with regex patterns
- `rogueessence_get_class` - Get full class definitions
- `rogueessence_find_files` - Find files by glob pattern
- `rogueessence_read_file` - Read file contents

### Documentation Tools
- `rogueessence_get_docs` - Access AI-optimized documentation
- `rogueessence_get_example` - Get annotated code examples
- `rogueessence_get_architecture` - Get architecture overview

## Installation

```bash
cd rogueessence-mcp-server
npm install
npm run build
```

## Usage

### With Claude Code

Add to your Claude Code MCP configuration (`~/.claude/claude_code_config.json`):

```json
{
  "mcpServers": {
    "rogueessence": {
      "command": "node",
      "args": ["/path/to/RogueEssence/rogueessence-mcp-server/dist/index.js"]
    }
  }
}
```

### Testing with MCP Inspector

```bash
npx @modelcontextprotocol/inspector node dist/index.js
```

## Development

```bash
# Install dependencies
npm install

# Build
npm run build

# Development with auto-reload
npm run dev

# Clean build
npm run clean
```

## Example Tool Usage

### Query Monster Data
```
rogueessence_get_data(dataType="Monster", id="pikachu")
```

### Search Code
```
rogueessence_search_code(pattern="yield return", filePattern="*Scene*.cs")
```

### Get Documentation
```
rogueessence_get_docs(name="architecture")
```

## Distribution

### Option 1: Run directly from GitHub (No publish needed)

Users can run directly from the repo without any publishing:

```json
{
  "mcpServers": {
    "rogueessence": {
      "command": "npx",
      "args": ["github:JohnVonDrashek/RogueEssence/rogueessence-mcp-server"]
    }
  }
}
```

### Option 2: Publish to npm

For a cleaner install experience:

**1. Prepare package.json** - Ensure these fields are set:

```json
{
  "name": "rogueessence-mcp-server",
  "bin": {
    "rogueessence-mcp-server": "dist/index.js"
  },
  "files": ["dist"],
  "scripts": {
    "prepublishOnly": "npm run build"
  }
}
```

**2. Publish:**

```bash
npm login
npm publish
```

**3. Users can then configure with npx:**

```json
{
  "mcpServers": {
    "rogueessence": {
      "command": "npx",
      "args": ["rogueessence-mcp-server"]
    }
  }
}
```

No build step required for end users - npx handles everything.

## License

MIT
