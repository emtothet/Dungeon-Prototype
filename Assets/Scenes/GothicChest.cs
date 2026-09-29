using System.Collections;
using UnityEngine;

public class GothicChest : MonoBehaviour
{
    [Header("Loot Settings")]
    public GameObject goldPrefab;
    public int minGoldDrops = 3;
    public int maxGoldDrops = 6;
    public float dropSpreadRadius = 1.0f;

    [Header("Visuals")]
    public Sprite openedSprite;
    public Color openTint = new Color(0.7f, 0.7f, 0.7f, 1f);

    private bool isOpened = false;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void OpenChest()
    {
        if (isOpened) return;
        isOpened = true;

        if (openedSprite != null && sr != null)
        {
            sr.sprite = openedSprite;
        }
        else if (sr != null)
        {
            sr.color = openTint;
        }

        SpawnLoot();
    }

    private void SpawnLoot()
    {
        if (goldPrefab == null) return;

        int count = Random.Range(minGoldDrops, maxGoldDrops + 1);
        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Random.insideUnitCircle * dropSpreadRadius;
            // Slightly bias downward for isometric floor placement
            offset.y *= 0.6f;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y - 0.2f, 0f);

            Instantiate(goldPrefab, spawnPos, Quaternion.identity);
        }

        Debug.Log("<color=gold>[Diablo Loot]</color> The ancient gothic chest bursts open with " + count + " gleaming coins!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
        {
            OpenChest();
        }
    }
}
