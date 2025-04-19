using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Playroom;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private TMP_Text waitingText;
    [SerializeField] private TMP_Text[] playerNameTexts;
    [SerializeField] private TMP_Text statusText;

    private PlayroomKit prk;
    private List<PlayroomKit.Player> players = new List<PlayroomKit.Player>();
    private bool gameStarted = false;

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
        gamePanel.SetActive(false);

        // Initialize PlayroomKit
        prk = new PlayroomKit();

        InitOptions options = new InitOptions
        {
            maxPlayersPerRoom = 2,
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

    private void StartGame()
    {
        gameStarted = true;
        lobbyPanel.SetActive(false);
        gamePanel.SetActive(true);

        // Update UI with player names
        UpdatePlayerList();

        statusText.text = "Game Started! Both players joined.";
        Debug.Log("Game Started!");
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
}