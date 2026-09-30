using System.Collections;
using System.Collections.Generic;
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
    private PlayerProgression progression;

    private void Start()
    {
        progression = GetComponent<PlayerProgression>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (Time.timeScale > 0f && Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void Attack()
    {
        if (spriteRenderer != null) StartCoroutine(AttackFlash());
        var hitEnemies = new HashSet<EnemyHealth>();
        // Snapshot damage so leveling mid-swing doesn't affect later targets.
        int hitDamage = damage + (progression != null ? progression.DamageBonus : 0);

        Collider2D[] targets = Physics2D.OverlapCircleAll(
            transform.position,
            attackRange
        );

        foreach (Collider2D target in targets)
        {
            EnemyHealth enemy = target.GetComponentInParent<EnemyHealth>();

            if (enemy != null && hitEnemies.Add(enemy))
            {
                enemy.TakeDamage(hitDamage, progression);
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
