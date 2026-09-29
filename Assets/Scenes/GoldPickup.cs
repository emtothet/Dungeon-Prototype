using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    public int value = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInventory inventory =
            other.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.AddGold(value);
            Destroy(gameObject);
        }
    }
}