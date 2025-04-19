/*******************************************************
 *  MenuButtons.cs
 *  ― adds “Tutorial”, “Credits”, and “Volume” buttons
 *  that look/behave exactly like the existing lobby
 *  buttons, but without touching LobbyManager.
 *
 *  HOW TO USE
 *  ----------
 *  1.  Duplicate any existing button in the Canvas
 *      (Ready / Start) three times and rename them
 *      TutorialButton, CreditsButton, VolumeButton.
 *  2.  Drop this script on the Canvas (or any empty
 *      GameObject) and drag the three buttons + the
 *      TextMeshPro label for Volume into the Inspector.
 *  3.  Make sure *SampleScene* and *Credits* are listed
 *      under File ▸ Build Settings ▸ Scenes In Build.
 ********************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuButtons : MonoBehaviour
{
    [Header("Buttons (wire these in Inspector)")]
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button volumeButton;

    [Header("Optional UI")]
    [SerializeField] private TMP_Text volumeButtonLabel;   // Text(TMP) under VolumeButton
    [SerializeField] private GameObject volumePanel;       // drag a slider/panel here if you have one

    private bool isMuted = false;

    private const string TUTORIAL_SCENE = "SampleScene";   // <── your game scene
    private const string CREDITS_SCENE = "SampleScene";       // change if needed

    private void Awake()
    {
        // Listeners wired exactly like LobbyManager does
        tutorialButton?.onClick.AddListener(OnTutorialClicked);
        creditsButton?.onClick.AddListener(OnCreditsClicked);
        volumeButton?.onClick.AddListener(OnVolumeClicked);
    }

    /*************  Button callbacks  *************/

    /// <summary>
    /// Loads the SampleScene to act as a tutorial/restart.
    /// </summary>
    private void OnTutorialClicked()
    {
        SceneManager.LoadScene(TUTORIAL_SCENE);
    }

    /// <summary>
    /// Loads a dedicated credits scene.
    /// </summary>
    private void OnCreditsClicked()
    {
        SceneManager.LoadScene(CREDITS_SCENE);
    }

    /// <summary>
    /// Either toggles mute or shows a volume panel (if one is provided).
    /// </summary>
    private void OnVolumeClicked()
    {
        if (volumePanel == null)   // simple mute / un‑mute
        {
            isMuted = !isMuted;
            AudioListener.volume = isMuted ? 0f : 1f;

            if (volumeButtonLabel != null)
                volumeButtonLabel.text = isMuted ? "Volume: Off" : "Volume: On";
        }
        else                        // open / close a custom panel
        {
            volumePanel.SetActive(!volumePanel.activeSelf);
        }
    }
}
