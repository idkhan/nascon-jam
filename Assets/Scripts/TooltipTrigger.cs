using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    [Multiline] public string content;

    // For UI elements
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipSystem.Show(content, header);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Hide();
    }

    // For 3D objects
    private void OnMouseEnter()
    {
        TooltipSystem.Show(content, header);
    }

    private void OnMouseExit()
    {
        TooltipSystem.Hide();
    }
}
