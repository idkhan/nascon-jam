using UnityEngine;

public class Player{
    public string name;
    public int health;

    public Player(string name){
        this.name = name;
        health = 10;
    }
}

public class GameManager : MonoBehaviour
{
    private Player player1;    
    private Player player2;    

    private Player currentLiar;
    private Player currentDetective;

    private int cardCount;
    private bool isTruth;

    private void Start()
    {
        player1 = new Player("Player 1");
        player2 = new Player("Player 2");

        float coinFlip = Random.Range(0,1);
        currentLiar = coinFlip > 0.5f ? player1 : player2;
        currentDetective = coinFlip > 0.5f ? player2 : player1;

        StartLiarTurn();
    }

    void StartLiarTurn()
    {
        Debug.Log($"{currentLiar.name}'s Turn (Liar): Pick your cards and form a statement.");
        // Trigger UI for liar to pick cards and submit statement
    }

    public void SubmitLiarStatement(string statement, int cardsPlayed, bool truth)
    {
        cardCount = cardsPlayed;
        isTruth = truth;

        Debug.Log($"{currentLiar.name} says: \"{statement}\"");
        StartDetectiveTurn();
    }

    void StartDetectiveTurn()
    {
        Debug.Log($"{currentDetective.name}'s Turn (Detective): Do you believe them? (Yes/No)");
        // Trigger UI for detective to choose yes/no
    }

    public void DetectiveGuess(bool guessedTruth)
    {
        if (guessedTruth == isTruth)
        {
            currentLiar.health -= cardCount;
            Debug.Log($"{currentDetective.name} guessed correctly! {currentLiar.name} loses {cardCount} health.");
        }
        else
        {
            currentDetective.health -= cardCount;
            Debug.Log($"{currentDetective.name} guessed wrong! They lose {cardCount} health.");
        }

        CheckGameOver();
    }

    void CheckGameOver()
    {
        if (currentLiar.health <= 0)
        {
            Debug.Log($"{currentDetective.name} wins!");
            return;
        }
        if (currentDetective.health <= 0)
        {
            Debug.Log($"{currentLiar.name} wins!");
            return;
        }

        SwapRoles();
        //DISCARD CARDS TOO
        StartLiarTurn();
    }

    void SwapRoles()
    {
        var temp = currentLiar;
        currentLiar = currentDetective;
        currentDetective = temp;
    }
}
