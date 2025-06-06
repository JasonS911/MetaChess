using System.Collections.Generic;
using UnityEngine;
public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King }

public class Piece : MonoBehaviour
{
    public bool isWhite;
    public PieceType pieceType;
    public Vector2Int boardPosition;
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
        //TODO: implement en passant and promotion
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
        //pawns can capture diagonally
        foreach (int dx in new[] { -1, 1 })
        {
            Vector2Int diag = new Vector2Int(boardPosition.x + dx, boardPosition.y + direction);
            if (IsInBounds(diag) && board[diag.x, diag.y] != null && board[diag.x, diag.y].isWhite != isWhite)
                moves.Add(diag);
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
        //TODO: need to implement castling and check for check
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
                    moves.Add(newPos);
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
