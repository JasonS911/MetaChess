using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King, 
    Scout, //capture backwards, move diagnolly 
    Dragonfly, // captures diaganol and one ahead of diaganol, also captures backwards 1 diagnaol
    Sumo, //can capture 3 x 2 recntangle in front and one behind
    Ninja, //can capture 2 diaganol squares like bishop. Can also jump
    Pegasus, //Move all directions like queen (only 1 squares) but also jump like knight. (Two squares might be too broken)
    } //ADD NEW CUSTOM PIECES HERE 

public class Piece : MonoBehaviour
{
    public bool isWhite;
    public PieceType pieceType;
    public bool hasMoved = false;
    public PieceType originalType;

    public Vector2Int boardPosition;


    private Image pieceImage;
    void Awake()
    {
        pieceImage = GetComponent<Image>();
    }

    //checks if piece can attack a square
    //ADD NEW CUSTOM PIECE ATTACKS HERE
    public bool CanAttackSquare(Vector2Int targetSquare, Piece[,] board)
    {
        //Pegasus - can move like queen (only 3 squares) but also jump like knight
        if (pieceType == PieceType.Pegasus)
        {
            Vector2Int[] directions = new Vector2Int[]
            {
                new(1, 0),   // Right
                new(-1, 0),  // Left
                new(0, 1),   // Up
                new(0, -1),  // Down
                new(1, 1),   // Up-Right
                new(-1, 1),  // Up-Left
                new(1, -1),  // Down-Right
                new(-1, -1), // Down-Left
            };
            foreach (var dir in directions)
            {
                for(int dist = 1; dist <= 1; dist++)
                {
                    Vector2Int newPos = boardPosition + dir * dist;
                    if (IsInBounds(newPos))
                    {
                        if (newPos == targetSquare)
                            return true;
                        if (board[newPos.x, newPos.y] != null)
                            break; // blocked by another piece
                    }
                }

            }

            //knight jump moves
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
        //Ninja - can capture 2 diagonal squares like bishop. Can also jump
        if (pieceType == PieceType.Ninja)
        {
            // Check diagonal captures
            int[] directions = { -1, 1 };
            foreach (int dx in directions)
            {
                foreach (int dy in directions)
                {
                    Vector2Int diag = new Vector2Int(boardPosition.x + dx, boardPosition.y + dy);
                    if (IsInBounds(diag) && diag == targetSquare)
                        return true;
                }
            }
            // Check jump capture
            int jumpDistance = 2; // Ninja can jump over one square
            foreach (int dx in directions)
            {
                foreach (int dy in directions)
                {
                    Vector2Int jumpSquare = new Vector2Int(boardPosition.x + dx * jumpDistance, boardPosition.y + dy * jumpDistance);
                    if (IsInBounds(jumpSquare) && jumpSquare == targetSquare)
                        return true;
                }
            }
            return false;
        }

        //Sumo can capture 3(horizontal) x 2(vertical) rectangle in front and one behind
        if (pieceType == PieceType.Sumo)
        {
            int dir = isWhite ? 1 : -1;
            int[] xDirections = { -1, 0, 1 }; // left, center, right

            // Check the 3x2 rectangle in front
            foreach (int dx in xDirections)
            {
                for (int dy = 1; dy <= 2; dy++)
                {

                    Vector2Int squareToCheck = new Vector2Int(boardPosition.x + dx, boardPosition.y + dir * dy);
                    if (!IsInBounds(squareToCheck)) continue;
                    //block the second row of the rectangle if there is a piece in the way
                    if (dy == 2 && board[squareToCheck.x, squareToCheck.y - 1 * dir] != null)
                        break;
                    if (IsInBounds(squareToCheck) && squareToCheck == targetSquare)
                        return true;
                }
            }

            // Check the square behind
            Vector2Int squareBehind = new Vector2Int(boardPosition.x, boardPosition.y - dir);
            if (IsInBounds(squareBehind) && squareBehind == targetSquare)
                return true;
            return false;
        }

        //Dragonfly - can capture diagonally and one square ahead of diagonal, also captures backwards
        if (pieceType == PieceType.Dragonfly)
        {
            int dir = isWhite ? 1 : -1;
            // Check diagonal captures
            //maybe needed but might be too broken
            //foreach (int dx in new[] { -1, 1 })
            //{
            //    Vector2Int diag = new Vector2Int(boardPosition.x + dx, boardPosition.y + dir);
            //    if (IsInBounds(diag) && diag == targetSquare)
            //        return true;
            //}
            // Check one square ahead of diagonal
            foreach (int dx in new[] { -1, 1 })
            {
                Vector2Int diagPlusOne = new Vector2Int(boardPosition.x + dx, boardPosition.y + 2 * dir);
                if (IsInBounds(diagPlusOne) && diagPlusOne == targetSquare)
                    return true; 
            }
            // Check one square left or right of diagonal
            foreach (int dx in new[] { -2, 2 })
            {
                Vector2Int diagPlusOneLR = new Vector2Int(boardPosition.x + dx, boardPosition.y + dir);
                if (IsInBounds(diagPlusOneLR) && diagPlusOneLR == targetSquare)
                    return true;
            }
            // Check diagonal backward capture
            foreach (int dx in new[] { -1, 1 })
            {
                Vector2Int diagBack = new Vector2Int(boardPosition.x + dx, boardPosition.y - dir);
                if (IsInBounds(diagBack) && diagBack == targetSquare)
                    return true;
            }


            return false;
        }

        //Scout - can capture backwards
        if (pieceType == PieceType.Scout)
        {
            int dir = isWhite ? 1 : -1;
            Vector2Int diag = new Vector2Int(boardPosition.x, boardPosition.y + -dir);
            if (IsInBounds(diag) && diag == targetSquare)
                return true;


            return false;
        }

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


    public List<Vector2Int> GetLegalMoves(Piece[,] boardState) //ADD NEW LEGAL MOVES FOR CUSTOM PIECES HERE
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
            case PieceType.Scout:
                return GetLegalMovesScout(boardState);
            case PieceType.Dragonfly:
                return GetLegalMovesDragonfly(boardState);
            case PieceType.Sumo:
                return GetLegalMovesSumo(boardState);   
            case PieceType.Ninja:   
                return GetLegalMovesNinja(boardState);
            case PieceType.Pegasus:
                return GetLegalMovesPegasus(boardState);
            //// add others later
            default:
                return new List<Vector2Int>();
        }
    }

