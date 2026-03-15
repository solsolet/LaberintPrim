using UnityEngine;
using TMPro;          // TextMeshPro — comes built into Unity 6

public class UIManager : MonoBehaviour
{
    [Header("HUD elements (always visible)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI levelText;

    [Header("Game Over panel")]
    public GameObject      gameOverPanel;
    public TextMeshProUGUI gameOverScoreText;   // shows final score inside the panel
 
    [Header("Victory panel")]
    public GameObject      victoryPanel;
    public TextMeshProUGUI victoryScoreText;    // shows final score inside the panel



    // Call this whenever score / lives / level changes
    public void Refresh()
    {
        if (scoreText) scoreText.text = "Score: " + GameManager.Instance.score;
        if (livesText) livesText.text = BuildLivesString(GameManager.Instance.lives);
        if (levelText) levelText.text = "Lv: " + (GameManager.Instance.currentLevel + 1);
    }

    // One heart per life remaining, dark heart for lost lives
    string BuildLivesString(int current)
    {
        int maxLives = 3;
        string result = "";
        for (int i = 0; i < maxLives; i++)
            result += (i < current) ? "♥" : "🖤";
        return result;
    }

    public void ShowGameOver()
    {
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
            if (gameOverScoreText)
                gameOverScoreText.text = "Score: " + GameManager.Instance.score + " pts";
        }
        Time.timeScale = 0f; // Pause the game
    }

    public void ShowVictory()
    {
        if (victoryPanel)
        {
            victoryPanel.SetActive(true);
            if (victoryScoreText)
                victoryScoreText.text = "Score: " + GameManager.Instance.score + " pts";
        }
        Time.timeScale = 0f;
    }

    // Call this from the "Restart" button in both panels
    public void OnRestartButton()
    {
        Time.timeScale = 1f; // Un-pause before reloading!
        GameManager.Instance.Restart();
    }
}