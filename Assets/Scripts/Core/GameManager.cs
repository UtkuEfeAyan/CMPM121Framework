using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        PREGAME,
        INWAVE,
        WAVEEND,
        COUNTDOWN,
        GAMEOVER
    }

    public GameState state;
    public int countdown;
    public GameObject player;

    public ProjectileManager projectileManager;
    public SpellIconManager spellIconManager;
    public EnemySpriteManager enemySpriteManager;
    public PlayerSpriteManager playerSpriteManager;
    public RelicIconManager relicIconManager;
    public EnemySpawner enemySpawner;

    public float elapsedTime;
    public int enemiesKilled;
    public int projectilesFired;
    public int waveScore; // This will be your total score

    private int hackRand = 0;

    private List<GameObject> enemies;
    public int enemy_count { get { return enemies.Count; } }

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        enemies = new List<GameObject>();
        DontDestroyOnLoad(this.gameObject); // Optional: persist across scenes
    }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
        enemiesKilled++;

        var ec = enemy.GetComponent<EnemyController>();
        if (ec != null)
        {
            waveScore += ec.score;
        }
    }

    public GameObject GetClosestEnemy(Vector3 point)
    {
        if (enemies == null || enemies.Count == 0) return null;
        if (enemies.Count == 1) return enemies[0];
        return enemies.Aggregate((a, b) => (a.transform.position - point).sqrMagnitude < (b.transform.position - point).sqrMagnitude ? a : b);
    }

    public void UpdateTimer()
    {
        if (state == GameState.INWAVE)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    public void ResetGameManager()
    {
        enemies.Clear();
        elapsedTime = 0;
        enemiesKilled = 0;
        projectilesFired = 0;
        waveScore = 0;
        state = GameState.PREGAME;
    }

    public int GetHackRand()
    {
        return ++hackRand;
    }

    public uint GetWave()
    {
        if (enemySpawner == null)
            return 0;
        return enemySpawner.GetWave();
    }
}
