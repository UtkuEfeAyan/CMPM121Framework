//Editor: utku efe ayan
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI manacost;
    public TextMeshProUGUI damage;
    public GameObject highlight;

    private Spell currentSpell;

    public void SetSpell(Spell spell)
    {
        if (spell == null || GameManager.Instance == null || GameManager.Instance.spellIconManager == null)
        {
            Debug.LogWarning("SpellUI.SetSpell was given an invalid or incomplete spell.");
            return;
        }

        currentSpell = spell;

        if (icon != null)
        {
            icon.enabled = true;
            icon.sprite = GameManager.Instance.spellIconManager.Get(spell.GetIcon());
        }

        if (manacost != null)
            manacost.text = spell.GetManaCost().ToString();

        if (damage != null)
            damage.text = spell.GetDamage().ToString();
    }

    public void ClearSpell()
    {
        currentSpell = null;

        if (icon != null) icon.enabled = false;
        if (manacost != null) manacost.text = "";
        if (damage != null) damage.text = "";
    }

    void Update() { }
}
