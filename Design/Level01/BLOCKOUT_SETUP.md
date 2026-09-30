# Cendrelith — Level 01 blockout v1

## Open the test

1. Back up the project and stop Play Mode.
2. Extract cendrelith_level01_blockout_v1.zip into the project root. It includes current menus and XP dependencies; do not apply older archives afterward.
3. Wait for compilation. Run **Tools > Cendrelith > Create Level 01 Blockout**.
4. The command builds and saves **Assets/Scenes/Cendrelith_Level01_Blockout.unity** and opens it. Press Play.
5. If that scene already exists, the command opens it without overwriting edits. To regenerate, first rename/move your previous test scene.

This does not change the refuge scene or the New Game destination. Launch the blockout directly from its scene. The shared build scene list gains this scene so Restart can reload it. If a Build Profile overrides the shared scene list, include the blockout there too.

## Controls

- WASD/arrows: movement; Space: existing area melee attack.
- E: caregiver, chest, remedy/note and return door interaction.
- M: whole-level overview / follow camera.
- Esc: existing pause menu.
- Death: click Restart blockout.

## Test circuit

Start beside the yellow caregiver in the refuge. Accept the quest with E. Travel north through the double-door placeholder to vestibule, then gallery. Three skeleton proxies are spaced through the gallery. East is an optional guarded store with an ochre chest. Main route continues north to the infirmary (purple necromancer proxy plus two skeletons), then east into the service corridor, south and west into observation. Collect the green remedy/note. Unbolt the west return door from the observation side; follow west then south to the refuge. Give the remedy to the caregiver with E.

The return door can be opened from inside even before collecting the remedy. It cannot be opened from the refuge side. The chest requires its guard defeated. Remedy pickup requires quest acceptance and infirmary enemies defeated.

## What this tests

- Room footprints, door clearance, corridor connectivity, camera traversal and optional branch.
- Existing player health/attack and XP on enemy death.
- 6 skeletons at 10 XP plus the necromancer proxy at 30 XP: 90 XP if all are defeated.
- Refuge return and minimal quest completion.

## Deliberate placeholders

- Top-down colored shapes, not final isometric artwork. M shows the layout; room names mark the geometry.
- Player starts with 10 HP in this scene for testing. Other scenes retain their authored health.
- No enemy health bars or hotbar.
- Enemy proxies pursue only within their own room; this is not general navigation/pathfinding AI.
- Necromancer is a slower, tougher melee proxy (7 HP), not the final ranged/summoning enemy.
- Chest directly adds +1 base damage once; no item/inventory interface is implemented here.
- Quest dialogue and note are short debug messages, no full dialogue system.
- No persistence: Restart resets enemies, quest, gate, chest and XP.
- Duration target of 10–15 minutes is unvalidated; actual movement/combat timing must drive revisions.

## Validation performed

Four new/modified C# files passed a syntax parser. Grid connectivity checked: observation accessible by the main route with return door shut; removing the optional store does not break progression; removing the infirmary does block the main route; opening the return gate reduces grid distance between refuge and observation from 110 to 38 cells. This is geometric grid validation, not collider/playtesting.

Unity and a C# compiler are unavailable in the authoring environment. Compilation, URP rendering, physics, UI/input and gameplay must be verified in Unity.

## Acceptance in Editor

- Compile with no errors; generated scene is visible in Scene view before Play.
- Walk along walls and through all doorways; no escaping the floor footprint.
- Try opening return door from refuge side: remains blocked.
- Complete main route without store weapon; record deaths/time, then test with upgrade.
- Pause next to an enemy: HP and positions remain stable. Resume restores input.
- Each enemy awards XP once. Chest adds damage once; remedy and quest complete once.
- Open shortcut, return, then restart: confirm door and quest reset.
- Check M overview at 16:9 and narrow windows, and confirm gameplay traverses multiple camera views.
