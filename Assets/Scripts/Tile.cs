using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int gridPosition;
    public Piece currentPiece;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        sr.color = color;
    }

}
