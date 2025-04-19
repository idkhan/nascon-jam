using UnityEngine;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour{
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private bool isHovered = false;

    public float hoverLift = 0.10f;
    public float hoverScale = 1.1f;
    public float lerpSpeed = 10f;
    bool selected = false;
    HandManager hand;
    public void setHand(HandManager newHand){
        hand = newHand;
    }

    public void SetOriginalPosition(Vector3 pos) {
        originalPosition = pos;
        originalScale = transform.localScale;
    }

    void Start() {
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;
    }

    public void OnMouseEnter()
    {
        isHovered = true;
    }

    public void OnMouseExit()
    {
        isHovered = false;
    }

    public void OnMouseDown()
    {
        Debug.Log("Clicked");
        if(hand != null){
            selected = hand.selectCard(GetComponent<Card>().cardData,!selected);
            Debug.Log(selected);
        }
    }

    void Update() {
        if (isHovered || selected) {
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
