using System;
using System.Collections.Generic;
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
    public bool isTurn;
    public Canvas liarUI;
    public Canvas detectiveUI;
    private HandManager handManager;

    [SerializeField]
    private List<GameObject> tableCards;
    public GameObject cardPrefab;
    public Transform tablePos;

    public string statement;
    private bool isTrue;
    public int damage;

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
        handManager = GameObject.FindGameObjectWithTag("HandManager").GetComponent<HandManager>();
        //Wait for lobby
        StartTurns();
    }
    
    private void StartTurns(){
        if(isLiar && isTurn){
            StartLiarTurn();
        } else {
            Debug.Log("IsTurn: " + isTurn);

            if(isTurn) {
                StartDetectiveTurn(); 
            }
        }
    }

    void Update()
    {
        liarUI.gameObject.SetActive(isLiar);
        detectiveUI.gameObject.SetActive(!isLiar);
    }

    void StartLiarTurn()
    {
        handManager.currentTurn = Turn.Liar;
    }
    public void EndLiarTurn(List<CardData> cards, bool _isTrue, string _statement, int _damage){
        isTurn = false;
        isTrue = _isTrue;
        statement = _statement;
        damage = _damage;
        int count = cards.Count;
        tableCards = new List<GameObject>();
        float spacing = 0.25f;
        for(int i = 0; i < count; i++){
            CardData card = cards[i];
            GameObject obj = Instantiate(cardPrefab, tablePos);
            obj.GetComponent<Card>().Initialize(card);
            tableCards.Add(obj);
            obj.transform.localPosition = obj.transform.localPosition + Vector3.right * spacing * (i - Mathf.Floor((count+1)/2));
            obj.transform.localPosition = obj.transform.localPosition + Vector3.forward * 0.01f * i;
        }
        
        //TEMPORARY
        isLiar = false;
        isTurn = true;
        StartTurns();
    }

    private void StartDetectiveTurn(){
        handManager.currentTurn = Turn.Detective;
    }

    public void EndDetectiveTurn(bool guess){
        foreach(GameObject obj in tableCards){
            obj.transform.localRotation = Quaternion.Euler(0,180,0);
        }
        if(guess == isTrue){
            //Detective Wins
        } else {
            //Liar Wins
        }
    }
}
