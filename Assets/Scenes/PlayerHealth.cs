using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int health = 5;

    public void TakeDamage(int damage)
    {
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