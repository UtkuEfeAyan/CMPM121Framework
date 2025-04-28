using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{

    public Transform target;
    public float speed;
    public Hittable hp;
    public float damage;
    public HealthBar healthui;
    public bool dead;
    public string onDeath;

    public float last_attack;
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
            ;
            GameManager.Instance.RemoveEnemy(gameObject);
            Destroy(gameObject);
        }
    }
}
