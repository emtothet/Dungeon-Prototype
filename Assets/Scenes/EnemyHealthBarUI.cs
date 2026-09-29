using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour
{
    public Slider slider;
    public EnemyHealth enemyHealth;

    private void Start()
    {
        slider.maxValue = enemyHealth.maxHealth;
        slider.value = enemyHealth.health;
    }

    private void Update()
    {
        if (enemyHealth != null)
        {
            slider.value = enemyHealth.health;
        }
    }
}