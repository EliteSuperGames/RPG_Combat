using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject tooltipPrefab;

    public string tooltipMessage;

    private GameObject currentTooltip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip();
        // currentTooltip = Instantiate(tooltipPrefab, transform.position, Quaternion.identity, transform);
        // currentTooltip.GetComponentInChildren<Text>().text = tooltipMessage;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }

    private void ShowTooltip()
    {
        if (tooltipPrefab != null && currentTooltip == null)
        {
            currentTooltip = Instantiate(tooltipPrefab, transform.position, Quaternion.identity, transform);
        }
    }

    private void HideTooltip()
    {
        if (currentTooltip != null)
        {
            Destroy(currentTooltip);
            currentTooltip = null;
        }
    }
}