    //ALL MOVES FOR EACH PIECE TYPE
    private List<Vector2Int> GetLegalMovesPegasus(Piece[,] board)
    {
        //Pegasus can move like queen (only 3 squares) but also jump like knight
        List<Vector2Int> moves = new List<Vector2Int>();
        Vector2Int[] directions = new Vector2Int[]
        {
            new(1, 0),   // Right
            new(-1, 0),  // Left
            new(0, 1),   // Up
            new(0, -1),  // Down
            new(1, 1),   // Up-Right
            new(-1, 1),  // Up-Left
            new(1, -1),  // Down-Right
            new(-1, -1), // Down-Left
        };
        foreach (var dir in directions)
        {
            for(int dist = 1; dist <= 1; dist++)
            {
                Vector2Int newPos = boardPosition + dir * dist;
                if (IsInBounds(newPos))
                {
                    if (board[newPos.x, newPos.y] == null || board[newPos.x, newPos.y].isWhite != isWhite)
                    {
                        moves.Add(newPos);
                    }
                    if (board[newPos.x, newPos.y] != null)
                        break; // blocked by another piece
                }
            }
        }
        //knight jump moves
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

    private List<Vector2Int> GetLegalMovesNinja(Piece[,] board)
    {
        //Ninja can capture 2 diagonal squares like bishop. Can also jump
        List<Vector2Int> moves = new List<Vector2Int>();
        int[] directions = { -1, 1 };
        // Check diagonal captures
        foreach (int dx in directions)
        {
            foreach (int dy in directions)
            {
                Vector2Int diag = new Vector2Int(boardPosition.x + dx, boardPosition.y + dy);
                if (IsInBounds(diag) && (board[diag.x, diag.y] == null || board[diag.x, diag.y].isWhite != isWhite))
                    moves.Add(diag);
            }
        }
        // Check jump capture
        int jumpDistance = 2; // Ninja can jump over one square
        foreach (int dx in directions)
        {
            foreach (int dy in directions)
            {
                Vector2Int jumpSquare = new Vector2Int(boardPosition.x + dx * jumpDistance, boardPosition.y + dy * jumpDistance);
                if (IsInBounds(jumpSquare) && (board[jumpSquare.x, jumpSquare.y] == null || board[jumpSquare.x, jumpSquare.y].isWhite != isWhite))
                    moves.Add(jumpSquare);
            }
        }
        return moves;
    }

    private List<Vector2Int> GetLegalMovesSumo(Piece[,] board)
    {
        // Sumo can capture 3x2 rectangle in front and one behind
        List<Vector2Int> moves = new List<Vector2Int>();
        int dir = isWhite ? 1 : -1;
        int[] xDirections = { -1, 0, 1 }; // left, center, right
        // Check the 3x2 rectangle in front
        foreach (int dx in xDirections)
        {
            for (int dy = 1; dy <= 2; dy++)
            {
                Vector2Int newPos = new Vector2Int(boardPosition.x + dx, boardPosition.y + dir * dy);
                if (!IsInBounds(newPos)) continue;
                //block the second row of the rectangle if there is a piece in the way. 
                if (dy == 2 && board[newPos.x, newPos.y - 1 * dir] != null)
                    break;
                if (IsInBounds(newPos) && (board[newPos.x, newPos.y] == null || board[newPos.x, newPos.y].isWhite != isWhite))
                    moves.Add(newPos);
            }
        }
        // Check the square behind
        Vector2Int squareBehind = new Vector2Int(boardPosition.x, boardPosition.y - dir);
        if (IsInBounds(squareBehind) && (board[squareBehind.x, squareBehind.y] == null || board[squareBehind.x, squareBehind.y].isWhite != isWhite))
            moves.Add(squareBehind);
        return moves;
    }
    private List<Vector2Int> GetLegalMovesDragonfly(Piece[,] board)
    {
        //dragonfly can capture diagonally and one square ahead of diagonal, also captures backwards
        List<Vector2Int> moves = new List<Vector2Int>();
        int dir = isWhite ? 1 : -1;
        // Check diagonal captures (maybe needed, but might be too broken)
        //foreach (int dx in new[] { -1, 1 })
        //{
        //    Vector2Int diag = new Vector2Int(boardPosition.x + dx, boardPosition.y + dir);
        //    if (!IsInBounds(diag)) continue;
        //    if (IsInBounds(diag) && board[diag.x, diag.y] == null || board[diag.x, diag.y].isWhite != isWhite)
        //        moves.Add(diag);
        //}
        // Check one square ahead of diagonal
        foreach (int dx in new[] { -1, 1 })
        {
            Vector2Int diagPlusOne = new Vector2Int(boardPosition.x + dx, boardPosition.y + 2 * dir);
            if (!IsInBounds(diagPlusOne)) continue;
            if (IsInBounds(diagPlusOne) && board[diagPlusOne.x, diagPlusOne.y] == null || board[diagPlusOne.x, diagPlusOne.y].isWhite != isWhite)
                moves.Add(diagPlusOne);
        }
        // Check one square left or right of diagonal
        foreach (int dx in new[] { -2, 2 })
        {
            Vector2Int diagPlusOneLR = new Vector2Int(boardPosition.x + dx, boardPosition.y + dir);
            if (!IsInBounds(diagPlusOneLR)) continue;
            if (IsInBounds(diagPlusOneLR) && board[diagPlusOneLR.x, diagPlusOneLR.y] == null || board[diagPlusOneLR.x, diagPlusOneLR.y].isWhite != isWhite)
                moves.Add(diagPlusOneLR);
        }
        // Check diagonal backward capture
        foreach (int dx in new[] { -1, 1 })
        {
            Vector2Int diagBack = new Vector2Int(boardPosition.x + dx, boardPosition.y - dir);
            if (!IsInBounds(diagBack)) continue;
            if (IsInBounds(diagBack) && board[diagBack.x, diagBack.y] == null || board[diagBack.x, diagBack.y].isWhite != isWhite)
                moves.Add(diagBack);
        }

        return moves;

    }
    private List<Vector2Int> GetLegalMovesScout(Piece[,] board)
    {
        //scout can move diagonally any number of squares, but can only capture backwards
        List<Vector2Int> moves = new List<Vector2Int>();
        int yDirection = isWhite ? 1 : -1;
        int[] directions = { -1, 1 };
        foreach (int dx in directions)
        {
            Vector2Int newPos = new Vector2Int(boardPosition.x + dx , boardPosition.y + yDirection);
            if (!IsInBounds(newPos)) continue;
            if (board[newPos.x, newPos.y] == null)
            {
                moves.Add(newPos);
            } 
        
        }

      

        //can only capture backward.
        //The move has to be in the board and there has to be a piece behind the scout to capture. They cannot be the same color.
        Vector2Int backwardCapture = new Vector2Int(boardPosition.x, boardPosition.y + -yDirection);
        if (IsInBounds(backwardCapture) && board[backwardCapture.x, backwardCapture.y] != null && board[backwardCapture.x, backwardCapture.y].isWhite != isWhite)
        {
            moves.Add(backwardCapture);
        }
        return moves;
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

    //ADD NEW LEGAL MOVES FOR CUSTOM PIECES ABOVE HERE

    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8;
    }

    //public Sprite GetSprite()
    //{
    //    switch (pieceType)
    //    {
    //        case PieceType.King: return isWhite ? BoardManager.Instance.MetaChess_King : BoardManager.Instance.MetaChess_King_Black;
    //        case PieceType.Queen: return isWhite ? BoardManager.Instance.MetaChess_Queen : BoardManager.Instance.MetaChess_Queen_Black;
    //        case PieceType.Rook: return isWhite ? BoardManager.Instance.MetaChess_Rook : BoardManager.Instance.MetaChess_Rook_Black;
    //        case PieceType.Bishop: return isWhite ? BoardManager.Instance.MetaChess_Bishop : BoardManager.Instance.MetaChess_Bishop_Black;
    //        case PieceType.Knight: return isWhite ? BoardManager.Instance.MetaChess_Knight : BoardManager.Instance.MetaChess_Knight_Black;
    //        case PieceType.Pawn: return isWhite ? BoardManager.Instance.MetaChess_Pawn : BoardManager.Instance.MetaChess_Pawn_Black;
    //    }

    //    return null;
    //}



}
