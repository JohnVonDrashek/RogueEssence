import { readFile, readdir } from "fs/promises";
import { resolve, basename, extname } from "path";
import { existsSync } from "fs";
import { DOCS_DIR, EXAMPLES_DIR, PROJECT_ROOT, CHARACTER_LIMIT } from "../constants.js";

interface DocFile {
  name: string;
  path: string;
  description?: string;
}

interface DocContent {
  name: string;
  path: string;
  content: string;
  truncated: boolean;
}

export async function listDocs(): Promise<DocFile[]> {
  const docs: DocFile[] = [];

  if (existsSync(DOCS_DIR)) {
    const files = await readdir(DOCS_DIR);
    for (const file of files) {
      if (extname(file) === ".md") {
        const content = await readFile(resolve(DOCS_DIR, file), "utf-8");
        const firstLine = content.split("\n")[0];
        const title = firstLine.replace(/^#+\s*/, "").trim();

        docs.push({
          name: basename(file, ".md"),
          path: `docs/claude/${file}`,
          description: title
        });
      }
    }
  }

  return docs;
}

export async function getDoc(name: string): Promise<DocContent | null> {
  // Try different variations of the name
  const variations = [
    resolve(DOCS_DIR, `${name}.md`),
    resolve(DOCS_DIR, name),
    resolve(PROJECT_ROOT, name)
  ];

  for (const filePath of variations) {
    if (existsSync(filePath)) {
      try {
        let content = await readFile(filePath, "utf-8");
        const truncated = content.length > CHARACTER_LIMIT;

        if (truncated) {
          content = content.slice(0, CHARACTER_LIMIT) + "\n\n... (truncated)";
        }

        return {
          name: basename(filePath, ".md"),
          path: filePath.replace(PROJECT_ROOT + "/", ""),
          content,
          truncated
        };
      } catch {
        continue;
      }
    }
  }

  return null;
}

export async function listExamples(): Promise<DocFile[]> {
  const examples: DocFile[] = [];

  if (existsSync(EXAMPLES_DIR)) {
    const files = await readdir(EXAMPLES_DIR);
    for (const file of files) {
      if (extname(file) === ".cs" || extname(file) === ".lua" || extname(file) === ".md") {
        const content = await readFile(resolve(EXAMPLES_DIR, file), "utf-8");
        // Extract description from first comment or heading
        const lines = content.split("\n");
        let description = "";

        for (const line of lines.slice(0, 10)) {
          if (line.startsWith("///") || line.startsWith("//") || line.startsWith("--") || line.startsWith("#")) {
            description = line.replace(/^[/#-]+\s*/, "").trim();
            if (description && !description.startsWith("<")) break;
          }
        }

        examples.push({
          name: basename(file, extname(file)),
          path: `examples/${file}`,
          description: description || `Example: ${basename(file, extname(file))}`
        });
      }
    }
  }

  return examples;
}

export async function getExample(name: string): Promise<DocContent | null> {
  if (!existsSync(EXAMPLES_DIR)) {
    return null;
  }

  const files = await readdir(EXAMPLES_DIR);

  // Find matching file
  for (const file of files) {
    const baseName = basename(file, extname(file));
    if (baseName === name || file === name) {
      const filePath = resolve(EXAMPLES_DIR, file);
      try {
        let content = await readFile(filePath, "utf-8");
        const truncated = content.length > CHARACTER_LIMIT;

        if (truncated) {
          content = content.slice(0, CHARACTER_LIMIT) + "\n\n// ... (truncated)";
        }

        return {
          name: baseName,
          path: `examples/${file}`,
          content,
          truncated
        };
      } catch {
        continue;
      }
    }
  }

  return null;
}

export async function getArchitectureOverview(): Promise<string> {
  const claudeMd = resolve(PROJECT_ROOT, "CLAUDE.md");
  const architectureMd = resolve(DOCS_DIR, "architecture.md");

  let overview = "# RogueEssence Architecture Overview\n\n";

  // Add CLAUDE.md summary
  if (existsSync(claudeMd)) {
    const content = await readFile(claudeMd, "utf-8");
    overview += "## Quick Reference (from CLAUDE.md)\n\n";
    overview += content.slice(0, 5000);
    if (content.length > 5000) overview += "\n\n... (see full CLAUDE.md for more)";
  }

  // Add architecture.md
  if (existsSync(architectureMd)) {
    const content = await readFile(architectureMd, "utf-8");
    overview += "\n\n## Detailed Architecture\n\n";
    overview += content.slice(0, CHARACTER_LIMIT - overview.length);
    if (overview.length >= CHARACTER_LIMIT) {
      overview += "\n\n... (truncated - use rogueessence_get_docs for full content)";
    }
  }

  return overview;
}
