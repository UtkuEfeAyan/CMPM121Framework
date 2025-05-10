using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndScreenManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI statsText;

    private bool shown = false;

    void Update()
    {
        if (shown) return;

        if (GameManager.Instance == null || GameManager.Instance.player == null)
            return;

        var playerController = GameManager.Instance.player.GetComponent<PlayerController>();
        if (playerController == null || playerController.hp == null)
            return;

        if (GameManager.Instance.state == GameManager.GameState.GAMEOVER)
        {
            gameOverPanel.SetActive(true);

            if (playerController.hp.hp <= 0)
            {
                resultText.text = "Game Over!";
            }
            else
            {
                resultText.text = "Victory!";
            }

            statsText.text =
                "Waves survived: " + GetWaveNumber() + "\n" +
                "Time alive: " + (GameManager.Instance.elapsedTime).ToString("F1") + " seconds\n" +
                "Enemies killed: " + GameManager.Instance.enemiesKilled + "\n" +
                "Projectiles fired: " + GameManager.Instance.projectilesFired + "\n" +
                "Wave score: " + GameManager.Instance.waveScore;

            shown = true;
        }
    }

    public void ReturnToMenu()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ResetGameManager();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    int GetWaveNumber()
    {
        return (int)GameManager.Instance.GetWave();
    }

    int CalculateScore()
    {
        return GetWaveNumber() * 100; // simple score: 100 points per wave
    }
}
