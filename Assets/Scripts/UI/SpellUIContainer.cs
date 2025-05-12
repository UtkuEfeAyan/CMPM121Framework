using UnityEngine;

public class SpellUIContainer : MonoBehaviour
{
    public GameObject[] spellUIs; // UI slots
    public PlayerController player;
    private int lastHighlight = -1;

    void Start()
    {
        RefreshAllSpells();
        UpdateHighlight(0); // Start on spell 0
    }

    public void RefreshAllSpells()
    {
        for (int i = 1; i < spellUIs.Length; i++)
        {
            spellUIs[i].SetActive(true); // Always show the slot

            if (i < player.spellcaster.spells.Count)
            {
                spellUIs[i].GetComponent<SpellUI>().SetSpell(player.spellcaster.spells[i]);
            }
            else
            {
                spellUIs[i].GetComponent<SpellUI>().ClearSpell(); // Show as empty
            }
        }
    }

    public void UpdateHighlight(int selectedIndex)
    {
        if (lastHighlight >= 0 && lastHighlight < spellUIs.Length)
        {
            spellUIs[lastHighlight].GetComponent<SpellUI>().highlight.SetActive(false);
        }

        if (selectedIndex >= 0 && selectedIndex < spellUIs.Length)
        {
            spellUIs[selectedIndex].GetComponent<SpellUI>().highlight.SetActive(true);
            lastHighlight = selectedIndex;
        }
    }

}
