using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int health;
    public GameObject lootPrefab;

    private void Awake()
    {
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Max(health, 0);

        Debug.Log(gameObject.name + " health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
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