using UnityEngine;

[DisallowMultipleComponent]
public class PlayerProgression : MonoBehaviour
{
    [Min(0)] public int healthPerLevel = 2;
    [Min(0)] public int damagePerLevel = 1;
    public bool showPrototypeHUD = true;

    public ProgressionState State { get; } = new ProgressionState();
    public int DamageBonus => (State.Level - 1) * Mathf.Max(0, damagePerLevel);
    private PlayerHealth playerHealth;
    private int baseMaxHealth;
    private float levelMessageUntil;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null) baseMaxHealth = playerHealth.maxHealth;
    }

    public void AddExperience(int amount)
    {
        if (playerHealth == null || playerHealth.health <= 0 || !isActiveAndEnabled) return;
        int levelsGained = State.AddExperience(amount);
        if (levelsGained > 0)
        {
            int newMax = baseMaxHealth + (State.Level - 1) * Mathf.Max(0, healthPerLevel);
            // Add only the gained capacity, not a full heal.
            playerHealth.health = Mathf.Clamp(playerHealth.health + newMax - playerHealth.maxHealth, 0, newMax);
            playerHealth.maxHealth = newMax;
            levelMessageUntil = Time.unscaledTime + 3f;
            Debug.Log($"Level up! Level {State.Level}, max HP {newMax}, bonus damage {DamageBonus}");
        }
    }

    private void OnGUI()
    {
        if (!showPrototypeHUD) return;
        string xp = State.Level == ProgressionState.LevelCap
            ? "MAX LEVEL" : $"XP {State.Experience}/{State.RequiredExperience}";
        // Temporary HUD, independent of the existing Canvas and Input System.
        GUI.Box(new Rect(16, Screen.height - 100, 250, 72),
            $"Level {State.Level}   |   {xp}\nBonus damage: +{DamageBonus}" +
            (Time.unscaledTime < levelMessageUntil ? "\nLEVEL UP!" : ""));
    }
}
