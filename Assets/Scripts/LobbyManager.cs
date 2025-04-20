using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Playroom;
using UnityEngine.SceneManagement;
using System;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private TMP_Text waitingText;
    [SerializeField] private TMP_Text[] playerNameTexts;
    [SerializeField] private TMP_Text statusText;

    private PlayroomKit prk;
    private List<PlayroomKit.Player> players = new List<PlayroomKit.Player>();
    private bool gameStarted = false;
    public PlayroomKit.Player Liar;
    public PlayroomKit.Player Detective;
    public GameManager gameManager;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        // Set initial UI state
        lobbyPanel.SetActive(true);

        // Initialize PlayroomKit
        prk = new PlayroomKit();

        InitOptions options = new InitOptions
        {
            maxPlayersPerRoom = 2,
            skipLobby = false,
            //discord = true,
            gameId = "LtHdOhiwxUqx1PXi0mZ0"
        };

        // Start the game room
        prk.InsertCoin(options, OnGameReady);
    }

    private void OnGameReady()
    {
        Debug.Log("Game Ready!");

        // Register player join event
        prk.OnPlayerJoin(OnPlayerJoined);

        StartCoroutine(CheckForGameStart());
    }

    private void OnPlayerJoined(PlayroomKit.Player player)
    {
        Debug.Log($"Player joined: {player.GetProfile().name}");

        // Add to player list if not already present
        if (!players.Contains(player))
        {
            players.Add(player);
        }

        // Update UI
        UpdatePlayerList();
    }

    private IEnumerator CheckForGameStart()
    {
        while (!gameStarted)
        {
            if (players.Count >= 2)
            {
                StartGame();
                break;
            }
            waitingText.text = $"Waiting for players... ({players.Count}/2)";
            yield return new WaitForSeconds(1f);
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        lobbyPanel.SetActive(false);

        // Update UI with player names
        UpdatePlayerList();

        statusText.text = "Game Started! Both players joined.";
        Debug.Log("Started Game");
        prk.SetState<bool>("started",true);
        Debug.Log("Game Started!");

        RunGame();
    }

    private void UpdatePlayerList()
    {
        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            if (i < players.Count)
            {
                playerNameTexts[i].text = players[i].GetProfile().name;
            }
            else
            {
                playerNameTexts[i].text = "Waiting...";
            }
        }
    }

    private void RunGame(){
        Liar = players[0];
        Detective = players[1];
        Debug.Log("Am I liar? " + (prk.Me() == Liar));
        gameManager.StartTurns(prk.Me() == Liar);
    }
}