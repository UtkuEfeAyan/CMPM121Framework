
//Editor: utku efe ayan
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellUI : MonoBehaviour
{
    public GameObject icon;
    public RectTransform cooldown;
    public TextMeshProUGUI manacost;
    public TextMeshProUGUI damage;
    public GameObject highlight;
    public GameObject dropbutton;

    private Spell currentSpell;
    float last_text_update;
    const float UPDATE_DELAY = 1f;

    void Start()
    {
        last_text_update = 0f;
    }

    public void SetSpell(Spell spell)
    {
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

        if (manacost != null) manacost.text = "";
        if (damage != null) damage.text = "";

        if (icon != null)
        {
            var img = icon.GetComponent<Image>();
            if (img != null)
            {
                img.enabled = false;
                img.sprite = null;
            }
        }

        if (dropbutton != null)
            dropbutton.SetActive(false);
    }



    public void DropSpell()
    {
        if (currentSpell == null)
        {
            if (manacost != null) manacost.text = "";
            if (damage != null) damage.text = "";

            if (icon != null)
            {
                var img = icon.GetComponent<Image>();
                if (img != null)
                {
                    img.enabled = false;
                    img.sprite = null;
                }
            }

            if (dropbutton != null)
                dropbutton.SetActive(false);
        }
    
        }

    void Update()
    {
        if (currentSpell == null)
            return;

        if (Time.time > last_text_update + UPDATE_DELAY)
        {
            if (manacost != null) manacost.text = currentSpell.GetManaCost().ToString();
            if (damage != null) damage.text = currentSpell.GetDamage().ToString();
            last_text_update = Time.time;
        }

        float since_last = Time.time - currentSpell.last_cast;
        float perc = since_last > currentSpell.GetCooldown() ? 0 : 1 - (since_last / currentSpell.GetCooldown());
        if (cooldown != null)
            cooldown.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 48 * perc);
    }
}
