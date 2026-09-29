using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform player;
    public float speed = 1.5f;
    public float attackRange = 0.9f;
    public int attackDamage = 1;
    public float attackCooldown = 1f;

    private float nextAttackTime = 0f;

    private void Update()
    {
        if (player == null || !player.gameObject.activeSelf)
        {
            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance > attackRange)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );
        }
        else if (Time.time >= nextAttackTime)
        {
            PlayerHealth playerHealth =
                player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }

            nextAttackTime = Time.time + attackCooldown;
        }
    }
}