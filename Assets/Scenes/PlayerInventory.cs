using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int gold = 0;

    public void AddGold(int amount)
    {
        gold += amount;

        Debug.Log("Gold collected: " + amount +
                  ". Total gold: " + gold);
    }
}