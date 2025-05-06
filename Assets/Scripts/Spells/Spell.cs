using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
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
    public ProjectileData projectile;
    public ProjectileData secondary_projectile;

    public Spell(SpellCaster owner)
    {
        //debug
        this.name = "Bolt";
        this.mana_cost = 10;
        this.damage = 100;
        this.cooldown = 0.75f;
        this.icon = 0;
        //debug end
        this.owner = owner;
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
        GameManager.Instance.projectileManager.CreateProjectile(0, "straight", where, target - where, 15f, OnHit);
        yield return new WaitForEndOfFrame();
    }

    void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage(GetDamage(), Damage.Type.ARCANE));
        }

    }

}
