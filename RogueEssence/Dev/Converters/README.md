# Converters

Data format upgrade converters for migrating save files and game data between versions. Ensures backward compatibility when data structures change.

## Key Files

| File | Description |
|------|-------------|
| `UpgradeConverters.cs` | Collection of upgrade converters for all version migrations |

## Relationships

- Used by **DataManager** when loading older save files
- Applied during **Serializer** deserialization
- Enables seamless updates without losing player progress

## Usage

```csharp
// Converters are automatically applied during data loading
// Register a converter for a specific version upgrade
Serializer.RegisterConverter(new MyUpgradeConverter());
```
