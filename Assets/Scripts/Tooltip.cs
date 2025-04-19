using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public RectTransform rectTransform;

    private bool isPinned = false;
    private Vector2 fixedOffset = new Vector2(30f, -30f); // More space from mouse
    private Vector2 screenPadding = new Vector2(20f, 20f); // Prevents it going off-screen

    private void Awake()
    {
        gameObject.SetActive(false);
        rectTransform = GetComponent<RectTransform>();
    }

    public void setText(string content, string header = " ")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        contentField.text = content;

        PositionTooltip();  // Set position once
        isPinned = true;    // Freeze while over target
    }

    public void Unpin()
    {
        isPinned = false;
    }

    private void Update()
    {
        if (!isPinned)
        {
            PositionTooltip();
        }
    }

    private void PositionTooltip()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 anchoredPos = mousePos + fixedOffset;

        // Get tooltip size
        Vector2 tooltipSize = rectTransform.sizeDelta * rectTransform.lossyScale;

        // Flip if near right edge
        if (anchoredPos.x + tooltipSize.x + screenPadding.x > Screen.width)
        {
            anchoredPos.x = mousePos.x - tooltipSize.x - fixedOffset.x;
            rectTransform.pivot = new Vector2(1f, rectTransform.pivot.y);
        }
        else
        {
            rectTransform.pivot = new Vector2(0f, rectTransform.pivot.y);
        }

        // Flip if near top edge
        if (anchoredPos.y - tooltipSize.y - screenPadding.y < 0)
        {
            anchoredPos.y = mousePos.y + tooltipSize.y + Mathf.Abs(fixedOffset.y);
            rectTransform.pivot = new Vector2(rectTransform.pivot.x, 0f);
        }
        else
        {
            rectTransform.pivot = new Vector2(rectTransform.pivot.x, 1f);
        }

        transform.position = anchoredPos;
    }
}
