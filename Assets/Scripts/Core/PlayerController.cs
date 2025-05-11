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
        void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            currentSpellIndex = 0;
            GetComponent<SpellUIContainer>().UpdateHighlight(0);
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            currentSpellIndex = 1;
            GetComponent<SpellUIContainer>().UpdateHighlight(1);
        }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            currentSpellIndex = 2;
            GetComponent<SpellUIContainer>().UpdateHighlight(2);
        }
        else if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            currentSpellIndex = 3;
            GetComponent<SpellUIContainer>().UpdateHighlight(3);
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
