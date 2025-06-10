using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject piecePrefab;

    public Sprite MetaChess_Pawn;
    public Sprite MetaChess_Pawn_Black;
    public Sprite MetaChess_Knight;
    public Sprite MetaChess_Knight_Black;
    public Sprite MetaChess_Bishop;
    public Sprite MetaChess_Bishop_Black;
    public Sprite MetaChess_Rook;
    public Sprite MetaChess_Rook_Black;
    public Sprite MetaChess_Queen;
    public Sprite MetaChess_Queen_Black;
    public Sprite MetaChess_King;
    public Sprite MetaChess_King_Black;

    public int rows = 8, cols = 8;
    public float tileSize = 1f;

    public static BoardManager Instance { get; private set; }
    public static Piece[,] boardState = new Piece[8, 8];

    public static Vector2Int? enPassantTargetSquare = null;

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
                if (y == 1 || y == 6)
                {
                    GameObject pawn = Instantiate(piecePrefab, new Vector3(x, y, -0.1f), Quaternion.identity);
                    Piece pieceScript = pawn.GetComponent<Piece>();
                    SpriteRenderer spriteRenderer = pawn.GetComponent<SpriteRenderer>();
                    pieceScript.isWhite = (y == 1); //if y is 1, it is white; if y is 6, it is black
                    spriteRenderer.sprite = pieceScript.isWhite ? MetaChess_Pawn : MetaChess_Pawn_Black;
                    pieceScript.boardPosition = new Vector2Int(x, y);
                    pieceScript.pieceType = PieceType.Pawn; //ensure the piece type is set
                    boardState[x, y] = pieceScript;
                }
                //create end pieces (standard is rook)
                if ((x == 0 || x == 7) && (y == 0 || y == 7))
                {   
                    GameObject rook = Instantiate(piecePrefab, new Vector3(x, y, -0.1f), Quaternion.identity);
                    Piece pieceScript = rook.GetComponent<Piece>();
                    SpriteRenderer spriteRenderer = rook.GetComponent<SpriteRenderer>();
                    pieceScript.isWhite = (y == 0); //if y is 0, it is white; if y is 7, it is black
                    spriteRenderer.sprite = pieceScript.isWhite ? MetaChess_Rook : MetaChess_Rook_Black;
                    pieceScript.boardPosition = new Vector2Int(x, y);
                    pieceScript.pieceType = PieceType.Rook; //ensure the piece type is set
                    boardState[x, y] = pieceScript;
                }
                //create second end pieces (standard is knight)
                if ((x == 1 || x == 6) && (y == 0 || y == 7))
                {
                    GameObject knight = Instantiate(piecePrefab, new Vector3(x, y, -0.1f), Quaternion.identity);
                    Piece pieceScript = knight.GetComponent<Piece>();
                    SpriteRenderer spriteRenderer = knight.GetComponent<SpriteRenderer>();
                    pieceScript.isWhite = (y == 0); //if y is 0, it is white; if y is 7, it is black
                    spriteRenderer.sprite = pieceScript.isWhite ? MetaChess_Knight : MetaChess_Knight_Black;
                    pieceScript.boardPosition = new Vector2Int(x, y);
                    pieceScript.pieceType = PieceType.Knight; //ensure the piece type is set
                    boardState[x, y] = pieceScript;
                }
                //create third end pieces (standard is bishop)
                if ((x == 2 || x == 5) && (y == 0 || y == 7))
                {
                    GameObject bishop = Instantiate(piecePrefab, new Vector3(x, y, -0.1f), Quaternion.identity);
                    Piece pieceScript = bishop.GetComponent<Piece>();
                    SpriteRenderer spriteRenderer = bishop.GetComponent<SpriteRenderer>();
                    pieceScript.isWhite = (y == 0); //if y is 0, it is white; if y is 7, it is black
                    spriteRenderer.sprite = pieceScript.isWhite ? MetaChess_Bishop : MetaChess_Bishop_Black;
                    pieceScript.boardPosition = new Vector2Int(x, y);
                    pieceScript.pieceType = PieceType.Bishop; //ensure the piece type is set
                    boardState[x, y] = pieceScript;
                }
                //create left middle piece (standard is queen)
                if (x == 3 && (y == 0 || y == 7))
                {
                    GameObject queen = Instantiate(piecePrefab, new Vector3(x, y, -0.1f), Quaternion.identity);
                    Piece pieceScript = queen.GetComponent<Piece>();
                    SpriteRenderer spriteRenderer = queen.GetComponent<SpriteRenderer>();
                    pieceScript.isWhite = (y == 0); //if y is 0, it is white; if y is 7, it is black
                    spriteRenderer.sprite = pieceScript.isWhite ? MetaChess_Queen : MetaChess_Queen_Black;
                    pieceScript.boardPosition = new Vector2Int(x, y);
                    pieceScript.pieceType = PieceType.Queen; //ensure the piece type is set
                    boardState[x, y] = pieceScript;
                }
                //create king
                if (x == 4 && (y == 0 || y == 7))
                {
                    GameObject king = Instantiate(piecePrefab, new Vector3(x, y, -0.1f), Quaternion.identity);
                    Piece pieceScript = king.GetComponent<Piece>();
                    SpriteRenderer spriteRenderer = king.GetComponent<SpriteRenderer>();
                    pieceScript.isWhite = (y == 0); //if y is 0, it is white; if y is 7, it is black
                    spriteRenderer.sprite = pieceScript.isWhite ? MetaChess_King : MetaChess_King_Black;
                    pieceScript.boardPosition = new Vector2Int(x, y);
                    pieceScript.pieceType = PieceType.King; //ensure the piece type is set
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

    // Moves a piece from one position to another
    public void MovePiece(Vector2Int from, Vector2Int to)
    {
        Piece movingPiece = boardState[from.x, from.y];
        Piece targetPiece = boardState[to.x, to.y]; // Check if there is a piece at target

        // Capture logic
        if (targetPiece != null && targetPiece.isWhite != movingPiece.isWhite)
        {
            Destroy(targetPiece.gameObject); // Remove captured piece from scene
        }

        // Move piece
        boardState[to.x, to.y] = movingPiece;
        boardState[from.x, from.y] = null;

        movingPiece.boardPosition = to;
        movingPiece.transform.position = new Vector3(to.x, to.y, -0.1f);

        //set piece as moved for castling and en passant
        movingPiece.hasMoved = true;


        // Pawn promotion 
        //TODO: make this more flexible, e.g. by showing a UI to select the promotion piece
        if (movingPiece.pieceType == PieceType.Pawn)
        {
            // If white pawn reaches rank 7 (y == 7), or black pawn reaches rank 0 (y == 0)
            if ((movingPiece.isWhite && to.y == 7) || (!movingPiece.isWhite && to.y == 0))
            {
                PromotePawn(movingPiece);
            }
        }
        // Clear previous en passant target
        enPassantTargetSquare = null;

        // set en passant target if pawn moves two squares
        if (movingPiece.pieceType == PieceType.Pawn)
        {
            int moveDistance = Mathf.Abs(to.y - from.y);
            if (moveDistance == 2)
            {
                // Set en passant target square, the square the pawn passed through
                int dir = movingPiece.isWhite ? -1 : 1;
                enPassantTargetSquare = new Vector2Int(to.x, to.y + dir);
            }
        }
        // case: en passant capture
        if (movingPiece.pieceType == PieceType.Pawn && BoardManager.enPassantTargetSquare.HasValue && to == BoardManager.enPassantTargetSquare.Value)
        {
            // Capture the pawn that moved two squares (it will be behind the target square)
            int dir = movingPiece.isWhite ? -1 : 1;
            Vector2Int capturedPawnPos = new Vector2Int(to.x, to.y + dir);

            Piece capturedPawn = boardState[capturedPawnPos.x, capturedPawnPos.y];
            if (capturedPawn != null && capturedPawn.pieceType == PieceType.Pawn)
            {
                Destroy(capturedPawn.gameObject);
                boardState[capturedPawnPos.x, capturedPawnPos.y] = null;
            }
        }


        // if king is castling, move the rook
        if (movingPiece.pieceType == PieceType.King && Mathf.Abs(to.x - from.x) == 2)
        {
            // King-side castling
            if (to.x == 6)
            {
                Piece rook = boardState[7, to.y];
                boardState[5, to.y] = rook;
                boardState[7, to.y] = null;

                rook.boardPosition = new Vector2Int(5, to.y);
                rook.transform.position = new Vector3(5, to.y, -0.1f);
                rook.hasMoved = true;
            }
            // Queen-side castling
            else if (to.x == 2)
            {
                Piece rook = boardState[0, to.y];
                boardState[3, to.y] = rook;
                boardState[0, to.y] = null;

                rook.boardPosition = new Vector2Int(3, to.y);
                rook.transform.position = new Vector3(3, to.y, -0.1f);
                rook.hasMoved = true;
            }
        }

    }


    //highlight legal moves for a piece
    public GameObject moveHighlightPrefab;  
    private List<GameObject> currentHighlights = new List<GameObject>();

    public void ShowLegalMoves(Piece piece)
    {

        ClearHighlights();
        var pseudoLegalMoves = piece.GetLegalMoves(boardState);
        var legalMoves = BoardManager.FilterMovesThatCauseCheck(piece, pseudoLegalMoves, boardState);

        foreach (var move in legalMoves)
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

    public Vector3 BoardToWorldPosition(Vector2Int boardPos)
    {
        return new Vector3(boardPos.x, boardPos.y, -0.1f);
    }


    //turns
    public enum PlayerColor { White, Black }
    public PlayerColor currentTurn = PlayerColor.White;

    public void SwitchTurn()
    {
        // Switch turn first
        currentTurn = currentTurn == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;

        // Determine whose king to find
        bool isWhite = currentTurn == PlayerColor.White;
        Vector2Int kingSquare = BoardManager.FindKingSquare(isWhite);

        // Check if the king is in check (attacked by enemy pieces)
        bool inCheck = BoardManager.IsSquareAttacked(kingSquare, !isWhite, BoardManager.boardState);

        //check if its gameover (checkmate or stalemate)
        bool isCheckmate;
        bool gameOver = BoardManager.IsCheckmateOrStalemate(isWhite, out isCheckmate);

        if (gameOver)
        {
            if (isCheckmate)
                Debug.Log("CHECKMATE! " + currentTurn + " loses!");
            else
                Debug.Log("STALEMATE! Game drawn.");
        }
        else
        {
            Debug.Log("Turn switched to: " + currentTurn);
        }

    }


    //checks if a single square is attacked by a piece by looping through all pieces on the board
    public static bool IsSquareAttacked(Vector2Int square, bool byWhite, Piece[,] boardState)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Piece p = boardState[x, y];
                //if no piece or piece is not of the correct color, skip
                if (p != null && p.isWhite == byWhite)
                {
                    if (p.CanAttackSquare(square, boardState))
                        return true;
                }
            }
        }
        return false;
    }


    //find the king's square for a given color
    public static Vector2Int FindKingSquare(bool isWhite)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Piece p = boardState[x, y];
                if (p != null && p.isWhite == isWhite && p.pieceType == PieceType.King)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        // Should never happen unless the king is missing
        return new Vector2Int(-1, -1);
    }

    //prevents moves that would leave the own king in check
    public static List<Vector2Int> FilterMovesThatCauseCheck(Piece piece, List<Vector2Int> pseudoLegalMoves, Piece[,] boardState)
    {
        List<Vector2Int> legalMoves = new List<Vector2Int>();

        foreach (Vector2Int move in pseudoLegalMoves)
        {
            // Save original board state
            Piece capturedPiece = boardState[move.x, move.y];
            Vector2Int originalPos = piece.boardPosition;

            // Simulate move
            boardState[originalPos.x, originalPos.y] = null;
            boardState[move.x, move.y] = piece;
            piece.boardPosition = move;

            // Find own king square
            bool isWhite = piece.isWhite;
            Vector2Int kingSquare = FindKingSquare(isWhite);

            // Check if king is attacked
            bool kingInCheck = IsSquareAttacked(kingSquare, !isWhite, boardState);

            // If move does NOT leave king in check add it
            if (!kingInCheck)
            {
                legalMoves.Add(move);
            }

            // Undo simulated move restore board
            piece.boardPosition = originalPos;
            boardState[originalPos.x, originalPos.y] = piece;
            boardState[move.x, move.y] = capturedPiece;
        }

        return legalMoves;
    }

     
    public static bool IsCheckmateOrStalemate(bool isWhite, out bool isCheckmate)
    {
        isCheckmate = false;

        // Check if player's king is in check
        Vector2Int kingSquare = FindKingSquare(isWhite);
        bool kingInCheck = IsSquareAttacked(kingSquare, !isWhite, boardState);

        // Loop over all pieces of this player
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Piece p = boardState[x, y];
                if (p != null && p.isWhite == isWhite)
                {
                    // Get pseudo-legal moves
                    List<Vector2Int> pseudoLegalMoves = p.GetLegalMoves(boardState);

                    // Filter moves that would leave king in check
                    List<Vector2Int> legalMoves = FilterMovesThatCauseCheck(p, pseudoLegalMoves, boardState);

                    // If any legal move exists, not checkmate or stalemate
                    if (legalMoves.Count > 0)
                        return false;
                }
            }
        }

        // No legal moves:
        isCheckmate = kingInCheck;
        return true; // Either checkmate or stalemate
    }

    private void PromotePawn(Piece pawn)
    {
        // Auto promote to Queen (simple version)
        pawn.pieceType = PieceType.Queen;

        // Change sprite
        SpriteRenderer sr = pawn.GetComponent<SpriteRenderer>();
        sr.sprite = pawn.isWhite ? MetaChess_Queen : MetaChess_Queen_Black;

    }
    
    private void Castle(Piece king)
    {

    }

        
}
