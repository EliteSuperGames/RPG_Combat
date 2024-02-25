using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Can be used on any UI element to trigger when it is hovered over
/// </summary>
public class HoverListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void HoverEvent(PointerEventData eventData);

    public event HoverEvent OnPointerEnterEvent;
    public event HoverEvent OnPointerExitEvent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent?.Invoke(eventData);
    }
}
