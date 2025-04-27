//Author: Efe Ayan
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Simple test script to check if JSONParser is loading enemies and levels correctly.
/// Attach this to any GameObject in your scene to test.
/// </summary>
public class JSONParserTester : MonoBehaviour
{
    void Start()
    {
        TestEnemyLoading();
        TestLevelLoading();
    }

    private void TestEnemyLoading()
    {
        var enemies = JSONParser.Instance.LoadAsList<EnemyData>("enemies");

        if (enemies == null)
        {
            Debug.LogError("Failed to load enemies!");
            return;
        }

        Debug.Log($"Successfully loaded {enemies.Count} enemies!");
        foreach (var enemy in enemies)
        {
            Debug.Log($"Enemy: {enemy.name}, HP: {enemy.hp}, Speed: {enemy.speed}, Damage: {enemy.damage}, Sprite: {enemy.sprite}");
        }
    }

    private void TestLevelLoading()
    {
        var levels = JSONParser.Instance.LoadAsList<LevelData>("levels");

        if (levels == null)
        {
            Debug.LogError("Failed to load levels!");
            return;
        }

        Debug.Log($"Successfully loaded {levels.Count} levels!");
        foreach (var level in levels)
        {
            Debug.Log($"Level: {level.name}, Waves: {level.waves}");
            foreach (var spawn in level.spawns)
            {
                Debug.Log($"  Spawn Enemy: {spawn.enemy}, Count Expression: {spawn.count}, HP Expression: {spawn.hp}");
            }
        }
    }
}
