using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PieceReplacementSet", menuName = "Chess/Replacement Set")]
public class PieceReplacementSet : ScriptableObject
{
    public PieceType pieceType;
    [System.Serializable]
    public class ReplacementOption
    {
        public string displayName;
        public Sprite blackSprite; // shown in replacement UI
        public Sprite whiteSprite; // applied to white pieces
        public PieceType resultingType;

    }
    public List<ReplacementOption> replacementOptions;

}
