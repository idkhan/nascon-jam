using UnityEngine;

public class Card : MonoBehaviour {
    public CardData cardData;

    public void Initialize(CardData data) {
        cardData = data;
        GetComponent<SpriteRenderer>().sprite = data.image;
    }
}
