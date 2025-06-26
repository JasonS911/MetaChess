using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public Transform tileContainerTransform;
    public Transform pieceContainerTransform;
    public Transform highlightContainerTransform;


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
    public Sprite MetaChess_Scout;
    public Sprite MetaChess_Scout_Black;



    public int rows = 8, cols = 8;
    public float tileSize = 1f;

    public static BoardManager Instance { get; private set; }
    public static Piece[,] boardState = new Piece[8, 8];

    public static Vector2Int? enPassantTargetSquare = null;
    public GameOverUI gameOverUI;

    void Awake()
    {
        Instance = this;

    }
    public float whiteTimeRemaining;
    public float blackTimeRemaining;
    private bool gameStarted = false;

    public bool gameOver = false;
    IEnumerator Start()
    {

        GenerateTiles();

        // Wait 1 frame so GridLayoutGroup lays out tiles
        yield return null;

        GeneratePieces();
        float startingTime = GameManager.Instance.selectedTimeMinutes * 60f;
        gameStarted = true;

        SoundManager.Instance.PlayGameStartSound();
        whiteTimeRemaining = startingTime;
        blackTimeRemaining = startingTime;
    }

    public Transform moveHistoryContent;
    public void ClearBoard()
    {
        foreach (Transform piece in pieceContainerTransform)
        {
            Destroy(piece.gameObject);
        }

        foreach (Transform moveHistory in moveHistoryContent)
        {
            Destroy(moveHistory.gameObject);
        }
        moveHistory.Clear();
    }

    //restart game
    public void Rematch()
    {
        ClearBoard();
        StartCoroutine(SetupBoard());
        timerRunning = true;
        isWhiteTurn = true; //for move history
        currentTurn = PlayerColor.White; //for board move
    }

    public GameObject gameOverOverlay;

    public IEnumerator SetupBoard()
    {
        GenerateTiles();
        yield return null; // wait for layout
        GeneratePieces();
        float startingTime = GameManager.Instance.selectedTimeMinutes * 60f;
        whiteTimeRemaining = startingTime;
        blackTimeRemaining = startingTime;
        if (gameOverOverlay != null)
            gameOverOverlay.SetActive(false);

        SoundManager.Instance?.PlayGameStartSound();
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

    void CreatePiece(int x, int y, PieceType originalType, bool isWhite)
    {

        // Load any saved replacement type (e.g., Scout instead of Pawn)
        PieceType actualType = CustomizationSave.LoadReplacementPieceType(originalType, isWhite);

        // Create piece GameObject
        GameObject pieceGO = Instantiate(piecePrefab, pieceContainerTransform);
        Piece pieceScript = pieceGO.GetComponent<Piece>(); //pieceScript is the piece
        Image pieceImage = pieceGO.GetComponent<Image>();

        // Assign sprite from saved customization if available
        string savedSpriteName = CustomizationSave.LoadSpriteChoice(originalType, isWhite);
        Sprite spriteToUse = null;

        if (!string.IsNullOrEmpty(savedSpriteName))
        {
            spriteToUse = SpriteRegistry.GetSpriteByName(savedSpriteName);
        }

        // Fallback to default sprite
        if (spriteToUse == null)
        {
            spriteToUse = GetSpriteForPiece(actualType, isWhite);
        }

        pieceImage.sprite = spriteToUse;

        // Setup piece data
        pieceScript.isWhite = isWhite;
        pieceScript.boardPosition = new Vector2Int(x, y);
        pieceScript.pieceType = actualType; 
        boardState[x, y] = pieceScript;

        // Position on board
        int tileIndex = y * cols + x;
        Transform tileTransform = tileContainerTransform.GetChild(tileIndex);
        RectTransform tileRect = tileTransform.GetComponent<RectTransform>();
        RectTransform pieceRect = pieceGO.GetComponent<RectTransform>();
        pieceRect.position = new Vector3(tileRect.position.x, tileRect.position.y, -10f);
        pieceRect.sizeDelta = tileRect.sizeDelta;

    }



    // Helper function to get the correct sprite for a piece type and color
    private Sprite GetSpriteForPiece(PieceType type, bool isWhite)
    {
        string savedSpriteName = CustomizationSave.LoadSpriteChoice(type, isWhite);

        if (!string.IsNullOrEmpty(savedSpriteName))
        {
            Sprite customSprite = SpriteRegistry.GetSpriteByName(savedSpriteName);
            if (customSprite != null)
                return customSprite;
        }

        // Fallback to default
        switch (type)
        {
            case PieceType.Pawn: return isWhite ? MetaChess_Pawn : MetaChess_Pawn_Black;
            case PieceType.Rook: return isWhite ? MetaChess_Rook : MetaChess_Rook_Black;
            case PieceType.Knight: return isWhite ? MetaChess_Knight : MetaChess_Knight_Black;
            case PieceType.Bishop: return isWhite ? MetaChess_Bishop : MetaChess_Bishop_Black;
            case PieceType.Queen: return isWhite ? MetaChess_Queen : MetaChess_Queen_Black;
            case PieceType.King: return isWhite ? MetaChess_King : MetaChess_King_Black;
            case PieceType.Scout: return isWhite ? MetaChess_Scout : MetaChess_Scout_Black;

            default: return null;
        }
    }

    public Vector2 GetPieceAnchoredPosition(Vector2Int boardPosition)
    {
        GridLayoutGroup grid = tileContainerTransform.GetComponent<GridLayoutGroup>();

        float cellSize = grid.cellSize.x;
        float spacingX = grid.spacing.x;
        float spacingY = grid.spacing.y;

        int boardHeight = rows;

        return new Vector2(
            boardPosition.x * (cellSize + spacingX),
            (boardHeight - 1 - boardPosition.y) * (cellSize + spacingY)
        );
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

        // Move piece logically
        boardState[to.x, to.y] = movingPiece;
        boardState[from.x, from.y] = null;
        movingPiece.boardPosition = to;
        movingPiece.hasMoved = true;


        // Move piece visually to match the tile
        int tileIndex = to.y * cols + to.x;
        Transform tileTransform = tileContainerTransform.GetChild(tileIndex);
        RectTransform tileRect = tileTransform.GetComponent<RectTransform>();

        RectTransform pieceRect = movingPiece.GetComponent<RectTransform>();
        pieceRect.position = tileRect.position;

        //move history 
        bool isCapture = targetPiece != null && targetPiece.isWhite != movingPiece.isWhite;

        string moveNotation = BoardManager.FormatMove(movingPiece, from, to, isCapture);
        Sprite pieceSprite = movingPiece.GetComponent<Image>().sprite;

        BoardManager.AddMoveToHistory(moveNotation, pieceSprite);


        // Pawn and scout promotion
        if (movingPiece.pieceType == PieceType.Pawn || movingPiece.pieceType == PieceType.Scout)
        {
            if ((movingPiece.isWhite && to.y == 7) || (!movingPiece.isWhite && to.y == 0))
            {
                SoundManager.Instance.PlayPromoteSound();
                ShowPromotionOptions(movingPiece); // Show promotion options UI
                return;
            }
        }

        // En passant capture
        if (movingPiece.pieceType == PieceType.Pawn && BoardManager.enPassantTargetSquare.HasValue && to == BoardManager.enPassantTargetSquare.Value)
        {
            int dir = movingPiece.isWhite ? 1 : -1;
            Vector2Int capturedPawnPos = new Vector2Int(to.x, to.y - dir);

            Piece capturedPawn = boardState[capturedPawnPos.x, capturedPawnPos.y];
            if (capturedPawn != null && capturedPawn.pieceType == PieceType.Pawn)
            {
                SoundManager.Instance.PlayCaptureSound();
                Destroy(capturedPawn.gameObject);
                boardState[capturedPawnPos.x, capturedPawnPos.y] = null;
            }
        }

        // Clear previous en passant target
        enPassantTargetSquare = null;

        // Set en passant target if pawn moves two squares
        if (movingPiece.pieceType == PieceType.Pawn)
        {
            int moveDistance = Mathf.Abs(to.y - from.y);
            if (moveDistance == 2)
            {
                int dir = movingPiece.isWhite ? -1 : 1;
                enPassantTargetSquare = new Vector2Int(to.x, to.y + dir);
            }
        }

        // Castling
        if (movingPiece.pieceType == PieceType.King && Mathf.Abs(to.x - from.x) == 2)
        {
            // King-side castling
            if (to.x == 6)
            {
                Piece rook = boardState[7, to.y];
                boardState[5, to.y] = rook;
                boardState[7, to.y] = null;

                rook.boardPosition = new Vector2Int(5, to.y);
                RectTransform rookRect = rook.GetComponent<RectTransform>();


                int rookTileIndex = to.y * cols + 5;
                Transform rookTileTransform = tileContainerTransform.GetChild(rookTileIndex);
                RectTransform rookTileRect = rookTileTransform.GetComponent<RectTransform>();
                rookRect.position = rookTileRect.position;

                rook.hasMoved = true;
            }
            // Queen-side castling
            else if (to.x == 2)
            {
                Piece rook = boardState[0, to.y];
                boardState[3, to.y] = rook;
                boardState[0, to.y] = null;

                rook.boardPosition = new Vector2Int(3, to.y);
                RectTransform rookRect = rook.GetComponent<RectTransform>();

                int rookTileIndex = to.y * cols + 3;
                Transform rookTileTransform = tileContainerTransform.GetChild(rookTileIndex);
                RectTransform rookTileRect = rookTileTransform.GetComponent<RectTransform>();
                rookRect.position = rookTileRect.position;

                rook.hasMoved = true;
            }
            SoundManager.Instance.PlayCastleSound();
        }

        // After moving piece, check if move caused opponent check
        bool isWhiteTurn = movingPiece.isWhite;
        bool opponentIsWhite = !isWhiteTurn;

        Vector2Int opponentKingSquare = BoardManager.FindKingSquare(opponentIsWhite);
        bool opponentInCheck = BoardManager.IsSquareAttacked(opponentKingSquare, isWhiteTurn, BoardManager.boardState);

        // Play capture sound if it was a capture and not causing check
        if (isCapture && !opponentInCheck)
        {
            if (opponentInCheck)
            {
                SoundManager.Instance.PlayCheckSound();
            }
            else
            {
                SoundManager.Instance.PlayCaptureSound();

            }
        }
        else
        {

            if (opponentInCheck)
            {
                SoundManager.Instance.PlayCheckSound();
            }
            else
            {
                SoundManager.Instance.PlayMoveSound();

            }

        }


    }

    //checks for check after move, used for promotion
    public static void CheckForCheckAfterMove(bool isWhite)
    {
        Vector2Int kingPos = FindKingSquare(!isWhite);
        bool opponentInCheck = IsSquareAttacked(kingPos, isWhite, boardState);

        if (opponentInCheck)
            SoundManager.Instance.PlayCheckSound();
        else
            SoundManager.Instance.PlayMoveSound(); 
    }



    //highlight legal moves for a piece
    public GameObject moveHighlightPrefab;
    public GameObject captureHighlightPrefab;

    private List<GameObject> currentHighlights = new List<GameObject>();
    public static bool showLegalMovesCheckboxOn = true;
    public void ShowLegalMoves(Piece piece)
    {
        if (!showLegalMovesCheckboxOn) { return; }
        ClearHighlights();
        var pseudoLegalMoves = piece.GetLegalMoves(boardState);
        var legalMoves = BoardManager.FilterMovesThatCauseCheck(piece, pseudoLegalMoves, boardState);

        foreach (var move in legalMoves)
        {
            bool isCapture = BoardManager.Instance.GetPieceAt(move) != null &&
                             BoardManager.Instance.GetPieceAt(move).isWhite != piece.isWhite;

            GameObject prefabToUse = isCapture ? captureHighlightPrefab : moveHighlightPrefab;

            GameObject highlight = Instantiate(prefabToUse, highlightContainerTransform);
            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            highlightRect.position = BoardToAnchoredPosition(move);
            if (isCapture)
            {
                highlightRect.localScale = new Vector3(1.2f, 1.2f, 1f);
                highlight.GetComponent<Image>().color = new Color32(0, 0, 0, 94);
            }


            highlightRect.position = BoardToAnchoredPosition(move);

            currentHighlights.Add(highlight);
        }

    }


    public void ClearHighlights()
    {
        foreach (var go in currentHighlights)
            Destroy(go);
        currentHighlights.Clear();
    }

    public Vector2 BoardToAnchoredPosition(Vector2Int boardPos)
    {
        int tileIndex = boardPos.y * cols + boardPos.x;

        Transform tileTransform = tileContainerTransform.GetChild(tileIndex);
        RectTransform tileRect = tileTransform.GetComponent<RectTransform>();

        return tileRect.position;
    }





    //turns
    public enum PlayerColor { White, Black }
    public PlayerColor currentTurn = PlayerColor.White;
    public static bool autoFlipBoard = false;
    public void SwitchTurn()
    {
        if (!gameStarted)
            return;
        // Switch turn first
        currentTurn = currentTurn == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;

        // Sync isWhiteTurn for move history
        isWhiteTurn = currentTurn == PlayerColor.White;

        // Determine whose king to find
        bool isWhite = currentTurn == PlayerColor.White;
        Vector2Int kingSquare = BoardManager.FindKingSquare(isWhite);

        // Check if players(my) king is in check (attacked by enemy pieces)
        bool inCheck = BoardManager.IsSquareAttacked(kingSquare, !isWhite, BoardManager.boardState);

        //check if its gameover (checkmate or stalemate)
        bool isCheckmate;
        gameOver = BoardManager.IsCheckmateOrStalemate(isWhite, out isCheckmate);

        if (gameOver)
        {
            string winner = isCheckmate ? (isWhite ? "White Won" : "Black Won") : "Draw";
            string outcome = isCheckmate ? "by checkmate!" : "";
            timerRunning = false; // Stop the timer
            SoundManager.Instance.PlayGameOverSound();
            gameOverUI.ShowGameOver(winner, outcome);
        }

        //flips board every turn
        else
        {
            if (autoFlipBoard)FlipBoard();
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


    public Transform boardContainerTransform;
    private bool isFlipped = false;
    public Transform topBar;
    public Transform bottomBar;

    public void FlipBoard()
    {
        isFlipped = !isFlipped;
        boardContainerTransform.rotation = isFlipped ? Quaternion.Euler(0, 0, 180) : Quaternion.identity;
        foreach (Transform child in pieceContainerTransform)
        {
            child.rotation = Quaternion.identity;
        }
        Vector3 temp = topBar.position;
        topBar.position = bottomBar.position;
        bottomBar.position = temp;

}

public float BoardToTileSize()
    {
        return tileContainerTransform.GetComponent<GridLayoutGroup>().cellSize.x;
    }

    // Converts mouseScreenPos (screen space), board position (Vector2Int)
    // Works even if the board is placed anywhere in the Canvas
    public Vector2Int ScreenPositionToBoardPosition(Vector2 mouseScreenPos)
    {
        Vector2 localPoint;
        RectTransform boardRect = boardContainerTransform as RectTransform;

        // Convert screen point, local point in board RectTransform
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            boardRect,
            mouseScreenPos,
            boardContainerTransform.GetComponentInParent<Canvas>().worldCamera,
            out localPoint
        );

        // BoardRect pivot = (0.5, 0.5), so localPoint is centered at (0,0)
        float tileSize = BoardToTileSize();

        float boardOriginX = -boardRect.rect.width / 2f;
        float boardOriginY = -boardRect.rect.height / 2f;

        float relativeX = localPoint.x - boardOriginX;
        float relativeY = localPoint.y - boardOriginY;

        int x = Mathf.FloorToInt(relativeX / tileSize);
        int y = Mathf.FloorToInt(relativeY / tileSize);

        // Clamp result
        x = Mathf.Clamp(x, 0, cols - 1);
        y = Mathf.Clamp(y, 0, rows - 1);

        return new Vector2Int(x, y);
    }


    //timer

    public TMP_Text whiteTimerText;
    public TMP_Text blackTimerText;

    private bool timerRunning = true;
    void Update()
    {
        if (!timerRunning) return;

        if (currentTurn == PlayerColor.White)
        {
            blackTimerText.alpha = 0.5f;
            whiteTimerText.alpha = 1.0f;
            whiteTimeRemaining -= Time.deltaTime;
            whiteTimeRemaining = Mathf.Max(whiteTimeRemaining, 0f);
        }
        else
        {   
            whiteTimerText.alpha = 0.5f;
            blackTimerText.alpha = 1.0f;
            blackTimeRemaining -= Time.deltaTime;
            blackTimeRemaining = Mathf.Max(blackTimeRemaining, 0f);
        }

        UpdateTimerUI();

        // Check for timeout
        //black wins
        if (whiteTimeRemaining <= 0f)
        {
            timerRunning = false;
            bool gameOver = true;

            if (gameOver)
            {
                string winner = "Black Won";
                string outcome = "on time!";
                SoundManager.Instance.PlayGameOverSound();
                gameOverUI.ShowGameOver(winner, outcome);
            }
        }
        //white wins
        else if (blackTimeRemaining <= 0f)
        {
            timerRunning = false;
            bool gameOver = true;

            if (gameOver)
            {
                string winner = "White Won";
                string outcome = "on time";
                SoundManager.Instance.PlayGameOverSound();
                gameOverUI.ShowGameOver(winner, outcome);
            }
        }
    }
    void UpdateTimerUI()
    {
        whiteTimerText.text = FormatTime(whiteTimeRemaining);
        blackTimerText.text = FormatTime(blackTimeRemaining);
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    //move history 
    public static List<MovePair> moveHistory = new List<MovePair>();
    public static bool isWhiteTurn = true;

    public static string BoardPositionToAlgebraic(Vector2Int pos)
    {
        char file = (char)('a' + pos.x);
        char rank = (char)('1' + pos.y);
        return $"{file}{rank}";
    }

    public static string FormatMove(Piece movingPiece, Vector2Int from, Vector2Int to, bool isCapture)
    {
        string pieceLetter = "";

        switch (movingPiece.pieceType)
        {
            case PieceType.King: pieceLetter = "K"; break;
            case PieceType.Queen: pieceLetter = "Q"; break;
            case PieceType.Rook: pieceLetter = "R"; break;
            case PieceType.Bishop: pieceLetter = "B"; break;
            case PieceType.Knight: pieceLetter = "N"; break;
            case PieceType.Pawn: pieceLetter = ""; break;
        }

        string move = "";

        if (movingPiece.pieceType == PieceType.Pawn && isCapture)
        {
            char fromFile = (char)('a' + from.x);
            move = $"{fromFile}x{BoardPositionToAlgebraic(to)}";
        }
        else if (isCapture)
        {
            move = $"{pieceLetter}x{BoardPositionToAlgebraic(to)}";
        }
        else
        {
            move = $"{pieceLetter}{BoardPositionToAlgebraic(to)}";
        }

        return move;
    }

    public static void AddMoveToHistory(string moveNotation, Sprite pieceSprite)
    {
        if (isWhiteTurn)
        {
            MovePair newPair = new MovePair();
            newPair.whiteMove = moveNotation;
            newPair.whitePieceSprite = pieceSprite;
            moveHistory.Add(newPair);
        }
        else
        {
            moveHistory[moveHistory.Count - 1].blackMove = moveNotation;
            moveHistory[moveHistory.Count - 1].blackPieceSprite = pieceSprite;
        }


        FindFirstObjectByType<MoveHistoryUI>().UpdateMoveHistoryUI();
    }

    [System.Serializable] 
    public class MovePair
    {
        public string whiteMove;
        public Sprite whitePieceSprite;

        public string blackMove;
        public Sprite blackPieceSprite;
    }


    [SerializeField] GameObject promotionChoicePanelPrefab;
    private GameObject currentPromotionPanel;

    private void ShowPromotionOptions(Piece pawn)
    {

        //switch turn again to ovverride first switch turn upon move
        SwitchTurn();
        // Clean up old panel
        if (currentPromotionPanel != null)
            Destroy(currentPromotionPanel);
        InputManager.isPromotionPanelOpen = true;
        Vector2Int boardPos = pawn.boardPosition;
        bool isWhite = pawn.isWhite;

        // Get world position of the tile where pawn was promoted
        int tileIndex = boardPos.y * 8 + boardPos.x;
        Transform tile = BoardManager.Instance.tileContainerTransform.GetChild(tileIndex);

        // Spawn UI panel at tile position
        currentPromotionPanel = Instantiate(promotionChoicePanelPrefab, boardContainerTransform);

        // Position it above the tile
        RectTransform panelRect = currentPromotionPanel.GetComponent<RectTransform>();
        RectTransform tileRect = tile.GetComponent<RectTransform>();

        if (isWhite)
        {
            panelRect.position = tileRect.position + new Vector3(0, tileRect.rect.height / 2, 0);
        } else {
            panelRect.position = tileRect.position + new Vector3(0, tileRect.rect.height + tileRect.rect.height * 3, 0);

        }

        Button[] buttons = currentPromotionPanel.GetComponentsInChildren<Button>();

        PieceType[] types = new PieceType[]
        {
            PieceType.Queen,
            PieceType.Rook,
            PieceType.Bishop,
            PieceType.Knight
        };

        for (int i = 0; i < buttons.Length && i < types.Length; i++)
        {
            Button button = buttons[i];
            PieceType type = types[i];
            PieceType newType = CustomizationSave.LoadReplacementPieceType(type, isWhite);
            string spriteName = CustomizationSave.LoadSpriteChoice(type, isWhite);
            Sprite customSprite = SpriteRegistry.GetSpriteByName(spriteName);


            Image image = button.GetComponentInChildren<Image>();
            image.sprite = customSprite;


            // For now just print, later you can apply promotion
            button.onClick.AddListener(() => Promote(pawn, newType, customSprite));
            button.onClick.AddListener(() => Destroy(currentPromotionPanel));


        }




    }

    private void Promote(Piece pawn, PieceType type, Sprite newPieceSprite)
    {
        // Auto promote to Queen (simple version)
        pawn.pieceType = type;

        // Change sprite
        Image sr = pawn.GetComponent<Image>();
        sr.sprite = newPieceSprite;
        CheckForCheckAfterMove(pawn.isWhite);
        InputManager.isPromotionPanelOpen = false;
        //switch turns after promotion
        SwitchTurn();

    }





}
