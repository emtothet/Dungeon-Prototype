using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int health;
    public GameObject lootPrefab;
    [Min(0)] public int experienceReward = 10;
    private bool isDead;

    private void Awake()
    {
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, null);
    }

    public void TakeDamage(int damage, PlayerProgression source)
    {
        if (isDead || damage <= 0) return;
        health -= damage;
        health = Mathf.Max(health, 0);

        Debug.Log(gameObject.name + " health: " + health);

        if (health <= 0)
        {
            Die(source);
        }
    }

    private void Die(PlayerProgression source)
    {
        isDead = true;
        // Only the credited attacker receives XP; environmental kills give none.
        if (source != null) source.AddExperience(experienceReward);
        if (lootPrefab != null)
        {
            Instantiate(
                lootPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }
}
