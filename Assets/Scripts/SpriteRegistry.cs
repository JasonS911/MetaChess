using System.Collections.Generic;
using UnityEngine;

public class SpriteRegistry : MonoBehaviour
{
    public List<Sprite> allSprites; // Set this in the Inspector

    private static Dictionary<string, Sprite> spriteLookup;

    void Awake()
    {

        // Build a lookup table: sprite name, sprite asset
        spriteLookup = new Dictionary<string, Sprite>();
        foreach (Sprite sprite in allSprites)
        {
            if (sprite != null)
            {
                spriteLookup[sprite.name] = sprite;
            }
        }
    }

    public static Sprite GetSpriteByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        return spriteLookup.TryGetValue(name, out var sprite) ? sprite : null;
    }
}
