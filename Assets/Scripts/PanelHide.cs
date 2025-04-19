using UnityEngine;

public class PanelHide : MonoBehaviour
{
    RectTransform panelRect;

    void Start()
    {
        panelRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(panelRect, Input.mousePosition, Camera.main))
        {
            this.gameObject.SetActive(false);
        }
    }
    }
}
