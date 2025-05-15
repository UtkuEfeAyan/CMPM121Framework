//Editor: Xavier Austin (this documentation is probably unnessesary cuz git but yk)
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

public class SpellBuilder 
{
    Dictionary<string, SpellData> spells;
    List<string> modSpellIds;
    List<string> spellIds;
    public Spell Build(SpellCaster owner, List<string> idWithMods = null)
    {
        SpellData tmp = new SpellData();
        tmp.damage = new DamageData();
        tmp.projectile = new ProjectileData();
        //more likely for spells to have fewer modifiers
        //https://docs.google.com/spreadsheets/d/1X-_UfeW2mAwUQv6uW-ZxrBZj2EktS3KD1O0_uItuPJ0/edit?usp=sharing
        int maxMods = UnityEngine.Random.Range(0,21)/UnityEngine.Random.Range(1,11);
        //thought it was gonna be way more of a hastle to get namespaces to cooperate
        //i didn't realize you could do namespace.function in c# that's funky and weird
        string id = "";
        List<string> repeats = new List<string>();
        //if idWithMods is not a passed arg generate idWithMods randomly
        if (idWithMods == null){
            idWithMods = new List<string>();
            for (int i = 0; i < maxMods; i++){
                idWithMods.Add(modSpellIds[UnityEngine.Random.Range(0,modSpellIds.Count)]);
            }
            idWithMods.Add(spellIds[UnityEngine.Random.Range(0, spellIds.Count)]);
        }
        //parse args
        foreach (string idInArg in idWithMods){
            SpellData current = spells[idInArg];
            if ((!spellIds.Contains(idInArg)) && (!modSpellIds.Contains(idInArg)))
                throw new ArgumentException("Spell has a spell id undefined by JSON");
            if (modSpellIds.Contains(idInArg)){
                //concat modifiers to spell name and add apropriate description (once for clutter avoidance)
                tmp.name += current.name + " ";
                tmp.description += (!repeats.Contains(idInArg))? $" {current.name}: {current.description}" : "";
                //handle modifier stat changes
                tmp.damage.amount += (current.damage_multiplier != null)? $" {current.damage_multiplier} *": "";
                tmp.mana_cost += (current.mana_adder != null)? $" {current.mana_adder} +": "";
                tmp.mana_cost += (current.mana_multiplier != null)? $" {current.mana_multiplier} *": "";
                tmp.cooldown += (current.cooldown_multiplier != null)? $" {current.cooldown_multiplier} *": "";
                tmp.crit += (current.crit != null)? $" {current.crit} +": "";
                tmp.projectile.speed += (current.speed_multiplier != null)? $" {current.speed_multiplier} *": "";
                //modify projectile trajectory; given how projectiles are currently implemented this is suprisingly best
                if (current.projectile_trajectory != null)
                    tmp.projectile.trajectory = current.projectile_trajectory;
                repeats.Add(idInArg);
            }else if (id != "")
                throw new ArgumentException("Spell has two base spells; Spell modifiers should not have icons");
            else
                id = idInArg;
        }
        //what a mess
        //visual information
        tmp.name += spells[id].name;
        tmp.description = spells[id].description + tmp.description;
        tmp.icon = spells[id].icon;
        //stats
        tmp.damage.amount = spells[id].damage.amount + tmp.damage.amount;
        tmp.mana_cost = spells[id].mana_cost + tmp.mana_cost;
        tmp.cooldown = spells[id].cooldown + tmp.cooldown;
        tmp.crit = ((spells[id].crit != null)? spells[id].crit : "0") + tmp.crit;
        //projectile mechanics
        if (tmp.projectile.trajectory == null)
            tmp.projectile.trajectory = spells[id].projectile.trajectory;
        tmp.projectile.speed = spells[id].projectile.speed + tmp.projectile.speed;
        tmp.projectile.lifetime = spells[id].projectile.lifetime?? "0";
        tmp.projectile.sprite = spells[id].projectile.sprite;
        //number of projectiles
        tmp.N = spells[id].N;
        //ive hardly done any memory conservation but here have a crumb
        //secondary projectile
        if (spells[id].secondary_projectile != null){
            tmp.secondary_projectile = new ProjectileData();
            tmp.secondary_projectile.trajectory = spells[id].secondary_projectile.trajectory;
            tmp.secondary_projectile.lifetime = spells[id].secondary_projectile.lifetime;
            tmp.secondary_projectile.speed = spells[id].secondary_projectile.speed;
            tmp.secondary_projectile.sprite = spells[id].secondary_projectile.sprite;
        }
        Debug.Log(tmp.name);
        return new Spell(owner, tmp);
    }

    public SpellBuilder()
    {        
        spells = JSONParser.LoadAsDictionary<string, SpellData>("spells");
        modSpellIds = new List<string>();
        spellIds = new List<string>();
        foreach (string spellID in spells.Keys){
            //spells that are modifiers don't have icons
            //hacky way to do it for scaleability 
            //but i love gambling and betting on modifiers never having icons is a safe bet
            if (spells[spellID].icon == -1)
                modSpellIds.Add(spellID);
            else
                spellIds.Add(spellID);
        }
    }
}
