//Editor: utku efe ayan

using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public Hittable hp;
    public HealthBar healthui;
    public ManaBar manaui;

    public SpellCaster spellcaster;
    public SpellUI spellui;

    public int speed;

    public Unit unit;

    private int currentSpellIndex = 0; // selected spell slot (0-3)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unit = GetComponent<Unit>();
        GameManager.Instance.player = gameObject;

        StartLevel();
    }


    public void StartLevel()
    {
        if (spellcaster != null)
            return; // Already initialized

        spellcaster = new SpellCaster(125, 8, Hittable.Team.PLAYER);
        StartCoroutine(spellcaster.ManaRegeneration());

        hp = new Hittable(100, Hittable.Team.PLAYER, gameObject);
        hp.OnDeath += Die;
        hp.team = Hittable.Team.PLAYER;

        // tell UI elements what to show
        healthui.SetHealth(hp);
        manaui.SetSpellCaster(spellcaster);

        if (spellui != null && spellcaster.spells.Count > 0)
            spellui.SetSpell(spellcaster.spells[0]);
    }

    // Update is called once per frame
    [Header("UI References")]
    public SpellUI[] spellSlots; // Assign all 4 spell slots in Inspector

    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME ||
            GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;

        int newIndex = -1;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) newIndex = 0;
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) newIndex = 1;
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) newIndex = 2;
        else if (Keyboard.current.digit4Key.wasPressedThisFrame) newIndex = 3;

        if (newIndex != -1)
        {
            UpdateSelectedSpell(newIndex);
        }
    }

    void UpdateSelectedSpell(int index)
    {
        // Validate index range
        if (index < 0 || index >= spellSlots.Length) return;

        // Clear previous highlight
        if (currentSpellIndex >= 0 && currentSpellIndex < spellSlots.Length)
        {
            spellSlots[currentSpellIndex].highlight.SetActive(false);
        }

        // Set new highlight
        currentSpellIndex = index;
        if (spellSlots[currentSpellIndex] != null)
        {
            spellSlots[currentSpellIndex].highlight.SetActive(true);
        }
    }

    public void RefreshSpellDisplays()
    {
        if (spellcaster == null) return;

        for (int i = 0; i < spellSlots.Length; i++)
        {
            bool hasSpell = i < spellcaster.spells.Count;
            spellSlots[i].SetSpell(hasSpell ? spellcaster.spells[i] : null);
        }

    }

        void OnAttack(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;

        Vector2 mouseScreen = Mouse.current.position.value;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;

        if (spellcaster.spells != null && currentSpellIndex < spellcaster.spells.Count)
        {
            StartCoroutine(spellcaster.Cast(currentSpellIndex, transform.position, mouseWorld));
        }

        // Add null check and slot validation
        if (spellcaster.spells != null &&
            currentSpellIndex < spellcaster.spells.Count &&
            spellcaster.spells[currentSpellIndex] != null)
        {
            StartCoroutine(spellcaster.Cast(currentSpellIndex, transform.position, mouseWorld));
        }
        else
        {
            Debug.Log("No spell in selected slot: " + currentSpellIndex);
        }
    }

    void OnMove(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;
        unit.movement = value.Get<Vector2>()*speed;// + Vector2.up;
    }

    void Die()
    {
        //Debug.Log("You Lost");
        GameManager.Instance.state = GameManager.GameState.GAMEOVER;
    }

}
