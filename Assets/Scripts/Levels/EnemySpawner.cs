//Editor: Xavier Austin (ui and level handling)
//Editor: Efe Ayan (enemy spawning)
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
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
        NextWave();
    }

    public void NextWave()
    {
        // if waves arn't specified assume endless otherwise use wave total
        if (levelJSON.waves == 0)
            StartCoroutine(SpawnWave());
        if (waveNum < levelJSON.waves)
            StartCoroutine(SpawnWave());
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
        //using system namespace brings up compile error bc system and unity each have their own random
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
        waveNum ++;
        GameManager.Instance.state = GameManager.GameState.WAVEEND;
    }

    IEnumerator SpawnEnemies(SpawnData data)
    {
        float  hp     = 0;
        float  speed  = 0;
        float  damage = 0;
        int    sprite = 0;
        int    childN = 0;
        string child  = null;
        string childW = null;
        foreach (var enemyDisc in enemiesJSON){
            if (data.enemy == enemyDisc.name){
                Debug.Log(data.hp);
                hp     = RPNParser.Instance.DoParse(data.hp,    new Dictionary<string, float>{{ "wave", waveNum }, {"base", enemyDisc.hp}});
                speed  = RPNParser.Instance.DoParse(data.speed, new Dictionary<string, float>{{ "wave", waveNum }, {"base", enemyDisc.speed}});
                damage = RPNParser.Instance.DoParse(data.damage,new Dictionary<string, float>{{ "wave", waveNum }, {"base", enemyDisc.damage}});
                child  = enemyDisc.child; 
                childW = enemyDisc.childWhen; 
                childN = enemyDisc.childNum; 
                sprite = enemyDisc.sprite;
                Debug.Log($"{child}, {childW}, {childN}");
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
        GameManager.Instance.AddEnemy(new_enemy);
        yield return new WaitForSeconds(0.5f);
    }

    uint GetWave(){
        return waveNum;
    }
}
