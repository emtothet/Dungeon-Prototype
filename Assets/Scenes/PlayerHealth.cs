using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int health = 5;

    private void Awake()
    {
        // Existing scenes work without manually wiring another component.
        if (GetComponent<PlayerProgression>() == null)
            gameObject.AddComponent<PlayerProgression>();
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || health <= 0) return;
        health -= damage;
        health = Mathf.Max(health, 0);

        Debug.Log("Player health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");

        GameManager gameManager =
            FindFirstObjectByType<GameManager>();

        if (gameManager != null)
        {
            gameManager.PlayerDied();
        }
        else
        {
            Debug.LogError("GameManager was not found!");
        }

        gameObject.SetActive(false);
    }
}
