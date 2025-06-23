using UnityEngine;
using UnityEngine.EventSystems;

public class PersonaliationPiecePrefabClickHandler : MonoBehaviour, IPointerClickHandler
{
    public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King }

    public Piece pieceData;

    //this method is called when user clicks on board piece
    //tells Perosnaliozation manager which piece was clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        if (pieceData == null)
        {
            return;
        }

        // Call PersonalizationManager to show replacement options
        PersonalizationManager.Instance.OnPieceSelected(pieceData);
    }
}
