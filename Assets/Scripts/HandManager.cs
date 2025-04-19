using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Turn{
    Liar,
    Detective
}

public class HandManager : MonoBehaviour {
    public GameObject cardPrefab;
    //Not using this rn, but if we want to move the hand, it'll be useful
    public Transform handArea;
    public List<CardData> hand;

    [SerializeField]
    private Turn currentTurn;

    public float arcRadius = 5f; // Radius of the curve
    public float maxAngle = 10f; // Total arc angle in degrees
    public float offset = -5f;
    public float scale = 0.2f;

    bool handShown = false;

    [SerializeField]
    float handBounds = 0.3f;

    private Vector3 originalPosition;
    private Vector3 originalRotation;
    [SerializeField]
    float yMove = 0.1f;

    [SerializeField]
    float xRotate = 0.1f;
    [SerializeField]
    float lerpSpeed = 5f;

    private Deck deck;
    
    [SerializeField]
    private List<CardData> selectedCards;
    private bool allowToggle;

    [Header("Liar UI")]
    public Button liarPlay;
    public CanvasRenderer liarSubmitPanel;
    public TMP_Text liarText;
    private int pickedCard;

    void Start() {
        liarPlay.gameObject.SetActive(false);
        liarSubmitPanel.gameObject.SetActive(false);

        deck = GameObject.FindGameObjectWithTag("Deck").GetComponent<Deck>();
        hand.Add(deck.DrawCard());
        hand.Add(deck.DrawCard());
        hand.Add(deck.DrawCard());
        hand.Add(deck.DrawCard());
        hand.Add(deck.DrawCard());
        foreach (var cardData in hand) {
            SpawnCard(cardData);
        }
        UpdateHandLayout();
        SetBasePosition();
    }
    
    void Update(){
        Vector3 mousePos = Input.mousePosition;
        handShown = mousePos.y / Screen.height < handBounds ? true : false; //If mouse is in the lowerr area
        ShowHand();
        if(currentTurn == Turn.Liar){
            allowToggle = true;
        } else {
            allowToggle = false;
        }
        LiarTurn();
    }

    void SetBasePosition(){
        originalPosition = transform.position;
        originalRotation = transform.rotation.eulerAngles;
    }

    void SpawnCard(CardData data) {
        GameObject card = Instantiate(cardPrefab, handArea);
        card.GetComponent<Card>().Initialize(data);
        CardHover hover = card.GetComponent<CardHover>();
        if(hover == null){
            card.AddComponent<CardHover>(); //Force adding hover to all cards inhand
        }
        hover = card.GetComponent<CardHover>();
        hover.setHand(this);
    }

    public void Draw(int amount){
        for(int i = 0; i < amount; i++){
            CardData newCard = deck.DrawCard();
            hand.Add(newCard);
            SpawnCard(newCard);
        }
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
            //if(hover != null){
            hover.SetOriginalPosition(card.localPosition);
            //}
        }
    }

    void ShowHand(){
        if(!handShown){
            Vector3 targetPos = originalPosition + Vector3.up * -yMove;
            Vector3 targetRotation = originalRotation + Vector3.right * xRotate;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * lerpSpeed);
            transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, targetRotation,Time.deltaTime * lerpSpeed));
        } else {
            transform.position = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * lerpSpeed);
            transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, originalRotation,Time.deltaTime * lerpSpeed));

        }
    }

    void LiarTurn(){
        if(selectedCards.Count >= 2){
            liarPlay.gameObject.SetActive(true);
        } else {
            liarPlay.gameObject.SetActive(false);
        }

        if(liarSubmitPanel.gameObject.activeSelf){
            allowToggle = false;
        }
    }

    public bool selectCard(CardData card, bool add){
        if(!allowToggle){
            Debug.Log("Not allowed");
            return false;
        }
        if(add){
            selectedCards.Add(card);
            return true;
        } else {
            selectedCards.Remove(card);
            return false;
        }
    }
    public void setCard(int number){
        //14 - Spades, 15 - Clubs, 16-Hearts, 17-Diamonds
        pickedCard = number;
        String text = selectedCards.Count.ToString() + " " + GetCardLabel(pickedCard) + "s";
        liarText.text = "I have " + text;
    }
    public void playCards(){
        Debug.Log("PLAYED");
        liarSubmitPanel.gameObject.SetActive(true);
        liarText.text = "I have " + selectedCards.Count.ToString();
    }

    public void HidePanel(){
        liarSubmitPanel.gameObject.SetActive(false);
    }

    string GetCardLabel(int value)
    {
        switch (value)
        {
            case 0: return "";
            case 1: return "A";
            case 2: return "Two";
            case 3: return "Three";
            case 4: return "Four";
            case 5: return "Five";
            case 6: return "Six";
            case 7: return "Seven";
            case 8: return "Eight";
            case 9: return "Nine";
            case 10: return "Ten";
            case 11: return "Jack";
            case 12: return "Queen";
            case 13: return "King";
            case 14: return "Spade";
            case 15: return "Club";
            case 16: return "Heart";
            case 17: return "Diamond";
            default: return value.ToString();
        }
    }

}
