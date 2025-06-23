using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PersonalizationBoardManager : MonoBehaviour
{
    public Transform tileContainerTransform;
    public Transform pieceContainerTransform;

    public GameObject tilePrefab;
    public GameObject piecePrefab;
    public int rows = 8, cols = 8;
    public float tileSize = 1f;
    public static Piece[,] boardState = new Piece[8, 8];

    public Sprite MetaChess_Pawn;
    public Sprite MetaChess_Knight;
    public Sprite MetaChess_Bishop;
    public Sprite MetaChess_Rook;
    public Sprite MetaChess_Queen;
    public Sprite MetaChess_King;

    public Sprite MetaChess_Pawn_Black;
    public Sprite MetaChess_Knight_Black;
    public Sprite MetaChess_Bishop_Black;
    public Sprite MetaChess_Rook_Black;
    public Sprite MetaChess_Queen_Black;
    public Sprite MetaChess_King_Black;
    public static PersonalizationBoardManager Instance { get; private set; }
    void Awake()
    {
        Instance = this;

    }

    IEnumerator Start()
    {

        GenerateTiles();

        // Wait 1 frame so GridLayoutGroup lays out tiles
        yield return null;

        GeneratePieces();
    }

    void GenerateTiles()
    {
        foreach (Transform child in tileContainerTransform)
        {
            Destroy(child.gameObject);
        }

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GameObject tile = Instantiate(tilePrefab, tileContainerTransform);
                Tile tileScript = tile.GetComponent<Tile>();
                tileScript.gridPosition = new Vector2Int(x, y);

                bool isLight = (x + y) % 2 == 0;
                tileScript.SetColor(isLight ? new Color32(235, 236, 208, 255) : new Color32(115, 149, 82, 255));
            }
        }
    }



    void GeneratePieces()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // Pawns
                if (y == 1 || y == 6)
                {
                    CreatePiece(x, y, PieceType.Pawn, y == 1);
                }

                // Rooks
                if ((x == 0 || x == 7) && (y == 0 || y == 7))
                {
                    CreatePiece(x, y, PieceType.Rook, y == 0);
                }

                // Knights
                if ((x == 1 || x == 6) && (y == 0 || y == 7))
                {
                    CreatePiece(x, y, PieceType.Knight, y == 0);
                }

                // Bishops
                if ((x == 2 || x == 5) && (y == 0 || y == 7))
                {
                    CreatePiece(x, y, PieceType.Bishop, y == 0);
                }

                // Queens
                if (x == 3 && (y == 0 || y == 7))
                {
                    CreatePiece(x, y, PieceType.Queen, y == 0);
                }

                // Kings
                if (x == 4 && (y == 0 || y == 7))
                {
                    CreatePiece(x, y, PieceType.King, y == 0);
                }
            }
        }
    }



    void CreatePiece(int x, int y, PieceType type, bool isWhite)
    {
        // Create piece GameObject
        GameObject pieceGO = Instantiate(piecePrefab, pieceContainerTransform);
        Piece pieceScript = pieceGO.GetComponent<Piece>();
        Image pieceImage = pieceGO.GetComponent<Image>();

        // Assign sprite
        // Check for saved customization
        string savedSpriteName = CustomizationSave.LoadSpriteChoice(type, isWhite);
        Sprite spriteToUse = null;

        if (!string.IsNullOrEmpty(savedSpriteName))
        {
            spriteToUse = SpriteRegistry.GetSpriteByName(savedSpriteName);
        }

        // Fall back to default if no saved sprite
        if (spriteToUse == null)
        {
            spriteToUse = GetSpriteForPiece(type, isWhite);
        }

        pieceImage.sprite = spriteToUse;

        // Setup piece data
        pieceScript.isWhite = isWhite;
        pieceScript.boardPosition = new Vector2Int(x, y);
        pieceScript.pieceType = type;
        pieceScript.originalType = type;
        boardState[x, y] = pieceScript;

        // Consistent tile positioning
        int tileIndex = y * cols + x;
        Transform tileTransform = tileContainerTransform.GetChild(tileIndex);
        RectTransform tileRect = tileTransform.GetComponent<RectTransform>();

        // Match position & size
        RectTransform pieceRect = pieceGO.GetComponent<RectTransform>();
        pieceRect.position = new Vector3(tileRect.position.x, tileRect.position.y, -10f);
        pieceRect.sizeDelta = tileRect.sizeDelta;
    }


    // Helper function to get the correct sprite for a piece type and color
    private Sprite GetSpriteForPiece(PieceType type, bool isWhite)
    {
        switch (type)
        {
            case PieceType.Pawn: return isWhite ? MetaChess_Pawn : MetaChess_Pawn_Black;
            case PieceType.Rook: return isWhite ? MetaChess_Rook : MetaChess_Rook_Black;
            case PieceType.Knight: return isWhite ? MetaChess_Knight : MetaChess_Knight_Black;
            case PieceType.Bishop: return isWhite ? MetaChess_Bishop : MetaChess_Bishop_Black;
            case PieceType.Queen: return isWhite ? MetaChess_Queen : MetaChess_Queen_Black;
            case PieceType.King: return isWhite ? MetaChess_King : MetaChess_King_Black;
            default: return null;
        }
    }
}
