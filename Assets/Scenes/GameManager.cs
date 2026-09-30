using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public bool IsGameOver { get; private set; }

    private void Start()
    {
        IsGameOver = false;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void PlayerDied()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        var menu = FindFirstObjectByType<DungeonMenuController>();
        if (menu != null) menu.NotifyGameOver();
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }
}
