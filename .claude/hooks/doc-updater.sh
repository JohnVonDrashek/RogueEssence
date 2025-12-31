#!/bin/bash
set -euo pipefail

# Documentation Updater Hook
# Runs after Claude finishes, spawns a subagent to update docs if code changed

# Read transcript from stdin
transcript=$(cat)

# Extract files modified by Write/Edit tools using jq
modified_files=$(echo "$transcript" | jq -r '
  .messages[]?
  | select(.type == "tool_use")
  | select(.name == "Write" or .name == "Edit")
  | .input.file_path
' 2>/dev/null | sort -u)

# Exit if no files modified
[ -z "$modified_files" ] && exit 0

# Filter to only code files (skip docs, configs, etc.)
code_files=$(echo "$modified_files" | grep -E '\.(cs|lua|json|xml|csproj)$' | grep -v 'README\|CLAUDE' || true)

# Exit if only non-code files changed
[ -z "$code_files" ] && exit 0

# Build file list for the subagent prompt
file_list=$(echo "$code_files" | tr '\n' ', ' | sed 's/,$//')

# Spawn subagent in background
nohup claude --print -p "You are a documentation maintenance agent.

The following files were just modified: $file_list

Review these changes and update any relevant documentation:
- CLAUDE.md if architecture, patterns, key entry points, or conventions changed
- README.md files in affected directories if APIs, usage, or component purposes changed

Guidelines:
- Only update docs if genuinely needed (not every code change needs doc updates)
- Make minimal, precise edits
- Match the existing documentation style
- Do not commit changes - leave them for human review
- Do not create new documentation files

Start by reading the modified files to understand what changed, then check relevant docs." > /tmp/doc-updater.log 2>&1 &

exit 0
