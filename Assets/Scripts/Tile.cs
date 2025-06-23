using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public Vector2Int gridPosition;
    public Piece currentPiece;

    private Image sr;

    private void Awake()
    {
        sr = GetComponent<Image>();
    }

    public void SetColor(Color color)
    {
        sr.color = color;
    }

}
