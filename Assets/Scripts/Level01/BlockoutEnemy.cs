using UnityEngine;

// Encounter proxy for layout testing. Constrained to its authored room.
[RequireComponent(typeof(Rigidbody2D), typeof(EnemyHealth))]
public sealed class BlockoutEnemy : MonoBehaviour
{
    public PlayerHealth player;
    public Rect homeRoom;
    public float detectionRange = 6f;
    public float speed = 1.7f;
    public float attackInterval = 1.4f;
    private Rigidbody2D body;
    private float nextAttack;
    private bool alerted;

    private void Awake() => body = GetComponent<Rigidbody2D>();

    private void FixedUpdate()
    {
        if (Time.timeScale <= 0 || player == null || player.health <= 0 || !player.isActiveAndEnabled) return;
        Vector2 target = player.transform.position;
        if (!homeRoom.Contains(target)) { alerted = false; return; }
        float distance = Vector2.Distance(body.position, target);
        alerted |= distance <= detectionRange;
        if (!alerted) return;
        if (distance > 1.05f)
        {
            Vector2 next = Vector2.MoveTowards(body.position, target, speed * Time.fixedDeltaTime);
            next.x = Mathf.Clamp(next.x, homeRoom.xMin + 0.6f, homeRoom.xMax - 0.6f);
            next.y = Mathf.Clamp(next.y, homeRoom.yMin + 0.6f, homeRoom.yMax - 0.6f);
            body.MovePosition(next);
        }
        else if (Time.time >= nextAttack)
        {
            player.TakeDamage(1);
            nextAttack = Time.time + attackInterval;
        }
    }
}
