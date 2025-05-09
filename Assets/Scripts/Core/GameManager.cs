using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager 
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
    private static GameManager theInstance;
    private int hackRand;
    public static GameManager Instance {  get
        {
            if (theInstance == null){
                theInstance = new GameManager();
                theInstance.hackRand = 0;
            }
            theInstance.hackRand ++;
            return theInstance;
        }
    }

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

    private List<GameObject> enemies;
    public int enemy_count { get { return enemies.Count; } }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }
    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
        enemiesKilled++; // Increment enemies killed


        var ec = enemy.GetComponent<EnemyController>();
        if (ec != null)
        {
            waveScore += ec.score; // Add enemy score when enemy is killed
        }
    }

    public GameObject GetClosestEnemy(Vector3 point)
    {
        if (enemies == null || enemies.Count == 0) return null;
        if (enemies.Count == 1) return enemies[0];
        return enemies.Aggregate((a,b) => (a.transform.position - point).sqrMagnitude < (b.transform.position - point).sqrMagnitude ? a : b);
    }
    //Call this every frame during gameplay
    public void UpdateTimer()
    {
        if (state == GameState.INWAVE)
        {
            elapsedTime += Time.deltaTime;
        }
    }
    private GameManager()
    {
        enemies = new List<GameObject>();
    }
    // added to fix the bug of enemeis left keeepingthe count of enemeis after reset
    public void ResetGameManager()
    {
        enemies.Clear();
        elapsedTime = 0;
        enemiesKilled = 0;
        projectilesFired = 0;
        waveScore = 0; //Reset score too
        state = GameState.PREGAME;
    }
    public int GetHackRand(){
        return hackRand;
    }
    public uint GetWave(){
        if (enemySpawner == null)
            return 0;
        return enemySpawner.GetWave();
    }
}
