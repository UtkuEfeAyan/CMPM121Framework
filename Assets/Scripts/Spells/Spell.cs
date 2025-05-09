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
    public string name;
    public string description;
    public int icon;
    public int N;
    public int damage;
    public int secondary_damage;
    public int mana_cost;
    public float cooldown;
    public float knockback;
    public float lifetime;
    public ProjectileData secondary_projectile;
    public int crit;

    public Spell(SpellCaster owner, SpellData data)
    {
        uint waveNum = GameManager.Instance.GetWave();
        Dictionary<string, float> vars = new Dictionary<string, float>{{ "wave", waveNum }, {"power", 20}};
        this.name = data.name;
        this.description = data.description;
        this.mana_cost = Convert.ToInt32(RPNParser.Instance.DoParse(data.mana_cost, vars));
        this.damage = Convert.ToInt32(RPNParser.Instance.DoParse(data.damage.amount, vars));
        this.cooldown = Convert.ToInt32(RPNParser.Instance.DoParse(data.cooldown, vars));
        this.crit = Convert.ToInt32(RPNParser.Instance.DoParse(data.crit, vars));
        this.icon = data.icon;
        this.owner = owner;
        this.lifetime = RPNParser.Instance.DoParse(data.projectile.lifetime, vars);
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
            GameManager.Instance.projectileManager.CreateProjectile(0, "straight", where, target - where, 15f, OnHit);
        else 
            GameManager.Instance.projectileManager.CreateProjectile(0, "straight", where, target - where, 15f, OnHit, this.lifetime);
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
