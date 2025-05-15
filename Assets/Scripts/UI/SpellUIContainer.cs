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
        // Validate array bounds first
        if (selectedIndex < 0 || selectedIndex >= spellUIs.Length)
        {
            Debug.LogWarning($"Invalid spell index: {selectedIndex}");
            return;
        }

        // Clear previous highlight
        if (lastHighlight >= 0 && lastHighlight < spellUIs.Length)
        {
            var previousHighlight = spellUIs[lastHighlight]?.GetComponent<SpellUI>()?.highlight;
            if (previousHighlight != null)
            {
                previousHighlight.SetActive(false);
            }
        }

        // Set new highlight
        var newHighlight = spellUIs[selectedIndex]?.GetComponent<SpellUI>();
        if (newHighlight != null)
        {
            if (newHighlight.highlight != null)
            {
                newHighlight.highlight.SetActive(true);
                lastHighlight = selectedIndex;
            }
            else
            {
                Debug.LogWarning($"No highlight object on spell slot {selectedIndex}");
            }
        }
        else
        {
            Debug.LogWarning($"Missing SpellUI component on slot {selectedIndex}");
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
