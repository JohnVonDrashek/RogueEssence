# WaypointServer

[![.NET 6.0](https://img.shields.io/badge/.NET-6.0-purple)](https://dotnet.microsoft.com/)
[![LiteNetLib](https://img.shields.io/badge/LiteNetLib-0.9.5.2-blue)](https://github.com/RevenantX/LiteNetLib)

A standalone multiplayer matchmaking server for Rogue Essence. This server facilitates peer-to-peer connections between players by managing connection handshakes, activity matching, and data relay between paired clients.

## Key Files

| File | Description |
|------|-------------|
| `Program.cs` | Entry point that initializes the server and runs the main event polling loop |
| `ConnectionManager.cs` | Core matchmaking logic handling client connections, pairing, and message relay |
| `ClientInfo.cs` | Data structure storing client connection metadata (activity type, destination) |
| `DiagManager.cs` | Singleton for configuration loading, logging, and error tracking |
| `TwoWayDict.cs` | Bidirectional dictionary utility for efficient peer-to-UUID lookups |
| `WaypointServer.csproj` | Project file defining .NET 6.0 target and LiteNetLib dependency |

## Relationships

- **Standalone project**: WaypointServer is an independent executable, not referenced by the main RogueEssence game project
- **Part of solution**: Included in `RogueEssence.sln` for unified build/deployment
- **Log output**: Writes error and info logs to the `Log/` directory (shared with main project)
- **Shared protocol**: Game clients connect using LiteNetLib with a matching connection key

## Usage

### Running the Server

```bash
# Using .NET 6 SDK
dotnet run --project WaypointServer

# macOS with Homebrew .NET 6
/opt/homebrew/opt/dotnet@6/bin/dotnet run --project WaypointServer

# From within WaypointServer directory
dotnet run
```

### Configuration

The server reads from `Config.xml` in its working directory. If the file doesn't exist, defaults are used and a new config file is created.

```xml
<Config>
  <ServerName>Default Server</ServerName>
  <Port>1705</Port>
</Config>
```

| Option | Default | Description |
|--------|---------|-------------|
| `ServerName` | `Default Server` | Display name sent to connecting clients |
| `Port` | `1705` | UDP port the server listens on |

### Network Protocol

The server uses **LiteNetLib** for reliable UDP networking. Key protocol details:

| Message Type | ID | Direction | Purpose |
|--------------|----|-----------|---------|
| `SERVER_INTRO` | 0 | Server -> Client | Sends server name upon connection |
| `CLIENT_INFO` | 1 | Client -> Server | Client sends UUID, activity type, and target partner UUID |
| `SERVER_CONNECTED` | 2 | Server -> Client | Confirms match found, sends partner's data |
| `CLIENT_DATA` | 3 | Client -> Client | Relay of game data between matched peers |

**Connection Flow:**
1. Client connects with connection key
2. Server sends `SERVER_INTRO` with server name
3. Client sends `CLIENT_INFO` with UUID, activity, and target partner UUID
4. If target partner is searching, server pairs them and sends `SERVER_CONNECTED` to both
5. Once paired, all subsequent messages are relayed directly to the partner

**Disconnect Codes:**
- `1` - Partner disconnected
- `2` - UUID already connected elsewhere
- `3` - Activity type mismatch between partners
- `4` - Server shutdown

### Runtime Display

While running, the server displays a status dashboard refreshing every ~1.5 seconds:

```
Name:Default Server
Port:1705
Searching:2/5
Active:4/5
Errors:0
```

- **Searching**: Clients waiting to be matched
- **Active**: Clients currently paired and exchanging data
- **Peers**: Total connected clients
- **Errors**: Cumulative error count since startup

### Publishing

Build standalone executables for distribution:

```bash
# Windows
dotnet publish -r win-x64 -c Release

# macOS
dotnet publish -r osx-x64 -c Release

# Linux
dotnet publish -r linux-x64 -c Release
```

Published binaries are placed in `../publish/{runtime}/WaypointServer/`.

---

![Repobeats analytics](https://repobeats.axiom.co/api/embed/placeholder.svg "Repobeats analytics image")
