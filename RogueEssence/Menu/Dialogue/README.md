# Dialogue

Dialogue boxes and text display systems for story and NPC conversations. Handles text animation, portraits, and player input during dialogue.

## Key Files

| File | Description |
|------|-------------|
| `DialogueBox.cs` | Main dialogue box with text scrolling and input handling |
| `TitleDialog.cs` | Large title screen style dialogue |
| `QuestionDialog.cs` | Dialogue with yes/no question choices |
| `TimedDialog.cs` | Auto-advancing timed dialogue |
| `ClickedDialog.cs` | Dialogue that waits for click |
| `TextPopUp.cs` | Small popup text notification |
| `InfoMenu.cs` | Information display panel |
| `SpeakerPortrait.cs` | Character portrait display next to dialogue |

## Relationships

- Used by **MenuManager** for dialogue display
- **Lua/** triggers dialogue through ScriptUI
- Renders using **Content/** fonts and UI

## Usage

```csharp
// Show dialogue with portrait
DialogueBox box = new DialogueBox("Hello adventurer!", speakerName, portrait);
yield return MenuManager.Instance.ProcessMenuCoroutine(box);

// Show question
QuestionDialog question = new QuestionDialog("Continue?", true, yesAction, noAction);
```
