using TMPro;
using UnityEngine;

public class RewardScreenManager : MonoBehaviour
{
    public GameObject rewardScreen;
    public TextMeshProUGUI rewardStatsText;


  

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //  Automatically find the RewardStatsText by tag
        GameObject statsObject = GameObject.FindGameObjectWithTag("RewardStatsText");
        if (statsObject != null)
        {
            rewardStatsText = statsObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogWarning("RewardStatsText with tag not found!");
        }

        if (rewardScreen != null)
            rewardScreen.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        //if (rewardStatsText != null)
        //    Debug.Log("Stats Text: " + rewardStatsText.text);

        if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            rewardScreen.SetActive(true);
                rewardStatsText.text =
                "Waves survived: " + GetWaveNumber() + "\n" +
                "Time elapsed: " + (GameManager.Instance.elapsedTime).ToString("F1") + " seconds\n" +
                "Enemies killed: " + GameManager.Instance.enemiesKilled + "\n" +
                "Projectiles fired: " + GameManager.Instance.projectilesFired + "\n" +
                "Wave score: " + GameManager.Instance.waveScore;

        }
        else
        {
            if (rewardScreen != null)
                rewardScreen.SetActive(false); // hide the screen during non-WAVEEND states
        }
    }
    // we use the below code twice at minimum  can probavbl;y take it out  of  thes isntance and make one script called by multiple thigns
    int GetWaveNumber()
    {
        return (int)FindFirstObjectByType<EnemySpawner>().GetWave();
    }
}
