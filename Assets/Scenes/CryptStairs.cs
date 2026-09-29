using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CryptStairs : MonoBehaviour
{
    [Header("Descent Settings")]
    public string nextLevelMessage = "Descending into the deeper crypts...";
    public float delayBeforeTransition = 1.5f;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;

        if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
        {
            isTransitioning = true;
            Debug.Log("<color=red>[Diablo Descent]</color> " + nextLevelMessage);

            // Trigger level reload or transition
            Invoke(nameof(AdvanceLevel), delayBeforeTransition);
        }
    }

    private void AdvanceLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
