using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{

    public Transform target;
    public float speed;
    public Hittable hp;
    public float damage;
    public string child;
    public string childWhen;
    public int childNum;
    public HealthBar healthui;
    public bool dead;
    public string onDeath;
    public EnemySpawner spawner;
    public float last_attack;

    public int score; // Score this enemy gives when killed
    private int myRand;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameManager.Instance.player.transform;
        hp.OnDeath += Die;
        EventBus.Instance.OnDamage += OnDamage;
        healthui.SetHealth(hp);
        myRand = UnityEngine.Random.Range(0,314);
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
            Unstuck();
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
            if (childWhen == "death")
                SpawnChildren();
            dead = true;
            GameManager.Instance.RemoveEnemy(gameObject);
            Destroy(gameObject);
        }
    }
    void OnDamage(Vector3 where, Damage dmg, Hittable target){
        if (childWhen == "hit" && hp == target)
            SpawnChildren();
    }
    void SpawnChildren(){
        for (int i = 0; (child != null) && (i < childNum); i++){//for (int i = 1; (child != null) && (i < childNum + 1); i++){
            GameObject new_enemy = spawner.EnemyToSpawnByName(child);
            new_enemy.transform.position = transform.position; //+ new Vector3(Convert.ToSingle(1.8*Math.Cos(i)*i), Convert.ToSingle(1.8*Math.Sin(i)*i), 0);
            GameManager.Instance.AddEnemy(new_enemy);
        }
    }

    void Unstuck(){
        GetComponent<Collider2D>().enabled = false;
        Vector3 pos3 = GetComponent<Unit>().transform.position;
        Vector2 pos = new Vector2(pos3.x,pos3.y);
        Vector2 tmp0 = new Vector2(-0.499f,-0.499f);
        Vector2 tmp1 = new Vector2(-0.6f,-0.6f);
        //if (Physics2D.OverlapArea(pos+tmp,pos-tmp) != null)
        //    Debug.Log(GetComponent<Unit>().transform.position);
        if (Physics2D.OverlapArea(pos+tmp0,pos-tmp0) != null)
            for (int i = 0; (Physics2D.OverlapArea(pos+tmp1,pos-tmp1) != null) && (i < 300); i++){
                float tempA = i + myRand;
                float tempX = Mathf.Cos(tempA)*i*0.3f;
                float tempY = Mathf.Sin(tempA)*i*0.3f;
                GetComponent<Unit>().transform.position = pos3 + new Vector3(tempX, tempY, 0);
                pos += new Vector2(tempX, tempY);
            }
        GetComponent<Collider2D>().enabled = true;
    }
}
