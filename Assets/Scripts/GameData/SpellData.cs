//author: Xavier Austin
using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SpellData
{
    //base spells
    public string name;
    public string description;
    public int icon = -1; //could've been nullable int but as a c programmer that statment makes me cringe
    //also the whole using uninitalized variables makes me cringe too but whatever
    public string N;
    public DamageData damage;
    public string secondary_damage;
    public string mana_cost;
    public string cooldown;
    public ProjectileData projectile;
    public ProjectileData secondary_projectile;
    //modifiers
    public string damage_multiplier;
    public string speed_multiplier;
    public string cooldown_multiplier;
    public string mana_multiplier;
    public string mana_adder;
    public string delay;
    public string angle;
    public string projectile_trajectory;
    public string knockback;
    public string crit;
}
public class DamageData
{
    public string amount;
    public string type;
}
public class ProjectileData
{
    public string trajectory;
    public string lifetime;
    public string speed;
    public int sprite;
}