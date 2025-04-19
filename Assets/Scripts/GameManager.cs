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
    public static GameManager Instance { get; private set; }

    public bool isLiar;
    public Canvas liarUI;
    public Canvas detectiveUI;


    public void Awake() //Singleton Made
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //Wait for lobby
        if(isLiar){
            StartLiarTurn();
        }
    }

    void Update()
    {
        liarUI.gameObject.SetActive(isLiar);
        detectiveUI.gameObject.SetActive(!isLiar);
    }

    void StartLiarTurn()
    {
    }
}
