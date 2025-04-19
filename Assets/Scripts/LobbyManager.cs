using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Playroom;

#if UNITY_EDITOR
using UnityEditor;          // lets us stop Play Mode from a Quit button
#endif

public class LobbyManager : MonoBehaviour
{
    /* ────────────────────────────────────────────────────────────────────────────
       SINGLETON
    ─────────────────────────────────────────────────────────────────────────────*/
    public static LobbyManager Instance { get; private set; }

    /* ────────────────────────────────────────────────────────────────────────────
       EXISTING  UI
    ─────────────────────────────────────────────────────────────────────────────*/
    [Header("Lobby / Game Panels")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject gamePanel;

    [Header("Lobby Text")]
    [SerializeField] private TMP_Text waitingText;
    [SerializeField] private TMP_Text[] playerNameTexts;
    [SerializeField] private TMP_Text statusText;

    [Header("Ready / Start UI Elements")]
    [SerializeField] private Button readyButton;
    [SerializeField] private Button startButton;
    [SerializeField] private TMP_Text readyStatusText;

    /* ────────────────────────────────────────────────────────────────────────────
       MENU  BUTTONS
    ─────────────────────────────────────────────────────────────────────────────*/
    [Header("Menu Buttons (always visible)")]
    [SerializeField] private Button tutorialButton;      // -> SampleScene
    [SerializeField] private Button creditsButton;       // -> SampleScene
    [SerializeField] private Button volumeButton;        // mute toggle
    [SerializeField] private Button quitButton;          // new
    [SerializeField] private TMP_Text volumeButtonLabel; // txt child of VolumeButton

    /* ────────────────────────────────────────────────────────────────────────────
       STATE
    ─────────────────────────────────────────────────────────────────────────────*/
    private PlayroomKit prk;
    private readonly List<PlayroomKit.Player> players = new();

    private bool gameStarted = false;
    private bool isReady = false;
    private bool muted = false;

    /* ────────────────────────────────────────────────────────────────────────────
       UNITY LIFECYCLE
    ─────────────────────────────────────────────────────────────────────────────*/
    private void Awake()
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
        /* initial UI */
        lobbyPanel.SetActive(true);
        gamePanel.SetActive(false);

        SetupMenuButtons();

        /* Playroom */
        prk = new PlayroomKit();
        InitOptions opts = new InitOptions
        {
            maxPlayersPerRoom = 2,
            skipLobby = false,
            gameId = "LtHdOhiwxUqx1PXi0mZ0",
            discord = true,
            defaultPlayerStates = new() { { "ready", false } }
        };
        prk.InsertCoin(opts, OnGameReady);
    }

    /* ────────────────────────────────────────────────────────────────────────────
       MENU  BUTTONS
    ─────────────────────────────────────────────────────────────────────────────*/
    private void SetupMenuButtons()
    {
        /* keep buttons under root Canvas so panels don’t hide them */
        Canvas rootCanvas = GetComponentInChildren<Canvas>();
        if (rootCanvas)
        {
            ReparentToCanvas(tutorialButton, rootCanvas);
            ReparentToCanvas(creditsButton, rootCanvas);
            ReparentToCanvas(volumeButton, rootCanvas);
            ReparentToCanvas(quitButton, rootCanvas);
        }

        /* Tutorial & Credits both go to SampleScene */
        if (tutorialButton)
        {
            tutorialButton.onClick.RemoveAllListeners();
            tutorialButton.onClick.AddListener(() =>
                SceneManager.LoadScene("SampleScene"));
        }
        if (creditsButton)
        {
            creditsButton.onClick.RemoveAllListeners();
            creditsButton.onClick.AddListener(() =>
                SceneManager.LoadScene("SampleScene"));
        }

        /* Volume mute toggle */
        if (volumeButton)
        {
            volumeButton.onClick.RemoveAllListeners();
            volumeButton.onClick.AddListener(ToggleVolume);
        }

        /* Quit */
        if (quitButton)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
        }

