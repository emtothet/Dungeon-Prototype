using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiabloHealthGlobeUI : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public Image fillImage;
    public TextMeshProUGUI healthText;

    [Header("Animation Settings")]
    public bool pulseOnLowHealth = true;
    public float lowHealthThreshold = 0.35f;

    private void Start()
    {
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (playerHealth == null) return;

        float max = Mathf.Max(1, playerHealth.maxHealth);
        float current = Mathf.Clamp(playerHealth.health, 0, max);
        float targetFill = current / max;

        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFill, Time.unscaledDeltaTime * 10f);

            // Subtle heartbeat pulse when near death
            if (pulseOnLowHealth && targetFill <= lowHealthThreshold && targetFill > 0f)
            {
                float pulse = 1f + Mathf.Sin(Time.unscaledTime * 8f) * 0.08f;
                fillImage.transform.localScale = new Vector3(pulse, pulse, 1f);
            }
            else
            {
                fillImage.transform.localScale = Vector3.one;
            }
        }

        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(current)} / {playerHealth.maxHealth}";
        }
    }
}
