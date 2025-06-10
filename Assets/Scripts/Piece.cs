using System.Collections.Generic;
using UnityEngine;
public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King }

public class Piece : MonoBehaviour
{
    public bool isWhite;
    public PieceType pieceType;
    public bool hasMoved = false;


    public Vector2Int boardPosition;


    private SpriteRenderer spriteRenderer;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //checks if piece can attack a square
    public bool CanAttackSquare(Vector2Int targetSquare, Piece[,] board)
    {
        // Pawn attacks diagonally only
        if (pieceType == PieceType.Pawn)
        {
            int dir = isWhite ? 1 : -1;
            foreach (int dx in new[] { -1, 1 })
            {
                Vector2Int diag = new Vector2Int(boardPosition.x + dx, boardPosition.y + dir);
                if (IsInBounds(diag) && diag == targetSquare)
                    return true;
            }

            return false;
        }

        // Knight moves in "L" shape
        if (pieceType == PieceType.Knight)
        {
            int[] dx = { -2, -1, 1, 2, 2, 1, -1, -2 };
            int[] dy = { 1, 2, 2, 1, -1, -2, -2, -1 };
            for (int i = 0; i < dx.Length; i++)
            {
                Vector2Int newPos = new Vector2Int(boardPosition.x + dx[i], boardPosition.y + dy[i]);
                if (IsInBounds(newPos) && newPos == targetSquare)
                    return true;
            }

            return false;
        }

        // King moves one square in any direction
        if (pieceType == PieceType.King)
        {
            int[] dx = { -1, 0, 1 };
            int[] dy = { -1, 0, 1 };
            for (int i = 0; i < dx.Length; i++)
            {
                for (int j = 0; j < dy.Length; j++)
                {
                    if (dx[i] == 0 && dy[j] == 0) continue;
                    Vector2Int newPos = new Vector2Int(boardPosition.x + dx[i], boardPosition.y + dy[j]);
                    if (IsInBounds(newPos) && newPos == targetSquare)
                        return true;
                }
            }

            return false;
        }

        // Sliding pieces, Bishop, Rook, Queen
        Vector2Int direction = new Vector2Int(
            Mathf.Clamp(targetSquare.x - boardPosition.x, -1, 1),
            Mathf.Clamp(targetSquare.y - boardPosition.y, -1, 1)
        );

        // If direction is zero, cannot attack
        if (direction == Vector2Int.zero)
            return false;

        // Only allow valid directions per piece type
        if (pieceType == PieceType.Bishop && Mathf.Abs(direction.x) != Mathf.Abs(direction.y))
            return false;

        if (pieceType == PieceType.Rook && direction.x != 0 && direction.y != 0)
            return false;

        // Queen allows both, no filter needed

        //Walk the ray
        Vector2Int current = boardPosition + direction;
        while (IsInBounds(current))
        {
            if (current == targetSquare)
                return true;

            if (board[current.x, current.y] != null)
                break; // blocked

            current += direction;
        }

        return false;
    }


    public List<Vector2Int> GetLegalMoves(Piece[,] boardState)
    {

        switch (pieceType)
        {
            case PieceType.Pawn:
                return GetLegalMovesPawn(boardState);
            case PieceType.Knight:
                return GetLegalMovesKnight(boardState);
            case PieceType.Bishop:
                return GetLegalMovesBishop(boardState);
            case PieceType.Rook:
                return GetLegalMovesRook(boardState);
            case PieceType.Queen:
                return GetLegalMovesQueen(boardState);
            case PieceType.King:
                return GetLegalMovesKing(boardState);
            // add others later
            default:
                return new List<Vector2Int>();
        }
    }
    private List<Vector2Int> GetLegalMovesPawn(Piece[,] board)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        int direction = isWhite ? 1 : -1; //if it is white, move up; if black, move down
        int startRow = isWhite ? 1 : 6; //if it is white, start at row 1; if black, start at row 6 (first row is index 0, last is 7)

        //pawns can only move forward one square
        Vector2Int oneForward = new Vector2Int(boardPosition.x, boardPosition.y + direction);
        if (IsInBounds(oneForward) && board[oneForward.x, oneForward.y] == null)
            moves.Add(oneForward);
        //starting row pawns can move two squares forward
        Vector2Int twoForward = new Vector2Int(boardPosition.x, boardPosition.y + 2 * direction);
        if (boardPosition.y == startRow && board[oneForward.x, oneForward.y] == null && board[twoForward.x, twoForward.y] == null)
            moves.Add(twoForward);
        // Pawns can capture diagonally
        foreach (int dx in new[] { -1, 1 })
        {
            Vector2Int diag = new Vector2Int(boardPosition.x + dx, boardPosition.y + direction);
            // Normal capture
            if (IsInBounds(diag) && board[diag.x, diag.y] != null && board[diag.x, diag.y].isWhite != isWhite)
            {
                moves.Add(diag);
            }
            // En passant capture
            else if (IsInBounds(diag) && BoardManager.enPassantTargetSquare.HasValue && BoardManager.enPassantTargetSquare.Value == diag)
            {
                moves.Add(diag);
            }
        }

