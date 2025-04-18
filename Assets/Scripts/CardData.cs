using UnityEngine;

public enum Suit {
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card")]
public class CardData : ScriptableObject {
    public Suit suit;
    [Range(1,13)]public int value; //11-J, 12-Q and 13-K, 1-A
    public Sprite image;
}