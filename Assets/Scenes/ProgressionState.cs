using System;

// Pure progression rules: no Unity objects or scene dependencies.
[Serializable]
public sealed class ProgressionState
{
    public const int LevelCap = 5;
    public int Level { get; private set; } = 1;
    public int Experience { get; private set; }
    public int RequiredExperience => Level >= LevelCap ? 0 : Level * 30;

    public int AddExperience(int amount)
    {
        if (amount <= 0 || Level >= LevelCap) return 0;
        long remaining = (long)Experience + amount;
        int previousLevel = Level;
        while (Level < LevelCap && remaining >= RequiredExperience)
        {
            remaining -= RequiredExperience;
            Level++;
        }
        Experience = Level == LevelCap ? 0 : (int)remaining;
        return Level - previousLevel;
    }
}
