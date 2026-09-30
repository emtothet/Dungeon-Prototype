using System;
using UnityEditor;
using UnityEngine;

public static class RpgProgressionChecks
{
    [MenuItem("Tools/Dungeon/Validate RPG Progression")]
    public static void Run()
    {
        var state = new ProgressionState();
        Check(state.Level == 1 && state.Experience == 0 && state.RequiredExperience == 30, "initial state");
        Check(state.AddExperience(-10) == 0 && state.Experience == 0, "negative XP ignored");
        Check(state.AddExperience(0) == 0, "zero XP ignored");
        Check(state.AddExperience(29) == 0 && state.Experience == 29, "below threshold");
        Check(state.AddExperience(1) == 1 && state.Level == 2 && state.Experience == 0, "exact threshold");
        Check(state.AddExperience(65) == 1 && state.Level == 3 && state.Experience == 5, "carry-over XP");
        var bulk = new ProgressionState();
        Check(bulk.AddExperience(95) == 2 && bulk.Level == 3 && bulk.Experience == 5, "multiple levels");
        Check(bulk.AddExperience(int.MaxValue) == 2 && bulk.Level == 5 && bulk.Experience == 0, "large award and cap");
        Check(bulk.AddExperience(10) == 0 && bulk.RequiredExperience == 0, "XP at cap");
        Debug.Log("RPG progression: all 9 rule checks passed. Run the scene checks in RPG_MVP_TESTING.md next.");
    }

    private static void Check(bool condition, string label)
    {
        if (!condition) throw new InvalidOperationException("RPG check failed: " + label);
    }
}
