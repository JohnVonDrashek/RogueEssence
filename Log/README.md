# Log

Runtime log output directory for Rogue Essence and WaypointServer. Log files are automatically created here during execution to capture errors, diagnostics, and debug information.

## Log Format

Log files are named by date using the format `YYYY-MM-DD.txt`. Each entry includes:

```
[YYYY/MM/DD HH:mm:ss.fff] Message or error description
Exception Depth: 0
Full exception stack trace...
```

## Key Details

- **Auto-created**: This directory is created automatically by `DiagManager` if it doesn't exist
- **Daily rotation**: A new log file is created for each day
- **Append mode**: Multiple errors on the same day are appended to the same file
- **Timestamped entries**: All log entries include millisecond-precision timestamps

## Relationships

- **WaypointServer**: The matchmaking server writes connection errors and diagnostics here via `DiagManager.LogError()` and `DiagManager.LogInfo()`
- **RogueEssence**: The main game also uses a similar logging pattern to this directory
- **Debugging**: Check these logs when troubleshooting multiplayer connection issues or game crashes

## Usage

To monitor logs in real-time while running WaypointServer:

```bash
# Follow today's log file
tail -f Log/$(date +%Y-%m-%d).txt

# View recent errors
cat Log/$(date +%Y-%m-%d).txt
```

This directory is typically excluded from version control (empty in the repository) since log content is machine-specific and runtime-generated.

---

![Repobeats analytics](https://repobeats.axiom.co/api/embed/placeholder.svg "Repobeats analytics image")
