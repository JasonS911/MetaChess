using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject whitePawnPrefab;


    public int rows = 8, cols = 8;
    public float tileSize = 1f;

    public static BoardManager Instance { get; private set; }
    public static Piece[,] boardState = new Piece[8, 8];
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GenerateBoard();
    }

    void GenerateBoard()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 pos = new Vector3(x * tileSize, y * tileSize, 0);
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                tile.transform.SetParent(transform);

                Tile tileScript = tile.GetComponent<Tile>();
                tileScript.gridPosition = new Vector2Int(x, y);

                bool isLight = (x + y) % 2 == 0;
                tileScript.SetColor(isLight ? Color.white : Color.gray);

                //Create pawns
                if (y == 1)
                {
                    GameObject pawn = Instantiate(whitePawnPrefab, new Vector3(x, y, -0.1f), Quaternion.identity);
                    Piece pieceScript = pawn.GetComponent<Piece>();
                    pieceScript.isWhite = true;
                    pieceScript.boardPosition = new Vector2Int(x, y);
                    pieceScript.pieceType = PieceType.Pawn; //ensure the piece type is set
                    boardState[x, y] = pieceScript;
                }
            }

        }
    }

    public Piece GetPieceAt(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= cols || pos.y < 0 || pos.y >= rows)
            return null;

        return boardState[pos.x, pos.y];
    }

    public bool IsTileEmpty(Vector2Int pos)
    {
        return GetPieceAt(pos) == null;
    }

    public void MovePiece(Vector2Int from, Vector2Int to)
    {
        Piece movingPiece = boardState[from.x, from.y];
        boardState[to.x, to.y] = movingPiece;
        boardState[from.x, from.y] = null;

        movingPiece.boardPosition = to;
        movingPiece.transform.position = new Vector3(to.x, to.y, -0.1f);
    }


    //highlight legal moves
    public GameObject moveHighlightPrefab;  
    private List<GameObject> currentHighlights = new List<GameObject>();

    public void ShowLegalMoves(Piece piece)
    {
        ClearHighlights();
        var moves = piece.GetLegalMoves(boardState);
        foreach (var move in moves)
        {
            Vector3 pos = new Vector3(move.x, move.y, 0.0f);
            GameObject highlight = Instantiate(moveHighlightPrefab, pos, Quaternion.identity);
            currentHighlights.Add(highlight);
        }
    }

    public void ClearHighlights()
    {
        foreach (var go in currentHighlights)
            Destroy(go);
        currentHighlights.Clear();
    }

}
