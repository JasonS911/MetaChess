using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static BoardManager;

public class InputManager : MonoBehaviour, PlayerControls.IPlayerActions
{
    private Camera cam;
    private PlayerControls controls;
    private Vector2 mouseScreenPos;

    private GameObject selectedPiece;
    private Piece selectedPieceScript;
    private Vector3 offset;
    private Vector3 originalPosition;
    private bool isDragging = false;

    private List<Vector2Int> legalMoves = new();


    //click to move
    private bool isClickSelecting = false; // For click-to-move
    private Vector2Int clickedTargetPos;
    private float clickStartTime;
    private bool clickHeld = false;
    private readonly float clickThreshold = 0.05f;



    void Awake()
    {
        cam = Camera.main;
        controls = new PlayerControls();
        controls.Player.SetCallbacks(this);
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        if (clickHeld && !isDragging && selectedPiece != null)
        {
            float heldTime = Time.time - clickStartTime;

            if (heldTime >= clickThreshold)
            {
                isDragging = true;
            }
        }

        if (isDragging && selectedPiece != null)
        {
            Vector3 worldPos = cam.ScreenToWorldPoint(mouseScreenPos);
            worldPos.z = -0.1f;

            float clampedX = Mathf.Clamp(worldPos.x + offset.x, 0, BoardManager.Instance.cols - 1);
            float clampedY = Mathf.Clamp(worldPos.y + offset.y, 0, BoardManager.Instance.rows - 1);

            selectedPiece.transform.position = new Vector3(clampedX, clampedY, -0.1f);
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        Vector3 worldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        worldPos.z = 0;
        Vector2Int boardClick = new(
            Mathf.RoundToInt(worldPos.x),
            Mathf.RoundToInt(worldPos.y)
        );

        if (context.started)
        {
            clickStartTime = Time.time;
            clickHeld = true;

            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null)
            {
                var piece = hit.collider.GetComponent<Piece>();
                if (piece != null)
                { 

                    // Only allow selecting pieces of the current player's color
                    if ((piece.isWhite && BoardManager.Instance.currentTurn != PlayerColor.White) ||
                        (!piece.isWhite && BoardManager.Instance.currentTurn != PlayerColor.Black))
                    {
                        return;
                    }

     

                    selectedPiece = piece.gameObject;
                    selectedPieceScript = piece;
                    originalPosition = BoardManager.Instance.BoardToWorldPosition(piece.boardPosition);
                    offset = selectedPiece.transform.position - worldPos;

                    //gets all legal moves for the piece and ten filters them to remove moves that would cause check
                    var pseudoLegalMoves = piece.GetLegalMoves(BoardManager.boardState);
                    legalMoves = BoardManager.FilterMovesThatCauseCheck(piece, pseudoLegalMoves, BoardManager.boardState);
                    BoardManager.Instance.ShowLegalMoves(piece);
                    isClickSelecting = true;

                    return;
                }
            }

            // If we didn't click a piece but already selected one earlier
            if (isClickSelecting && selectedPiece != null)
            {
                if (legalMoves.Contains(boardClick))
                {
                    BoardManager.Instance.MovePiece(selectedPieceScript.boardPosition, boardClick);
                    BoardManager.Instance.SwitchTurn();

                }


                ClearSelection();
            }
        }

        if (context.canceled && clickHeld)
        {
            clickHeld = false;
            float heldTime = Time.time - clickStartTime;

            if (!isDragging) return;

            isDragging = false;

            Vector2Int dropPos = new(
                Mathf.RoundToInt(worldPos.x),
                Mathf.RoundToInt(worldPos.y)
            );

            if (legalMoves.Contains(dropPos))
            {
                BoardManager.Instance.MovePiece(selectedPieceScript.boardPosition, dropPos);
                BoardManager.Instance.SwitchTurn();


            }
            else
            {
                selectedPiece.transform.position = originalPosition;
                var pos = selectedPieceScript.boardPosition;
                selectedPiece.transform.position = BoardManager.Instance.BoardToWorldPosition(pos);
                BoardManager.boardState[pos.x, pos.y] = selectedPieceScript;
            }

            ClearSelection();
        }
    }




    public void OnPosition(InputAction.CallbackContext context)
    {
        mouseScreenPos = context.ReadValue<Vector2>();
    }
    private void ClearSelection()
    {
        selectedPiece = null;
        selectedPieceScript = null;
        legalMoves.Clear();
        isClickSelecting = false;
        isDragging = false;
        clickHeld = false;
        BoardManager.Instance.ClearHighlights();
    }



}
