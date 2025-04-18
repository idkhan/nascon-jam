// HandManager.cs
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class HandManager : MonoBehaviour {
    public GameObject cardPrefab;
    //Not using this rn, but if we want to move the hand, it'll be useful
    public Transform handArea;
    public List<CardData> startingHand;

    public float arcRadius = 5f; // Radius of the curve
    public float maxAngle = -30f; // Total arc angle (e.g. 30 degrees
    public float offset = -1f;
    public float scale = 0.1f;

    void Start() {
        foreach (var cardData in startingHand) {
            SpawnCard(cardData);
        }
        UpdateHandLayout();
    }

    void Update(){
        
    }

    void SpawnCard(CardData data) {
        GameObject card = Instantiate(cardPrefab, handArea);
        card.GetComponent<Card>().Initialize(data);
        //card.AddComponent<CardHover>();
    }

    void UpdateHandLayout() {
        int cardCount = handArea.childCount;
        if (cardCount == 0) return;

        float angleStep = (cardCount > 1) ? maxAngle / (cardCount - 1) : 0f;
        float startAngle = -maxAngle / 2f;

        for (int i = 0; i < cardCount; i++) {
            float angle = startAngle + angleStep * i;
            float rad = Mathf.Deg2Rad * angle;

            // Flip the Y to curve upward instead of downward
            float x = Mathf.Sin(rad) * arcRadius;
            float y = Mathf.Cos(rad) * arcRadius;

            Transform card = handArea.GetChild(i);

            // Apply position
            card.localPosition = new Vector3(x, y + offset, i * 0.01f);

            // Rotate to face upward arc
            card.localRotation = Quaternion.Euler(0, 0, -angle);
            card.localScale = Vector3.one * scale;

            //Update Hover (if it exists?? We probably dont need this since ill force add hover anyways)
            CardHover hover = card.GetComponent<CardHover>();
            if(hover != null){
                hover.SetOriginalPosition(card.localPosition);
            }
        }
    }



}
