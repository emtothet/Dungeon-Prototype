using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    public TMP_Text goldText;
    public PlayerInventory inventory;

    private void Update()
    {
        goldText.text = "Gold: " + inventory.gold;
    }
}