        UpdateVolumeLabel();
    }

    private static void ReparentToCanvas(Button btn, Canvas canvas)
    {
        if (btn && btn.transform.parent != canvas.transform)
            btn.transform.SetParent(canvas.transform, false);
    }

    /* ────────────────────────────────────────────────────────────────────────────
       VOLUME
    ─────────────────────────────────────────────────────────────────────────────*/
    private void ToggleVolume()
    {
        muted = !muted;
        AudioListener.volume = muted ? 0f : 1f;
        UpdateVolumeLabel();
    }

    private void UpdateVolumeLabel()
    {
        if (volumeButtonLabel)
            volumeButtonLabel.text = muted ? "Volume: Off" : "Volume: On";
    }

    /* ────────────────────────────────────────────────────────────────────────────
       QUIT  GAME
    ─────────────────────────────────────────────────────────────────────────────*/
    private void QuitGame()
    {
        Debug.Log("Quit pressed – closing application.");

        // No LeaveRoom() in C# API; simply quitting ends the connection.

        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    /* ────────────────────────────────────────────────────────────────────────────
       PLAYROOM  FLOW  (unchanged)
    ─────────────────────────────────────────────────────────────────────────────*/
    private void OnGameReady()
    {
        prk.OnPlayerJoin(OnPlayerJoined);
        SetupNetworkButtons();

        prk.RpcRegister("PlayerReady", HandlePlayerReady);
        prk.RpcRegister("StartGame", HandleStartGame);

        StartCoroutine(CheckForGameStart());
    }

    private void SetupNetworkButtons()
    {
        readyButton.gameObject.SetActive(!prk.IsHost());
        startButton.gameObject.SetActive(false);

        if (!prk.IsHost())
        {
            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(() =>
            {
                isReady = true;
                prk.MyPlayer().SetState("ready", true);
                prk.RpcCall("PlayerReady", "", PlayroomKit.RpcMode.ALL);

                readyButton.interactable = false;
                readyStatusText.text = "You are ready!";
            });
        }
    }

    private void HandlePlayerReady(string data, string senderId)
    {
        PlayroomKit.Player p = prk.GetPlayer(senderId);
        if (p == null) return;

        readyStatusText.text = $"{p.GetProfile().name} is ready!";

        if (prk.IsHost())
        {
            bool everyoneReady = true;
            foreach (PlayroomKit.Player pl in players)
                if (pl.id != prk.MyPlayer().id && !pl.GetState<bool>("ready"))
                    everyoneReady = false;

            if (everyoneReady && players.Count >= 2)
            {
                startButton.gameObject.SetActive(true);
                startButton.onClick.RemoveAllListeners();
                startButton.onClick.AddListener(() =>
                    prk.RpcCall("StartGame", "", PlayroomKit.RpcMode.ALL));
            }
        }
    }

    private void HandleStartGame(string _, string __)
        => SceneManager.LoadScene("SampleScene");

    private void OnPlayerJoined(PlayroomKit.Player p)
    {
        if (!players.Contains(p)) players.Add(p);
        UpdatePlayerList();

        if (!prk.IsHost() && p.id == prk.MyPlayer().id && p.GetState<bool>("ready"))
        {
            isReady = true;
            readyButton.interactable = false;
            readyStatusText.text = "You are ready!";
        }
    }

    private IEnumerator CheckForGameStart()
    {
        while (!gameStarted)
        {
            if (players.Count >= 2)
            {
                EnterGamePanel();
                yield break;
            }

            waitingText.text = $"Waiting for players... ({players.Count}/2)";
            yield return new WaitForSeconds(1f);
        }
    }

    private void EnterGamePanel()
    {
        gameStarted = true;
        lobbyPanel.SetActive(false);
        gamePanel.SetActive(true);

        UpdatePlayerList();
        statusText.text = prk.IsHost()
            ? "You are the Host. Wait for the other player to be ready."
            : "Press Ready when you're prepared to start.";
    }

    private void UpdatePlayerList()
    {
        for (int i = 0; i < playerNameTexts.Length; i++)
            playerNameTexts[i].text = i < players.Count
                ? players[i].GetProfile().name
                : "Waiting...";
    }
}
