//reworked i will lbe marking which one i made changes so you can gee themeasier like this on top(efe)
using UnityEngine;

public class SpellIconManager : IconManager
{
    [SerializeField] Sprite defaultSprite;

    void Start()
    {
        GameManager.Instance.spellIconManager = this;
    }

    public new Sprite Get(int index)
    {
        if (index < 0 || index >= sprites.Length)
        {
            Debug.LogWarning($"Invalid spell icon index: {index}");
            return defaultSprite;
        }
        return sprites[index];
    }
}