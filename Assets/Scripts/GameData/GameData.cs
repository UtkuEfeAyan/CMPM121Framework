using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    private Dictionary<string, EnemyData> enemyTypes = new Dictionary<string, EnemyData>();
    private List<LevelData> levels = new List<LevelData>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        LoadEnemies();
        LoadLevels();
    }

    private void LoadEnemies()
    {
        var enemyList = JSONParser.LoadAsList<EnemyData>("enemies");
        if (enemyList == null)
        {
            Debug.LogError("GameData: Failed to load enemies.");
            return;
        }

        foreach (var enemy in enemyList)
        {
            if (!enemyTypes.ContainsKey(enemy.name))
                enemyTypes.Add(enemy.name, enemy);
            else
                Debug.LogWarning($"GameData: Duplicate enemy name found: {enemy.name}");
        }

        Debug.Log($"GameData: Loaded {enemyTypes.Count} enemies.");
    }

    private void LoadLevels()
    {
        var levelList = JSONParser.LoadAsList<LevelData>("levels");
        if (levelList == null)
        {
            Debug.LogError("GameData: Failed to load levels.");
            return;
        }

        levels = levelList;
        Debug.Log($"GameData: Loaded {levels.Count} levels.");
    }

    // Public Read-Only Access

    public EnemyData GetEnemyByName(string name)
    {
        if (enemyTypes.ContainsKey(name))
            return enemyTypes[name];

        Debug.LogWarning($"GameData: Enemy '{name}' not found.");
        return null;
    }

    public LevelData GetLevelByName(string name)
    {
        return levels.Find(level => level.name == name);
    }

    public List<EnemyData> GetAllEnemies()
    {
        return new List<EnemyData>(enemyTypes.Values);
    }

    public List<LevelData> GetAllLevels()
    {
        return new List<LevelData>(levels);
    }
}