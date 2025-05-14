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

    public void RefreshAllSpells()
    {
        // Changed to start from 0 and handle nulls
        for (int i = 0; i < spellUIs.Length; i++)
        {
            spellUIs[i].SetActive(true);
            bool hasSpell = i < player.spellcaster.spells.Count &&
                           player.spellcaster.spells[i] != null;

            spellUIs[i].GetComponent<SpellUI>().SetSpell(hasSpell ? player.spellcaster.spells[i] : null);
        }
    }



}
