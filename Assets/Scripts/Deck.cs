using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Deck : MonoBehaviour
{
    public List<CardData> allCards;
    public List<CardData> deck;
    public List<CardData> discardedCards;
    public float cardSize = 0.001f;
    private Vector3 originalPosition;

    void Awake(){
        InitializeDeck();
        originalPosition = transform.position;
    }

    private void InitializeDeck(){
        deck = new List<CardData>(allCards);
        ShuffleDeck();
    }
    public void ShuffleDeck(){
        for (int i = 0; i < deck.Count; i++){
            CardData temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public void ResetDiscard(){
        deck.AddRange(discardedCards);
        discardedCards.RemoveRange(0,discardedCards.Count);
        UpdateSize();
    }

    public CardData DrawCard(){
        if (deck.Count == 0){
            ResetDiscard();
            if(deck.Count == 0) return null;
            return DrawCard();
        }
        CardData drawn = deck[0];
        deck.RemoveAt(0);
        UpdateSize();
        return drawn;
    }

    private void UpdateSize(){
        transform.position = originalPosition + Vector3.up * deck.Count * cardSize;
    }
    public int RemainingCards => deck.Count;
}
