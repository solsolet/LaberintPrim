using UnityEngine;
using TMPro;          // TextMeshPro — comes built into Unity 6

public class UIManager : MonoBehaviour
{
    [Header("HUD elements (always visible)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI levelText;

    [Header("Panels (GameObjects with CanvasGroup or just SetActive)")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    // Call this whenever score / lives / level changes
    public void Refresh()
    {
        if (scoreText) scoreText.text = "Score: " + GameManager.Instance.score;
        if (livesText) livesText.text = "Lives: " + GameManager.Instance.lives;
        if (levelText) levelText.text = "Level: " + (GameManager.Instance.currentLevel + 1);
    }

    public void ShowGameOver()
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    public void ShowVictory()
    {
        if (victoryPanel) victoryPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // Call this from the "Restart" button in both panels
    public void OnRestartButton()
    {
        Time.timeScale = 1f; // Un-pause before reloading!
        GameManager.Instance.Restart();
    }
}