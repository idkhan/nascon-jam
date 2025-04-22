using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public string statementText;
    private bool isTrue;
    public int damage;

    public LobbyManager lobby;

    [Header("Liar UI")]
    public Button liarPlay;
    public CanvasRenderer liarSubmitPanel;
    public TMP_Text liarText;
    public Button sendButton;
    public TMP_Text DamageCount;
    public bool allowToggle;

    [Header("Detective UI")]
    public CanvasRenderer detectivePanel;
    public TMP_Text statement;


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
        //StartTurns();
    }
    
    public void StartTurns(bool Liar){
        isLiar = Liar;
        isTurn = Liar;
        Debug.Log("Turn: " + isTurn + " Liar: " + isLiar);
    }

    public void SetText(string text, int dam){
        liarText.text = "I have " + text;
        sendButton.interactable = true;
        damage = dam;
        DamageCount.text = damage.ToString() + " Damage";
    }

    void Update()
    {
        setUIElements();
        allowToggle = isTurn && isLiar && !liarSubmitPanel.gameObject.activeSelf;
    }

    public void playCards(){
        Debug.Log("PLAYED");
        liarSubmitPanel.gameObject.SetActive(true);
        statementText = "I have " + handManager.selectedCards.Count.ToString();
        liarText.text = statementText;
    }

    public void HidePanel(){
        liarSubmitPanel.gameObject.SetActive(false);
    }

    void setUIElements(){
        //Liar UI
        liarUI.gameObject.SetActive(isLiar);
        liarPlay.gameObject.SetActive(isLiar && isTurn && handManager.selectedCards.Count >=2); //Play card button

        //Detective UI
        detectiveUI.gameObject.SetActive(!isLiar);
        statement.text = statementText;
        statement.gameObject.SetActive(isTurn); //Lie Statement
        detectivePanel.gameObject.SetActive(isTurn); //Picker
    }

    public void EndLiarTurn(List<CardData> cards, bool _isTrue, int _damage){
        isTurn = false;
        isTrue = _isTrue;
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
        liarSubmitPanel.gameObject.SetActive(false);
        nextTurn();
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

    public void DetectiveGuess(bool guess){
        EndDetectiveTurn(guess);
    }

    public void nextTurn(){
        lobby.nextTurn(isTurn);
    }
}