        return moves;
    }
    private List<Vector2Int> GetLegalMovesKnight(Piece[,] board)
    {
        //knight moves in an "L" shape: two squares in one direction and then one square perpendicular, or vice versa
        List<Vector2Int> moves = new List<Vector2Int>();
        int[] dx = { -2, -1, 1, 2, 2, 1, -1, -2 };
        int[] dy = { 1, 2, 2, 1, -1, -2, -2, -1 };
        for (int i = 0; i < dx.Length; i++)
        {
            Vector2Int newPos = new Vector2Int(boardPosition.x + dx[i], boardPosition.y + dy[i]);
            if (IsInBounds(newPos) && (board[newPos.x, newPos.y] == null || board[newPos.x, newPos.y].isWhite != isWhite))
            {
                moves.Add(newPos);
            }
        }
        return moves;
    }

    private List<Vector2Int> GetLegalMovesBishop(Piece[,] board)
    {
        //bishops move diagonally any number of squares
        List<Vector2Int> moves = new List<Vector2Int>();
        int[] directions = { -1, 1 };
        foreach (int dx in directions)
        {
            foreach (int dy in directions)
            {
                for (int step = 1; step < 8; step++)
                {
                    Vector2Int newPos = new Vector2Int(boardPosition.x + dx * step, boardPosition.y + dy * step);
                    if (!IsInBounds(newPos)) break;
                    if (board[newPos.x, newPos.y] == null)
                    {
                        moves.Add(newPos);
                    }
                    else
                    {
                        if (board[newPos.x, newPos.y].isWhite != isWhite)
                            moves.Add(newPos);
                        break;
                    }
                }
            }
        }
        return moves;
    }

    private List<Vector2Int> GetLegalMovesRook(Piece[,] board)
    {
        //rooks move horizontally or vertically any number of squares
        List<Vector2Int> moves = new List<Vector2Int>();
        int[] directions = { -1, 1 };
        foreach (int dx in directions)
        {
            for (int step = 1; step < 8; step++)
            {
                Vector2Int newPos = new Vector2Int(boardPosition.x + dx * step, boardPosition.y);
                if (!IsInBounds(newPos)) break;
                if (board[newPos.x, newPos.y] == null)
                {
                    moves.Add(newPos);
                }
                else
                {
                    if (board[newPos.x, newPos.y].isWhite != isWhite)
                        moves.Add(newPos);
                    break;
                }
            }
        }
        foreach (int dy in directions)
        {
            for (int step = 1; step < 8; step++)
            {
                Vector2Int newPos = new Vector2Int(boardPosition.x, boardPosition.y + dy * step);
                if (!IsInBounds(newPos)) break;
                if (board[newPos.x, newPos.y] == null)
                {
                    moves.Add(newPos);
                }
                else
                {
                    if (board[newPos.x, newPos.y].isWhite != isWhite)
                        moves.Add(newPos);
                    break;
                }
            }
        }
        return moves;
    }

    private List<Vector2Int> GetLegalMovesQueen(Piece[,] board)
    {
        //queen combines the moves of a rook and a bishop
        List<Vector2Int> moves = new List<Vector2Int>();
        moves.AddRange(GetLegalMovesBishop(board));
        moves.AddRange(GetLegalMovesRook(board));
        return moves;
    }

    private List<Vector2Int> GetLegalMovesKing(Piece[,] board)
    {

        //king can move one square in any direction
        
        List<Vector2Int> moves = new List<Vector2Int>();
        int[] dx = { -1, 0, 1 };
        int[] dy = { -1, 0, 1 };

        for (int i = 0; i < dx.Length; i++)
        {
            for (int j = 0; j < dy.Length; j++)
            {
                if (dx[i] == 0 && dy[j] == 0) continue; // skip the case where both are zero
                Vector2Int newPos = new Vector2Int(boardPosition.x + dx[i], boardPosition.y + dy[j]);
                if (IsInBounds(newPos) && (board[newPos.x, newPos.y] == null || board[newPos.x, newPos.y].isWhite != isWhite))
                {
                    //Check if moving king to this square would be attacked
                    if (!BoardManager.IsSquareAttacked(newPos, !isWhite, board))
                    {
                        moves.Add(newPos);
                    }
                }
            }
        }

        // Castling
        if (!hasMoved && !BoardManager.IsSquareAttacked(boardPosition, !isWhite, board))
        {
            // King-side castling
            Vector2Int rookPosKingside = new Vector2Int(7, boardPosition.y);
            Piece rookKingside = board[rookPosKingside.x, rookPosKingside.y];

            if (rookKingside != null && rookKingside.pieceType == PieceType.Rook && !rookKingside.hasMoved)
            {
                // Squares between king and rook must be empty
                if (board[5, boardPosition.y] == null && board[6, boardPosition.y] == null)
                {
                    // Squares the king passes through must not be attacked
                    if (!BoardManager.IsSquareAttacked(new Vector2Int(5, boardPosition.y), !isWhite, board) &&
                        !BoardManager.IsSquareAttacked(new Vector2Int(6, boardPosition.y), !isWhite, board))
                    {
                        // Add castling move (king moves two squares right)
                        moves.Add(new Vector2Int(6, boardPosition.y));
                    }
                }
            }

            // Queen-side castling
            Vector2Int rookPosQueenside = new Vector2Int(0, boardPosition.y);
            Piece rookQueenside = board[rookPosQueenside.x, rookPosQueenside.y];

            if (rookQueenside != null && rookQueenside.pieceType == PieceType.Rook && !rookQueenside.hasMoved)
            {
                // Squares between king and rook must be empty
                if (board[1, boardPosition.y] == null && board[2, boardPosition.y] == null && board[3, boardPosition.y] == null)
                {
                    // Squares the king passes through must not be attacked
                    if (!BoardManager.IsSquareAttacked(new Vector2Int(3, boardPosition.y), !isWhite, board) &&
                        !BoardManager.IsSquareAttacked(new Vector2Int(2, boardPosition.y), !isWhite, board))
                    {
                        // Add castling move (king moves two squares left)
                        moves.Add(new Vector2Int(2, boardPosition.y));
                    }
                }
            }
        }


        return moves;
    }

    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8;
    }




}
