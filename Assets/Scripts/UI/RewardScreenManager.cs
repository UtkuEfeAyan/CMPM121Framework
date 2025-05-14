// reworked by efe
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class RewardScreenManager : MonoBehaviour
{
    // Existing references
    public GameObject rewardScreen;
    public TextMeshProUGUI rewardStatsText;
    public SpellUI rewardSpellUI;

    // New references
    public SpellUI[] playerSpellSlots; // Assign your 4 spell slot buttons here
    private Spell generatedRewardSpell;
    private bool rewardPending;

    void Start()
    {
        rewardScreen.SetActive(false);
        InitializeSlots();
    }

    void InitializeSlots()
    {
        foreach (var slot in playerSpellSlots)
        {
            var button = slot.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnSlotSelected(slot));
            }
        }
    }

    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            if (!rewardPending)
            {
                ShowRewardScreen();
                rewardPending = true;
            }
        }
        else if (rewardScreen.activeSelf)
        {
            rewardScreen.SetActive(false);
            CleanupSlots();
            rewardPending = false;
        }
    }

    void ShowRewardScreen()
    {
        rewardScreen.SetActive(true);
        GenerateRewardSpell();
        UpdateUI();
        SetSlotsInteractable(true);
    }

    void GenerateRewardSpell()
    {
        var builder = new SpellBuilder();
        generatedRewardSpell = builder.Build(
            GameManager.Instance.player.GetComponent<PlayerController>().spellcaster
        );
    }

    void UpdateUI()
    {
        rewardStatsText.text = GetWaveStats();
        rewardSpellUI.SetSpell(generatedRewardSpell);
        RefreshPlayerSlots();
    }

    void RefreshPlayerSlots()
    {
        var spellcaster = GameManager.Instance.player.GetComponent<PlayerController>().spellcaster;
        for (int i = 0; i < playerSpellSlots.Length; i++)
        {
            bool hasSpell = i < spellcaster.spells.Count && spellcaster.spells[i] != null;
            playerSpellSlots[i].SetSpell(hasSpell ? spellcaster.spells[i] : null);
        }
    }

    void OnSlotSelected(SpellUI selectedSlot)
    {
        int slotIndex = System.Array.IndexOf(playerSpellSlots, selectedSlot);
        if (slotIndex == -1) return;

        var pc = GameManager.Instance.player.GetComponent<PlayerController>();
        var spellcaster = pc.spellcaster;

        // Ensure spell list has enough slots
        while (spellcaster.spells.Count <= slotIndex)
        {
            spellcaster.spells.Add(null);
        }

        // Replace spell
        spellcaster.spells[slotIndex] = generatedRewardSpell;

        // Update UI immediately
        pc.GetComponent<SpellUIContainer>().RefreshAllSpells();

        // Visual feedback
        StartCoroutine(HighlightSlot(selectedSlot));

        rewardScreen.SetActive(false);
        rewardPending = false;
    }

    IEnumerator HighlightSlot(SpellUI slot)
    {
        if (slot != null && slot.highlight != null)
        {
            slot.highlight.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            slot.highlight.SetActive(false);
        }
    }


    void SetSlotsInteractable(bool interactable)
    {
        foreach (var slot in playerSpellSlots)
        {
            var button = slot.GetComponent<Button>();
            if (button != null) button.interactable = interactable;
        }
    }

    void CleanupSlots()
    {
        foreach (var slot in playerSpellSlots)
        {
            var button = slot.GetComponent<Button>();
            if (button != null) button.onClick.RemoveAllListeners();
        }
    }

    string GetWaveStats()
    {
        return $"Waves survived: {GetWaveNumber()}\n" +
               $"Time alive: {GameManager.Instance.elapsedTime:F1}s\n" +
               $"Enemies killed: {GameManager.Instance.enemiesKilled}\n" +
               $"Projectiles fired: {GameManager.Instance.projectilesFired}\n" +
               $"Wave score: {GameManager.Instance.waveScore}";
    }

    int GetWaveNumber() => (int)FindFirstObjectByType<EnemySpawner>().GetWave();
}