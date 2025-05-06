using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SpellData
{
    public string name;
    public string description;
    public int icon;
    public string N;
    public DamageData damage;
    public string secondary_damage;
    public string mana_cost;
    public string cooldown;
    public string knockback;
    public ProjectileData projectile;
    public ProjectileData secondary_projectile;
}
public class DamageData
{
    public string ammount;
    public string type;
}
public class ProjectileData
{
    string trajectory;
    string lifetime;
    string speed;
    int sprite;
}