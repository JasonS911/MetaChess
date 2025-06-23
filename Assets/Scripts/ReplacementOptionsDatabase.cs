using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReplacementOptionsDatabase", menuName = "Chess/Replacement Database")]
public class ReplacementOptionsDatabase : ScriptableObject
{
    public List<PieceReplacementSet> replacementSets;

    public List<PieceReplacementSet.ReplacementOption> GetOptionsForPiece(PieceType type, bool isWhite)
    {
        foreach (var set in replacementSets)
        {
            if (set.pieceType == type)
            {
                return set.replacementOptions.FindAll(option =>
                    isWhite ? option.whiteSprite != null : option.blackSprite != null
                );
            }
        }

        return new List<PieceReplacementSet.ReplacementOption>();
    }

}
