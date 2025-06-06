using UnityEngine;
using UnityEngine.InputSystem;

public class PieceDragHandler : MonoBehaviour
{
    private Camera cam;
    private bool isDragging = false;
    private Vector3 dragOffset;

    private PlayerControls controls;
    private Vector2 mousePosition;

    void Awake()
    {
        cam = Camera.main;
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Player.Click.started += OnClickStarted;
        controls.Player.Click.canceled += OnClickCanceled;
        controls.Player.Position.performed += ctx => mousePosition = ctx.ReadValue<Vector2>();
    }

    void OnDisable()
    {
        controls.Player.Click.started -= OnClickStarted;
        controls.Player.Click.canceled -= OnClickCanceled;
        controls.Disable();
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 worldPos = cam.ScreenToWorldPoint(mousePosition);
            worldPos.z = -0.1f;
            transform.position = worldPos + dragOffset;
        }
    }

    private void OnClickStarted(InputAction.CallbackContext ctx)
    {
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePosition);
        worldPos.z = 0;

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == this.gameObject)
        {
            isDragging = true;
            dragOffset = transform.position - worldPos;
        }
    }

    private void OnClickCanceled(InputAction.CallbackContext ctx)
    {
        if (isDragging)
        {
            isDragging = false;

            Vector3 worldPos = cam.ScreenToWorldPoint(mousePosition);
            worldPos.z = 0;

            float snappedX = Mathf.Round(worldPos.x);
            float snappedY = Mathf.Round(worldPos.y);

            transform.position = new Vector3(snappedX, snappedY, -0.1f);
        }
    }
}
