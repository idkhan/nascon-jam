using UnityEngine;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler{
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private bool isHovered = false;

    public float hoverLift = 0.50f;
    public float hoverScale = 1.1f;
    public float lerpSpeed = 10f;

    public void SetOriginalPosition(Vector3 pos) {
        originalPosition = pos;
        originalScale = transform.localScale;
    }

    void Start() {
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    void Update() {
        if (isHovered) {
            Vector3 targetPos = originalPosition + Vector3.up * hoverLift;
            Vector3 targetScale = originalScale * hoverScale;
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * lerpSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * lerpSpeed);
        } else {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * lerpSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * lerpSpeed);
        }
    }
}
