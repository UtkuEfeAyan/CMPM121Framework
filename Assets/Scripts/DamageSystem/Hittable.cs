using UnityEngine;
using System;

public class Hittable
{

    public enum Team { PLAYER, MONSTERS }
    public Team team;

    public int hp;
    public int max_hp;

    public GameObject owner;

    //public event Action OnHit;
    public event Action OnDeath;
    public void Damage(Damage damage)
    {
        EventBus.Instance.DoDamage(owner.transform.position, damage, this);
        hp -= damage.amount;
        //OnHit?.Invoke(); // added on hit i think
        //i looked around for similar statements and actually found one in the event bus

        if (hp <= 0)
        {
            hp = 0;
            OnDeath();
        }
    }

    

    public Hittable(float hp, Team team, GameObject owner)
    {
        this.hp = Convert.ToInt32(hp);
        this.max_hp = this.hp;
        this.team = team;
        this.owner = owner;
    }

    public void SetMaxHP(int max_hp)
    {
        float perc = this.hp * 1.0f / this.max_hp;
        this.max_hp = max_hp;
        this.hp = Mathf.RoundToInt(perc * max_hp);
    }
}
