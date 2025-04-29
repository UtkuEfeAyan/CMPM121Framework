using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{

    public Transform target;
    public float speed;
    public Hittable hp;
    public float damage;
    public string child;
    public int childNum;
    public HealthBar healthui;
    public bool dead;
    public string onDeath;
    public EnemySpawner spawner;
    public float last_attack;

    public int score; // Score this enemy gives when killed


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameManager.Instance.player.transform;
        hp.OnDeath += Die;
        healthui.SetHealth(hp);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = target.position - transform.position;
        if (direction.magnitude < 2f)
        {
            DoAttack();
        }
        else
        {
            GetComponent<Unit>().movement = direction.normalized * speed;
        }
    }
    
    void DoAttack()
    {
        if (last_attack + 2 < Time.time)
        {
            last_attack = Time.time;
            target.gameObject.GetComponent<PlayerController>().hp.Damage(new Damage(Convert.ToInt32(damage), Damage.Type.PHYSICAL));
        }
    }


    void Die()
    {
        //Die can be ran multiple times in one frame from multiple sources so
        //this activates once with this implementation
        if (!dead)
        {
            dead = true;
            if (child != "" && child != null){
                Debug.Log(child);
                for (int i = 0; i < childNum; i++){
                    GameObject new_enemy = spawner.EnemyToSpawnByName(child);
                    new_enemy.transform.position = transform.position + new Vector3(Convert.ToSingle(1.8*Math.Cos(i)*i), Convert.ToSingle(1.8*Math.Sin(i)*i), 0);
                    GameManager.Instance.AddEnemy(new_enemy);
                }
            }
            GameManager.Instance.RemoveEnemy(gameObject);
            Destroy(gameObject);
        }
    }
}
