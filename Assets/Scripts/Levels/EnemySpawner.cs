//Editor: Xavier Austin (ui and level handling and enemy spawning)
//Editor: Efe Ayan (enemy spawning, next wave and multiple other fucntions for handling the end game and stats requirements, also music)
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using UnityEngine.SocialPlatforms.Impl;
//using System;

public class EnemySpawner : MonoBehaviour
{
    public Image level_selector;
    public GameObject button;
    public GameObject enemy;
    public SpawnPoint[] SpawnPoints;
    //expected range is 1 to uint max; 0 if StartLevel has yet to have been called
    private uint waveNum;
    //current difficulty information; only loaded from file once
    private LevelData levelJSON;
    //every enemy; only loaded from file once
    private List<EnemyData> enemiesJSON;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //unity makes me dizzy
        GameObject selector0 = Instantiate(button, level_selector.transform);
        selector0.transform.localPosition = new Vector3(0, 130);
        selector0.GetComponent<MenuSelectorController>().spawner = this;
        selector0.GetComponent<MenuSelectorController>().SetLevel("Easy");
        GameObject selector1 = Instantiate(button, level_selector.transform);
        selector1.transform.localPosition = new Vector3(0, 0);
        selector1.GetComponent<MenuSelectorController>().spawner = this;
        selector1.GetComponent<MenuSelectorController>().SetLevel("Medium");
        GameObject selector2 = Instantiate(button, level_selector.transform);
        selector2.transform.localPosition = new Vector3(0, -130);
        selector2.GetComponent<MenuSelectorController>().spawner = this;
        selector2.GetComponent<MenuSelectorController>().SetLevel("Endless");

        enemiesJSON = JSONParser.Instance.LoadAsList<EnemyData>("enemies");
        waveNum = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLevel(string levelname)
    {
        var levelsJSON = JSONParser.Instance.LoadAsList<LevelData>("levels");
        foreach (var iter in levelsJSON){
            if (iter.name == levelname)
                levelJSON = iter;
        }
        level_selector.gameObject.SetActive(false);
        // this is not nice: we should not have to be required to tell the player directly that the level is starting
        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();
        //Play appropriate music!
        MusicManager.Instance.PlayMusicForDifficulty(levelname);
        NextWave();
    }

    public void NextWave()
    {
        waveNum ++;

        // Give bonus points for surviving the previous wave
        if (waveNum > 1) // Only after Wave 1 (not at start)
            GameManager.Instance.waveScore += (int)((waveNum - 1) * 100);
        // if waves arn't specified assume endless otherwise use wave total
        // Endless mode: waves == 0
        if (levelJSON.waves == 0)
        {
            StartCoroutine(SpawnWave());
            return; // Important: STOP HERE for endless mode!
        }
        // Normal fixed-wave mode
        if (waveNum < levelJSON.waves)
        {
            StartCoroutine(SpawnWave());
        }
        else
        {
            GameManager.Instance.state = GameManager.GameState.GAMEOVER;
        }
    }

    IEnumerator SpawnWave()
    {
        GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
        GameManager.Instance.countdown = 3;
        for (int i = 3; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
            GameManager.Instance.countdown--;
        }
        GameManager.Instance.state = GameManager.GameState.INWAVE;
        //using system namespace brings up compile error bc system and unity each have their own random function
        //very fixable but i'm lazy!
        //if (levelJSON.spawns == null)
        //    throw new ArgumentException("levels.json is missing spawns for selected difficulty");
        foreach (var enemyDisc in levelJSON.spawns){
            List<int> tempList = new List<int>{1};
            SpawnData temp = new SpawnData();
            temp.enemy      = enemyDisc.enemy; //default not defined in assignment
            temp.count      = enemyDisc.count; //default not defined in assignment
            temp.hp         = enemyDisc.hp ?? "base";
            temp.speed      = enemyDisc.speed ?? "base";
            temp.damage     = enemyDisc.damage ?? "base";
            temp.delay      = enemyDisc.delay ?? "2";
            temp.sequence   = enemyDisc.sequence ?? tempList;
            temp.location   = enemyDisc.location ?? "random";
            //handle spawn sequence
            float spawnNum = RPNParser.Instance.DoParse(temp.count,new Dictionary<string, float>{{ "wave", waveNum }});
            for (int j = 0; spawnNum > 0; j++){
                for (int i = 0; (spawnNum > 0) && (i < temp.sequence[j%temp.sequence.Count]); i ++){
                    yield return SpawnEnemies(temp);
                    spawnNum --;
                }
                yield return new WaitForSeconds(RPNParser.Instance.DoParse(temp.delay));
            }
        }
        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);
        GameManager.Instance.state = GameManager.GameState.WAVEEND;
    }

    IEnumerator SpawnEnemies(SpawnData data)
    {
        GameObject new_enemy = EnemyToSpawnByName(data.enemy);
        /*
        Vector2 offset = Random.insideUnitCircle * 1.8f;
        List<SpawnPoint> MySpawns = new List<SpawnPoint>{};
        foreach (SpawnPoint possible in SpawnPoints){
            if ((possible.kind == SpawnPoint.SpawnName.RED && data.location == "random red") 
             || (possible.kind == SpawnPoint.SpawnName.GREEN && data.location == "random green") 
             || (possible.kind == SpawnPoint.SpawnName.BONE && data.location == "random bone"))
                MySpawns.Add(possible);
        }
        new_enemy.transform.position = MySpawns[Random.Range(0, MySpawns.Count)].transform.position + new Vector3(offset.x, offset.y, 0);
        */
        //comment to push and do pull request
        EnemyController en = new_enemy.GetComponent<EnemyController>();
        en.hp = new Hittable(RPNParser.Instance.DoParse(data.hp, new Dictionary<string, float>{{ "wave", waveNum }, {"base", en.hp.hp}}), Hittable.Team.MONSTERS, new_enemy);
        en.speed = RPNParser.Instance.DoParse(data.speed, new Dictionary<string, float>{{ "wave", waveNum }, {"base", en.speed}});
        en.damage = RPNParser.Instance.DoParse(data.damage, new Dictionary<string, float>{{ "wave", waveNum }, {"base", en.damage}});
        GameManager.Instance.AddEnemy(new_enemy);
        yield return new WaitForSeconds(0.5f);
    }

    public GameObject EnemyToSpawnByName(string name)
    {
        float hp = 0;
        float speed = 0;
        float damage = 0;
        int sprite = 0;
        int childN = 0;
        string child = null;
        string childW = null;
        int score = 0; // Add score variable

        // Find the enemy data that matches the given name
        foreach (var enemyData in enemiesJSON)
        {
            if (enemyData.name == name)
            {
                hp = enemyData.hp;
                speed = enemyData.speed;
                damage = enemyData.damage;
                child = enemyData.child;
                childW = enemyData.childWhen;
                childN = enemyData.childNum;
                sprite = enemyData.sprite;
                score = enemyData.score; // Store score properly
                break; // Found it, stop looping
            }
        }

        SpawnPoint spawn_point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        Vector2 offset = Random.insideUnitCircle * 1.8f;
        Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);
        
        GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity);

        //sprite selector
        new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(sprite);
        EnemyController en = new_enemy.GetComponent<EnemyController>();
        //stats
        en.hp = new Hittable(hp, Hittable.Team.MONSTERS, new_enemy);
        en.speed = speed;
        en.damage = damage;
        en.child = child;
        en.childNum = childN;
        en.spawner = this;
        en.score = score; // if you read 'score' from JSON
        //Debug.Log(name);
        return new_enemy;
    }

    public uint GetWave(){
        return waveNum;
    }
}
