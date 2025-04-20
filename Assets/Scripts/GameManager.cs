using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool isLiar;
    public bool isTurn;
    public int health = 10;
    public TMP_Text healthText;
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
        isLiar = false;
        isTurn = false;

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        handManager = GameObject.FindGameObjectWithTag("HandManager").GetComponent<HandManager>();
        //Wait for lobby
        //StartTurns();
    }
    
    public void StartTurns(bool Liar){
        if(Liar){
            StartLiarTurn();
        } else {
            isLiar = false;
            isTurn = false;
        }
        handManager.RoundStart();
    }

    void Update()
    {
        liarUI.gameObject.SetActive(isLiar);
        detectiveUI.gameObject.SetActive(!isLiar);
    }

    void StartLiarTurn()
    {
        isLiar = true;
        isTurn = true;
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
            obj.transform.localPosition = obj.transform.localPosition + Vector3.forward * 0.01f * i + Vector3.up * -i * 0.1f;
        }
        
    }

    private void StartDetectiveTurn(){
        isTurn = true;
        isLiar = false;
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
