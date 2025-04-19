using TMPro;
using UnityEngine;

public class Card : MonoBehaviour {
    public CardData cardData;
    public TextMeshProUGUI cardValue;
    public void Initialize(CardData data) {
        cardData = data;
        GetComponent<SpriteRenderer>().sprite = data.image;
        cardValue.text = GetCardLabel(data.value);
    }

    string GetCardLabel(int value)
    {
        switch (value)
        {
            case 1: return "A";
            case 11: return "J";
            case 12: return "Q";
            case 13: return "K";
            default: return value.ToString();
        }
    }
}
