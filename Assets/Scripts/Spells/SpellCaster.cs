//Editor: utku efe ayan

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellCaster
{
    public int mana;
    public int max_mana;
    public int mana_reg;
    public Hittable.Team team;
    public List<Spell> spells;

    public IEnumerator ManaRegeneration()
    {
        while (true)
        {
            mana += mana_reg;
            mana = Mathf.Min(mana, max_mana);
            yield return new WaitForSeconds(1);
        }
    }

    public SpellCaster(int mana, int mana_reg, Hittable.Team team)
    {
        this.mana = mana;
        this.max_mana = mana;
        this.mana_reg = mana_reg;
        this.team = team;

        spells = new List<Spell>
        {
            // Start with the consistent predefined spell setup
            new SpellBuilder().Build(this, new List<string>{"wild","wild","arcane_bolt"})
        };
    }

    public IEnumerator Cast(int index, Vector3 where, Vector3 target)
    {
        if (index < 0 || index >= spells.Count)
            yield break;

        var spell = spells[index];
        if (mana >= spell.GetManaCost() && spell.IsReady())
        {
            mana -= spell.GetManaCost();
            yield return spell.Cast(where, target, team);
        }
    }
}
