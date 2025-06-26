using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static BoardManager;

public class InputManager : MonoBehaviour, PlayerControls.IPlayerActions
{
    private PlayerControls controls;
    private Vector2 mouseScreenPos;

    private GameObject selectedPiece;
    private Piece selectedPieceScript;
    private Vector2 originalPosition;
    private Vector2 offset;
    private bool isDragging = false;

    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;

    private List<Vector2Int> legalMoves = new();

    // click to move
    private bool isClickSelecting = false;
    private float clickStartTime;
    private bool clickHeld = false;
    private readonly float clickThreshold = 0.05f;


    public static bool isPromotionPanelOpen = false;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.SetCallbacks(this);
        raycaster = FindFirstObjectByType<GraphicRaycaster>();
        eventSystem = EventSystem.current;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void LateUpdate()
    {
        if (isPromotionPanelOpen) return; 
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
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                BoardManager.Instance.pieceContainerTransform as RectTransform,
                mouseScreenPos,
                BoardManager.Instance.boardContainerTransform.GetComponentInParent<Canvas>().worldCamera,
                out Vector2 localPoint);

            selectedPiece.GetComponent<RectTransform>().anchoredPosition = localPoint + offset;
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (isPromotionPanelOpen) return;

        Vector2Int boardClick = BoardManager.Instance.ScreenPositionToBoardPosition(mouseScreenPos);

        if (context.started)
        {
            clickStartTime = Time.time;
            clickHeld = true;

            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = mouseScreenPos
            };

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            foreach (var result in results)
            {
                Piece piece = result.gameObject.GetComponent<Piece>();
                if (piece != null)
                {
                    // Only allow selecting pieces of current turn
                    if ((piece.isWhite && BoardManager.Instance.currentTurn != PlayerColor.White) ||
                        (!piece.isWhite && BoardManager.Instance.currentTurn != PlayerColor.Black))
                    {
                        return;
                    }

                    selectedPiece = piece.gameObject;
                    selectedPieceScript = piece;

                    // Store original position
                    originalPosition = selectedPiece.GetComponent<RectTransform>().anchoredPosition;

                    // Calculate offset
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        BoardManager.Instance.pieceContainerTransform as RectTransform,
                        mouseScreenPos,
                        BoardManager.Instance.boardContainerTransform.GetComponentInParent<Canvas>().worldCamera,
                        out Vector2 localPoint);

                    offset = selectedPiece.GetComponent<RectTransform>().anchoredPosition - localPoint;

                    // Compute legal moves
                    var pseudoLegalMoves = piece.GetLegalMoves(BoardManager.boardState);
                    legalMoves = BoardManager.FilterMovesThatCauseCheck(piece, pseudoLegalMoves, BoardManager.boardState);
                    BoardManager.Instance.ShowLegalMoves(piece);
                    isClickSelecting = true;

                    return;
                }
            }

            // If clicked empty square but already had a piece selected
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

            if (!isDragging) return;

            isDragging = false;

            Vector2Int dropPos = BoardManager.Instance.ScreenPositionToBoardPosition(mouseScreenPos);

            if (legalMoves.Contains(dropPos))
            {
                BoardManager.Instance.MovePiece(selectedPieceScript.boardPosition, dropPos);
                BoardManager.Instance.SwitchTurn();
            }
            else
            {
                // Snap piece back
                selectedPiece.GetComponent<RectTransform>().anchoredPosition = originalPosition;
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
