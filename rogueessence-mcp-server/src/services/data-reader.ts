import { readFile, readdir } from "fs/promises";
import { resolve, basename, extname } from "path";
import { existsSync } from "fs";
import { DATA_DIR, DataType, DATA_TYPES } from "../constants.js";

interface GameData {
  id: string;
  name?: string;
  [key: string]: unknown;
}

interface ListResult<T> {
  items: T[];
  total: number;
  hasMore: boolean;
  nextOffset?: number;
}

function getDataPath(dataType: DataType): string {
  return resolve(DATA_DIR, dataType);
}

export async function getDataById(dataType: DataType, id: string): Promise<GameData | null> {
  const dataPath = getDataPath(dataType);
  const filePath = resolve(dataPath, `${id}.json`);

  if (!existsSync(filePath)) {
    return null;
  }

  try {
    const content = await readFile(filePath, "utf-8");
    const data = JSON.parse(content) as GameData;
    data.id = id;
    return data;
  } catch (error) {
    throw new Error(`Failed to read ${dataType} data for ID '${id}': ${error instanceof Error ? error.message : String(error)}`);
  }
}

export async function listData(
  dataType: DataType,
  options: {
    limit?: number;
    offset?: number;
    search?: string;
  } = {}
): Promise<ListResult<GameData>> {
  const { limit = 20, offset = 0, search } = options;
  const dataPath = getDataPath(dataType);

  if (!existsSync(dataPath)) {
    return { items: [], total: 0, hasMore: false };
  }

  try {
    const files = await readdir(dataPath);
    const jsonFiles = files.filter(f => extname(f) === ".json");

    // Load all items (for filtering/counting)
    const allItems: GameData[] = [];
    for (const file of jsonFiles) {
      try {
        const content = await readFile(resolve(dataPath, file), "utf-8");
        const data = JSON.parse(content) as GameData;
        data.id = basename(file, ".json");
        allItems.push(data);
      } catch {
        // Skip malformed files
      }
    }

    // Apply search filter if provided
    let filteredItems = allItems;
    if (search) {
      const searchLower = search.toLowerCase();
      filteredItems = allItems.filter(item => {
        const name = String(item.name || item.id || "").toLowerCase();
        const id = String(item.id || "").toLowerCase();
        return name.includes(searchLower) || id.includes(searchLower);
      });
    }

    // Apply pagination
    const total = filteredItems.length;
    const paginatedItems = filteredItems.slice(offset, offset + limit);
    const hasMore = offset + paginatedItems.length < total;

    return {
      items: paginatedItems,
      total,
      hasMore,
      ...(hasMore ? { nextOffset: offset + paginatedItems.length } : {})
    };
  } catch (error) {
    throw new Error(`Failed to list ${dataType} data: ${error instanceof Error ? error.message : String(error)}`);
  }
}

export function isValidDataType(type: string): type is DataType {
  return DATA_TYPES.includes(type as DataType);
}

export async function getDataTypeStats(): Promise<Record<DataType, number>> {
  const stats: Partial<Record<DataType, number>> = {};

  for (const dataType of DATA_TYPES) {
    const dataPath = getDataPath(dataType);
    if (existsSync(dataPath)) {
      try {
        const files = await readdir(dataPath);
        stats[dataType] = files.filter(f => extname(f) === ".json").length;
      } catch {
        stats[dataType] = 0;
      }
    } else {
      stats[dataType] = 0;
    }
  }

  return stats as Record<DataType, number>;
}
