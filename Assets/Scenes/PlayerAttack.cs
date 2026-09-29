using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 1.3f;
    public int damage = 1;
    public float attackCooldown = 0.5f;

    private float nextAttackTime = 0f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void Attack()
    {
        StartCoroutine(AttackFlash());

        Collider2D[] targets = Physics2D.OverlapCircleAll(
            transform.position,
            attackRange
        );

        foreach (Collider2D target in targets)
        {
            EnemyHealth enemy = target.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    private IEnumerator AttackFlash()
    {
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(0.12f);
        spriteRenderer.color = originalColor;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}