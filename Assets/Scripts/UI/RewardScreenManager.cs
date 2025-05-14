using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            else
            {
                Debug.LogError("Slot missing Button component: " + slot.name);
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
        else
        {
            if (rewardScreen.activeSelf)
            {
                rewardScreen.SetActive(false);
                CleanupSlots();
                rewardPending = false;
            }
        }
    }

    void ShowRewardScreen()
    {
        rewardScreen.SetActive(true);
        generatedRewardSpell = new SpellBuilder().Build(
            GameManager.Instance.player.GetComponent<PlayerController>().spellcaster
        );
        UpdateUI();
        SetSlotsInteractable(true);
    }

    void UpdateUI()
    {
        rewardStatsText.text = GetWaveStats();
        rewardSpellUI.SetSpell(generatedRewardSpell);

        // Update player spell slots
        var spellcaster = GameManager.Instance.player.GetComponent<PlayerController>().spellcaster;
        for (int i = 0; i < playerSpellSlots.Length; i++)
        {
            playerSpellSlots[i].SetSpell(i < spellcaster.spells.Count ? spellcaster.spells[i] : null);
        }
    }

    void OnSlotSelected(SpellUI selectedSlot)
    {
        int slotIndex = System.Array.IndexOf(playerSpellSlots, selectedSlot);
        if (slotIndex == -1) return;

        var spellcaster = GameManager.Instance.player.GetComponent<PlayerController>().spellcaster;

        // Replace or add spell
        if (spellcaster.spells.Count > slotIndex)
            spellcaster.spells[slotIndex] = generatedRewardSpell;
        else
            spellcaster.spells.Add(generatedRewardSpell);

        UpdateUI();
        SetSlotsInteractable(false);
        rewardScreen.SetActive(false);
        rewardPending = false;
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