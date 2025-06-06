using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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

        if (context.started)
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null)
            {
                var piece = hit.collider.GetComponent<Piece>();
                if (piece != null)
                {
                    selectedPiece = piece.gameObject;
                    selectedPieceScript = piece;
                    originalPosition = selectedPiece.transform.position;
                    offset = selectedPiece.transform.position - worldPos;
                    isDragging = true;

                    legalMoves = piece.GetLegalMoves(BoardManager.boardState);
                    BoardManager.Instance.ShowLegalMoves(piece);
                }
            }
        }
        else if (context.canceled && selectedPiece != null)
        {
            isDragging = false;

            Vector2Int targetPos = new(
                Mathf.RoundToInt(worldPos.x),
                Mathf.RoundToInt(worldPos.y)
            );

            if (legalMoves.Contains(targetPos))
            {
                BoardManager.Instance.MovePiece(selectedPieceScript.boardPosition, targetPos);
            }
            else
            {
                // Snap back
                selectedPiece.transform.position = originalPosition;

                selectedPieceScript.boardPosition = new Vector2Int(
                    Mathf.RoundToInt(originalPosition.x),
                    Mathf.RoundToInt(originalPosition.y)
                );


                var pos = selectedPieceScript.boardPosition;
                BoardManager.boardState[pos.x, pos.y] = selectedPieceScript;

            }

            selectedPiece = null;
            selectedPieceScript = null;
            legalMoves.Clear();
            BoardManager.Instance.ClearHighlights();
        }
    }

    public void OnPosition(InputAction.CallbackContext context)
    {
        mouseScreenPos = context.ReadValue<Vector2>();
    }
}
