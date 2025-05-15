//Author: Efe Ayan
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


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
        TestSpellLoading();
    }

    private void TestEnemyLoading()
    {
        var enemies = JSONParser.LoadAsList<EnemyData>("enemies");

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
        var levels = JSONParser.LoadAsList<LevelData>("levels");

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

    private void TestSpellLoading()
    {
        var spells = JSONParser.LoadAsDictionary<string, SpellData>("spells");
        
        if (spells == null)
        {
            Debug.LogError("Failed to load spells!");
            return;
        }

        Debug.Log($"Successfully loaded {spells.Count} spells!");
        //Debug.Log($"Successfully loaded {} spells!");
        foreach (string spellID in spells.Keys)
        {
            string amount = "";
            if (spells[spellID].damage != null)
                amount = spells[spellID].damage.amount;
            Debug.Log($"Name: {spells[spellID].name}, Description: {spells[spellID].description}, Icon: {spells[spellID].icon}, Damage: {amount}");
        }
    }
}
