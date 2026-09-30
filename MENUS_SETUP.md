# Cendrelith — Main menu, pause and ambient background — v2

## Installation (Unity 6.6)

1. Back up or commit the project. Stop Play Mode.
2. Extract this archive into the project root (the directory containing Assets).
3. Wait for script compilation. Resolve any Console compile errors before continuing.
4. Run **Tools > Dungeon > Set Up Menus**. Save any open scene when Unity asks.
5. The command creates Assets/Scenes/MainMenu.unity if missing, opens it, and puts MainMenu and Dungeon_PrototypeUnityAI first in the shared build scene list. Existing MainMenu scenes are not overwritten.
6. If the active Build Profile overrides the shared scene list, include those two scenes there, with MainMenu first.
7. Press Play. Choose New Game. Inside the dungeon press Esc to pause/resume.

Menus are generated at runtime and therefore appear in Play Mode. Editing those runtime objects does not persist after stopping Play. The controller only installs in MainMenu and Dungeon_PrototypeUnityAI.

## Implemented

- Game title: Cendrelith. Main menu and credits read the shared GameTitle constant.
- Setup applies Cendrelith as Unity Product Name. The displayed title fits on one line.
- Clean main-menu background with live UI buttons.
- Continue disabled; New Game loads the existing dungeon scene.
- Settings: master volume, fullscreen, ambient menu animation toggle. Preferences persist locally.
- Credits is explicitly a temporary placeholder.
- Pause: Resume, Settings, Main Menu and Quit to Desktop.
- Confirmation before leaving the current run; Cancel/Esc returns to the previous menu.
- Escape cannot resume the game-over state.
- Pause restores the prior time scale, audio pause and cursor state.
- Asset-pack input/AI components found in this scene are suspended while paused and restored on resume.
- Main-menu background: slow image drift, procedural drifting embers and a soft fluctuating warm light. People, smoke and flames in the artwork are not individually animated.
- Menu animation can be disabled in Settings. No video or extra package is needed.
- Mouse and EventSystem keyboard navigation; gamepad menu navigation uses the Input System UI module. Escape handling is keyboard-specific.
- Quitting in the Editor stops Play Mode; quitting a standalone build closes the application.

## Art and scope

This is the first functional implementation. Buttons/panels use simple bronze outlines and a built-in readable font, not the final ornate artwork/typeface in the mockups. The generated background has no baked-in labels. UI graphics, character animation, functional item inventory and quests remain separate tasks.

The prior XP/leveling patch is included for dependency completeness. Its files replace the same v1 scripts, not a second copy of the classes. Do not apply the older XP ZIP after this package, as that would revert newer combat pause guards.

No new gameplay save/load implementation is included. Existing asset-pack SaveSystem code is untouched. Continue does not claim compatibility with those saves. Returning to the menu or restarting reloads the scene and resets our new progression.

## Required verification in Unity

1. Compilation completes without errors; run Tools > Dungeon > Validate RPG Progression.
2. Main menu shows one set of buttons and one EventSystem. Continue is disabled. Mouse and keyboard selection work.
3. New Game loads Dungeon_PrototypeUnityAI. Esc opens pause; hero/enemies stop and attacks/interactions do not fire.
4. Resume and Esc restore movement, combat and audio. Repeat several times; no duplicate overlays.
5. Settings and confirmation dialogs keep the game paused. Cancel/Esc returns correctly.
6. Die normally: game-over panel opens; Esc does not resume. Restart resets the run.
7. Return to Main Menu, cancel once then confirm. Start a fresh game. Check default time scale and working input.
8. Adjust volume, fullscreen and menu-animation preference. Reload and check persistence. Fullscreen must also be checked in a standalone build.
9. With animation on, verify subtle drift and embers; off gives a fully static background. Test at 1920x1080 and a narrower window; inspect text and panel fit.
10. Build and test New Game/Main Menu/Quit. Check overridden Build Profile scene lists if a destination is reported unavailable.

## Verification performed here

Eight affected menu/integration C# files passed a tree-sitter C# syntax parse. git diff --check and ZIP integrity passed. This is not compilation or a Unity runtime test: Unity/C# compiler are unavailable here.

Background generated with the built-in image tool using the approved main-menu mockup as reference: remove all text, dividers and button frames; retain composition, dark left-side space, double doors, refugees and ruined settlement.

API references checked:
https://docs.unity.cn/Packages/com.unity.inputsystem@1.13/api/UnityEngine.InputSystem.UI.InputSystemUIInputModule.html
https://github.com/Unity-Technologies/uGUI/blob/main/com.unity.ugui/Runtime/UGUI/UI/Core/Text.cs
