using UnityEngine;
using UnityEngine.EventSystems;

public class HoverRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject gameController; // The GameObject that should handle the hover logic

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameController?.SendMessage("OnPanelHoverEnter", SendMessageOptions.DontRequireReceiver);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameController?.SendMessage("OnPanelHoverExit", SendMessageOptions.DontRequireReceiver);
    }
}
