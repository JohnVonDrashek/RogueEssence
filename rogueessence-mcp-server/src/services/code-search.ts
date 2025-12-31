import { readFile, readdir, stat } from "fs/promises";
import { resolve, relative, extname, basename } from "path";
import { existsSync } from "fs";
import { glob } from "glob";
import { SOURCE_DIRS, PROJECT_ROOT, CHARACTER_LIMIT } from "../constants.js";

interface SearchResult {
  file: string;
  line: number;
  content: string;
  context?: string[];
}

interface FileInfo {
  path: string;
  relativePath: string;
  size: number;
  extension: string;
}

interface ClassInfo {
  name: string;
  file: string;
  line: number;
  namespace?: string;
  baseClass?: string;
  interfaces?: string[];
  summary?: string;
  content: string;
}

export async function searchCode(
  pattern: string,
  options: {
    filePattern?: string;
    caseSensitive?: boolean;
    limit?: number;
    contextLines?: number;
  } = {}
): Promise<SearchResult[]> {
  const { filePattern = "*.cs", caseSensitive = false, limit = 50, contextLines = 2 } = options;
  const results: SearchResult[] = [];
  const regex = new RegExp(pattern, caseSensitive ? "g" : "gi");

  for (const sourceDir of SOURCE_DIRS) {
    if (!existsSync(sourceDir)) continue;

    const files = await glob(`**/${filePattern}`, {
      cwd: sourceDir,
      nodir: true,
      ignore: ["**/bin/**", "**/obj/**", "**/node_modules/**"]
    });

    for (const file of files) {
      if (results.length >= limit) break;

      const fullPath = resolve(sourceDir, file);
      try {
        const content = await readFile(fullPath, "utf-8");
        const lines = content.split("\n");

        for (let i = 0; i < lines.length; i++) {
          if (results.length >= limit) break;

          if (regex.test(lines[i])) {
            const context: string[] = [];
            for (let j = Math.max(0, i - contextLines); j <= Math.min(lines.length - 1, i + contextLines); j++) {
              context.push(`${j + 1}: ${lines[j]}`);
            }

            results.push({
              file: relative(PROJECT_ROOT, fullPath),
              line: i + 1,
              content: lines[i].trim(),
              context
            });
          }
        }
      } catch {
        // Skip unreadable files
      }
    }
  }

  return results;
}

export async function getClassDefinition(className: string): Promise<ClassInfo | null> {
  // Search for class definition
  const classPattern = `class\\s+${className}\\b`;
  const regex = new RegExp(classPattern);

  for (const sourceDir of SOURCE_DIRS) {
    if (!existsSync(sourceDir)) continue;

    const files = await glob("**/*.cs", {
      cwd: sourceDir,
      nodir: true,
      ignore: ["**/bin/**", "**/obj/**"]
    });

    for (const file of files) {
      const fullPath = resolve(sourceDir, file);
      try {
        const content = await readFile(fullPath, "utf-8");
        const lines = content.split("\n");

        for (let i = 0; i < lines.length; i++) {
          if (regex.test(lines[i])) {
            // Extract class info
            const classLine = lines[i];

            // Find namespace
            let namespace: string | undefined;
            for (let j = i - 1; j >= 0; j--) {
              const nsMatch = lines[j].match(/namespace\s+([\w.]+)/);
              if (nsMatch) {
                namespace = nsMatch[1];
                break;
              }
            }

            // Parse class declaration
            const declMatch = classLine.match(/class\s+(\w+)(?:\s*:\s*(.+))?/);
            let baseClass: string | undefined;
            let interfaces: string[] | undefined;

            if (declMatch && declMatch[2]) {
              const inheritance = declMatch[2].split(",").map(s => s.trim());
              // First non-interface is usually base class
              for (const item of inheritance) {
                if (!item.startsWith("I") || item.length < 2 || item[1] !== item[1].toUpperCase()) {
                  baseClass = item;
                } else {
                  interfaces = interfaces || [];
                  interfaces.push(item);
                }
              }
            }

            // Extract XML summary if present
            let summary: string | undefined;
            if (i > 0 && lines[i - 1].includes("</summary>")) {
              const summaryLines: string[] = [];
              for (let j = i - 1; j >= 0; j--) {
                summaryLines.unshift(lines[j]);
                if (lines[j].includes("<summary>")) break;
              }
              summary = summaryLines
                .join("\n")
                .replace(/<\/?summary>/g, "")
                .replace(/\/\/\//g, "")
                .trim();
            }

            // Extract class body (up to CHARACTER_LIMIT)
            let braceCount = 0;
            let startFound = false;
            const classContent: string[] = [];

            for (let j = i; j < lines.length && classContent.join("\n").length < CHARACTER_LIMIT; j++) {
              classContent.push(lines[j]);
              for (const char of lines[j]) {
                if (char === "{") {
                  braceCount++;
                  startFound = true;
                } else if (char === "}") {
                  braceCount--;
                  if (startFound && braceCount === 0) {
                    return {
                      name: className,
                      file: relative(PROJECT_ROOT, fullPath),
                      line: i + 1,
                      namespace,
                      baseClass,
                      interfaces,
                      summary,
                      content: classContent.join("\n")
                    };
                  }
                }
              }
            }

            // Return partial if we hit the limit
            return {
              name: className,
              file: relative(PROJECT_ROOT, fullPath),
              line: i + 1,
              namespace,
              baseClass,
              interfaces,
              summary,
              content: classContent.join("\n") + "\n// ... truncated"
            };
          }
        }
      } catch {
        // Skip unreadable files
      }
    }
  }

  return null;
}

export async function findFiles(
  pattern: string,
  options: { limit?: number } = {}
): Promise<FileInfo[]> {
  const { limit = 50 } = options;
  const results: FileInfo[] = [];

  for (const sourceDir of SOURCE_DIRS) {
    if (!existsSync(sourceDir)) continue;

    const files = await glob(`**/${pattern}`, {
      cwd: sourceDir,
      nodir: true,
      ignore: ["**/bin/**", "**/obj/**", "**/node_modules/**"]
    });

    for (const file of files) {
      if (results.length >= limit) break;

      const fullPath = resolve(sourceDir, file);
      try {
        const stats = await stat(fullPath);
        results.push({
          path: fullPath,
          relativePath: relative(PROJECT_ROOT, fullPath),
          size: stats.size,
          extension: extname(file)
        });
      } catch {
        // Skip inaccessible files
      }
    }
  }

  return results;
}

export async function getFileContent(filePath: string): Promise<string | null> {
  // Handle both absolute and relative paths
  const fullPath = filePath.startsWith("/") ? filePath : resolve(PROJECT_ROOT, filePath);

  if (!existsSync(fullPath)) {
    return null;
  }

  try {
    const content = await readFile(fullPath, "utf-8");
    if (content.length > CHARACTER_LIMIT) {
      return content.slice(0, CHARACTER_LIMIT) + "\n// ... truncated (file too large)";
    }
    return content;
  } catch (error) {
    throw new Error(`Failed to read file: ${error instanceof Error ? error.message : String(error)}`);
  }
}
