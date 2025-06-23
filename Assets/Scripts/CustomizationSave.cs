using UnityEngine;

public static class CustomizationSave
{
    private const string KeyPrefix = "CustomSprite_";

    public static void SaveSpriteChoice(PieceType type, bool isWhite, string spriteName)
    {
        string key = $"Customization_{type}_{(isWhite ? "White" : "Black")}";
        PlayerPrefs.SetString(key, spriteName);
    }

    public static string LoadSpriteChoice(PieceType type, bool isWhite)
    {
        string key = $"Customization_{type}_{(isWhite ? "White" : "Black")}";
        return PlayerPrefs.GetString(key, "");
    }

    public static bool HasSavedChoice(PieceType type)
    {
        return PlayerPrefs.HasKey(KeyPrefix + type.ToString());
    }

    public static void SaveReplacementPieceType(PieceType originalType, bool isWhite, PieceType newType)
    {
        string key = $"{originalType}_{(isWhite ? "White" : "Black")}_ReplacementType";
        PlayerPrefs.SetString(key, newType.ToString());

    }

    public static PieceType LoadReplacementPieceType(PieceType originalType, bool isWhite)
    {
        string key = $"{originalType}_{(isWhite ? "White" : "Black")}_ReplacementType";
        string saved = PlayerPrefs.GetString(key, originalType.ToString());

        if (System.Enum.TryParse(saved, out PieceType result))
            return result;



        return originalType;
    }

}
