using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider slider;
    public PlayerHealth playerHealth;

    private void Start()
    {
        slider.maxValue = playerHealth.maxHealth;
        slider.value = playerHealth.health;
    }

    private void Update()
    {
        slider.maxValue = playerHealth.maxHealth;
        slider.value = playerHealth.health;
    }
}
