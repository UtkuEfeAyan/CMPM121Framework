using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;
public class Spell 
{
    public float last_cast;
    public SpellCaster owner;
    public Hittable.Team team;
    private string name;
    private string description;
    private int icon;
    private int N;
    private int damage;
    private int secondary_damage;
    private int mana_cost;
    private float cooldown;
    private float knockback;
    private float lifetime;
    private string trajectory;
    private float speed;
    private int sprite;
    private ProjectileData secondary_projectile;
    private int crit;

    public Spell(SpellCaster owner, SpellData data)
    {
        uint waveNum = GameManager.Instance.GetWave();
        Dictionary<string, float> vars = new Dictionary<string, float>{{ "wave", waveNum }, {"power", 20}};
        this.name = data.name;
        this.description = data.description;
        this.mana_cost = Convert.ToInt32(RPNParser.DoParse(data.mana_cost, vars));
        this.damage = Convert.ToInt32(RPNParser.DoParse(data.damage.amount, vars));
        this.cooldown = Convert.ToInt32(RPNParser.DoParse(data.cooldown, vars));
        this.crit = Convert.ToInt32(RPNParser.DoParse(data.crit, vars));
        this.icon = data.icon;
        this.owner = owner;
        this.lifetime = RPNParser.DoParse(data.projectile.lifetime, vars);
        this.trajectory = data.projectile.trajectory;
        this.sprite = data.projectile.sprite;
        this.speed = RPNParser.DoParse(data.projectile.speed, vars);
    }

    public string GetName()
    {
        return this.name;
    }

    public int GetManaCost()
    {
        return this.mana_cost;
    }

    public int GetDamage()
    {
        return this.damage;
    }

    public float GetCooldown()
    {
        return this.cooldown;
    }

    public virtual int GetIcon()
    {
        return icon;
    }

    public bool IsReady()
    {
        return (last_cast + GetCooldown() < Time.time);
    }

    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;
        if (this.lifetime == 0)
            GameManager.Instance.projectileManager.CreateProjectile(this.sprite, this.trajectory, where, target - where, this.speed, OnHit);
        else 
            GameManager.Instance.projectileManager.CreateProjectile(this.sprite, this.trajectory, where, target - where, this.speed, OnHit, this.lifetime);
        yield return new WaitForEndOfFrame();
    }

    void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            int temp = this.damage;
            this.damage = (UnityEngine.Random.Range(0,100) < this.crit)? this.damage * 6: this.damage;
            other.Damage(new Damage(GetDamage(), Damage.Type.ARCANE));
            this.damage = temp;
        }

    }

}
