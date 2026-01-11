# Contributing to RogueEssence

First off, **thank you** for considering contributing! I truly believe in open source and the power of community collaboration. Unlike many repositories, I actively welcome contributions of all kinds - from bug fixes to new features.

## My Promise to Contributors

- **I will respond to every PR and issue** - I guarantee feedback on all contributions
- **Bug fixes are obvious accepts** - If it fixes a bug, it's getting merged
- **New features are welcome** - I'm genuinely open to new ideas and enhancements
- **Direct line of communication** - If I'm not responding to a PR or issue, email me directly at johnvondrashek@gmail.com

## Getting Started

### Prerequisites

```bash
# Initialize submodules (FNA, NLua, RogueElements)
git submodule update --init --recursive

# Install .NET 6 runtime (required for WaypointServer)
brew install dotnet@6
```

### Building

```bash
# Build the entire solution
dotnet build RogueEssence.sln
```

### Testing Changes

RogueEssence is an engine library. To test your changes with the full game + editor, use [PMDODump](https://github.com/audinowho/PMDODump):

```bash
git clone --recursive https://github.com/audinowho/PMDODump ~/code/PMDODump
cd ~/code/PMDODump
dotnet run --project PMDC/PMDC/PMDC.csproj
```

## Project Structure

| Project | Purpose |
|---------|---------|
| `RogueEssence/` | Core game engine library |
| `RogueEssence.Editor.Avalonia/` | Visual map/data editor (Avalonia UI) |
| `WaypointServer/` | Multiplayer matchmaking server |

For detailed architecture information, see the [CLAUDE.md](CLAUDE.md) file.

## Areas Where Help is Appreciated

- **Bug fixes** - Always welcome
- **Documentation** - Improving guides and API docs
- **New dungeon generation algorithms** - Extend the LevelGen system
- **Editor improvements** - Enhance the Avalonia editor experience
- **Lua scripting examples** - Help others learn the scripting API
- **Localization** - Translate content to other languages
- **Performance optimizations** - Make dungeons run smoother

## Submitting Changes

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Ensure the build passes (`dotnet build RogueEssence.sln`)
5. Commit your changes with a clear message
6. Push to your fork
7. Open a Pull Request

## Code Style

- **PascalCase** for classes, methods, properties
- **camelCase** for local variables, parameters
- **`I*`** prefix for interfaces
- **`*Data`** suffix for data classes
- **`*Scene`** suffix for scene classes
- **`*Manager`** suffix for singleton managers

## Code of Conduct

This project follows the [Rule of St. Benedict](CODE_OF_CONDUCT.md) as its code of conduct.

## Questions?

- Open an issue
- Email: johnvondrashek@gmail.com

## License

By contributing, you agree that your contributions will be licensed under the [MIT License](LICENSE).
