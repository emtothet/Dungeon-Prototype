# Cendrelith RPG MVP — XP and levels (first slice)

## Delivered

- PlayerHealth adds PlayerProgression automatically at runtime. Existing scenes need no YAML edits.
- PlayerAttack credits its kills to that progression component, handles colliders on children and damages each enemy once per swing.
- EnemyHealth grants XP only once, to the credited attacker. The original one-argument TakeDamage API remains valid but grants no XP.
- Default reward: 10 XP per enemy. Thresholds: 30, 60, 90, 120 XP (300 total to level 5).
- Per level: +2 maximum/current HP, +1 damage. Base Inspector damage is unchanged; the attack adds the level bonus.
- Carry-over XP and multiple levels per award are supported; level cap is 5.
- Temporary level/XP text at bottom-left, enabled by showPrototypeHUD. This is a prototype IMGUI overlay, not final UI.
- Health slider maximum follows level-up changes; the existing health globe already reads maximum HP dynamically.
- Existing scenes, art, enemy stats and loot prefabs are not replaced.

## Not included

No armor, abilities, item inventory, equipment, save/load or persistence across scene loads yet. Stopping Play, reloading a scene or Restart resets progression. Restart uses the existing scene reload behavior. This is not a complete RPG/save system.

## Verification in Unity 6.6

1. Import the changed scripts and new .meta files. Wait for compilation; Console must have no compile errors.
2. Run Tools > Dungeon > Validate RPG Progression. Expect the success message for nine pure rule checks.
3. Open Dungeon_PrototypeUnityAI and Play. Expect Level 1, XP 0/30 at bottom-left. No manual component assignment is required when the hero has PlayerHealth and PlayerAttack on the same object.
4. Kill three enemies (10 XP each). Expect level 2, XP 0/60, maximum HP +2 and actual attack damage +1. Damage in PlayerAttack's Inspector stays the BASE damage.
5. Take damage before leveling: only the additional 2 HP should be healed, not the whole health bar. Verify slider and globe.
6. Give a test enemy 95 Experience Reward before Play. From a fresh level-1 session, one kill should reach level 3 with 5/90 XP, +4 HP capacity and +2 damage.
7. Give another test enemy 300 XP. From a fresh run it should reach level 5 with MAX LEVEL, +8 HP capacity and +4 damage. Further kills do not increase level.
8. Test an enemy with multiple child colliders: one swing deals damage once; its death spawns loot once and grants XP once.
9. Verify environmental/uncredited damage grants no XP, death still opens game-over, and Restart resets XP and stats to scene values.
10. Verify attack input does nothing while paused. If the temporary HUD overlaps other UI, disable Show Prototype HUD and replace it with the final Canvas UI later.

## Limits of verification here

Unity and a C# compiler are not installed in the authoring environment. The Unity menu checks and scene checks have been provided but not executed here. Runtime validation must be completed in the Editor before merging/releasing.

## Files

New: ProgressionState.cs, PlayerProgression.cs, Editor/RpgProgressionChecks.cs, their metadata.
Changed: PlayerHealth.cs, PlayerAttack.cs, EnemyHealth.cs, HealthBarUI.cs.
No generated art is required for this patch.
