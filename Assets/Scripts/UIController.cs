using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;

public class UIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SceneController sceneController;
    private GameObject player;
    private GameObject mainCamera;

    [Header("Settings")]
    [SerializeField] private Image settingsPopup;
    [SerializeField] private AudioSource music;
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Slider volumeSlider;
    
    [SerializeField] private Image volumeImage;
    [SerializeField] private Sprite volumeOnSprite;
    [SerializeField] private Sprite volumeOffSprite;

    [Header("Ingame Overlay")]

    [SerializeField] private Image progressFill;
    [SerializeField] private Image progressBar;

    [SerializeField] private Image clock;

    [SerializeField] private Button helpButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Image levelImage;

    [SerializeField] private Image doubleJumpActive;
    [SerializeField] private Image doubleJumpInactive;
    [SerializeField] private Image grapplingHookActive;
    [SerializeField] private Image grapplingHookInactive;

    public TMP_Text levelLabel;
    public TMP_Text currentKeyText;
    public TMP_Text timerText;
    public TMP_Text highScoreStartScreen;
    public TMP_Text highScoreEndScreen;
    public TMP_Text grenadeCounterText;
    public TMP_Text playerHint;

    [Header("UI Screens")]
    [SerializeField] private Image startScreen;
    [SerializeField] private Image leaderboardScreen;
    [SerializeField] private Image endScreen;

    [Header ("Help Text Customizing")]
    [SerializeField] private Image helpPopup;
    public TMP_Text[] helpTexts;
    [SerializeField] private UnityEngine.Color helpTextActiveIncompleteColor;
    [SerializeField] private UnityEngine.Color helpTextActiveCompleteColor;
    [SerializeField] private UnityEngine.Color helpTextInactiveIncompleteColor;
    [SerializeField] private UnityEngine.Color helpTextInactiveCompleteColor;

    private bool[] showHelpTexts;

    [Header("Leader Board")]
    public TMP_Text[] leaderboardNames;
    public TMP_Text[] leaderboardScores;
    public TMP_InputField playerNameInput;


    private bool showCrosshair = false;
    private bool scoreSaved = false;
    private bool fromEndScreen = false;


    // Start is called before the first frame update
    void Start()
    {
        // don't display the popup on start
        settingsPopup.gameObject.SetActive(false);
        helpPopup.gameObject.SetActive(false);
        clock.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);

        // display the start screen and hide the other screens
        startScreen.gameObject.SetActive(true);
        leaderboardScreen.gameObject.SetActive(false);
        endScreen.gameObject.SetActive(false);

        // get references to the player and camera
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        volumeSlider.value = AudioListener.volume;
        mouseSensitivitySlider.value = player.GetComponent<MouseLook>().sensitivityHor;

        // don't display the help and settings buttons and level image
        helpButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        levelImage.gameObject.SetActive(false);

        player.GetComponent<FPSInput>().enabled = false;
        player.GetComponent<MouseLook>().enabled = false;
        mainCamera.GetComponent<MouseLook>().enabled = false;
        Cursor.lockState = CursorLockMode.None;

        showHelpTexts = new bool[helpTexts.Length];
        for (int i = 0; i < showHelpTexts.Length; i++) { 
            showHelpTexts[i] = false; 
        }

        playerHint.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // display the cursor when the ESC key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // unlock and display the cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetKeyDown(KeyCode.T) && playerHint.text == "Press T to use your torch")
        {
            SetPlayerHintEnabled(false);
        }
    }

    private void OnGUI()
    {
        if (!showCrosshair)
        {
            return;
        }
        
        int size = 12;

        Camera cam = Camera.main;

        // centre of screen and caters for font size
        float posX = cam.pixelWidth / 2 - size / 4;
        float posY = cam.pixelHeight / 2 - size / 2;

        // displays "*" on screen
        GUI.Label(new Rect(posX, posY, size, size), "*");
    }

    public void OnCloseSettings()
    {
        // don't display the settings popup
        settingsPopup.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(true);

        player.GetComponent<FPSInput>().enabled = true;
        player.GetComponent<MouseLook>().enabled = true;
        mainCamera.GetComponent<MouseLook>().enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        sceneController.ResumeTimer();
        SetShowCrosshair(true);
    }

    public void OnOpenSettings()
    {
        // display the settings popup
        settingsPopup.gameObject.SetActive(true);
        progressBar.gameObject.SetActive(false);

        player.GetComponent<FPSInput>().enabled = false;
        player.GetComponent<MouseLook>().enabled = false;
        mainCamera.GetComponent<MouseLook>().enabled = false;

        sceneController.StopTimer();
        SetShowCrosshair(false);
    }

    public void OnOpenHelp()
    {
        helpPopup.gameObject.SetActive(true);

        for (int i = 0; i < helpTexts.Length; i++)
        {
            helpTexts[i].gameObject.SetActive(showHelpTexts[i]);
        }
        SetShowCrosshair(false);
    }
    public void OnCloseHelp()
    {
        helpPopup.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetShowCrosshair(true);
    }

    public void OnMouseSensitivityChange()
    {
        // change the mouse sensitivity
        player.GetComponent<MouseLook>().sensitivityHor = mouseSensitivitySlider.value;
        player.GetComponent<MouseLook>().sensitivityVert = mouseSensitivitySlider.value;
        mainCamera.GetComponent<MouseLook>().sensitivityHor = mouseSensitivitySlider.value;
        mainCamera.GetComponent<MouseLook>().sensitivityVert = mouseSensitivitySlider.value;
    }
    public void OnVolumeChange()
    {
        // change the volume
        AudioListener.volume = volumeSlider.value;
        if (AudioListener.pause)
        {
            volumeImage.sprite = volumeOffSprite;
            AudioListener.pause = false;
        }
    }

    public void Mute()
    {
        bool isMute = AudioListener.pause;
        volumeImage.sprite = isMute ? volumeOffSprite : volumeOnSprite;
        AudioListener.pause = !isMute;
    }

    public void UpdateProgress(float value)
    {
        progressFill.fillAmount = Mathf.Clamp01(value);
    }

    public void UpdateProgressSmooth(float value = 0.2f)
    {
        StartCoroutine(UpdateProgressCoroutine(value));
    }
    private IEnumerator UpdateProgressCoroutine(float targetValue, float duration = 0.5f)
    {
        float startValue = progressFill.fillAmount;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            progressFill.fillAmount = Mathf.Lerp(startValue, Mathf.Clamp01(targetValue), t / duration);
            yield return null;
        }
        progressFill.fillAmount = Mathf.Clamp01(targetValue);
    }

    private IEnumerator VanishProgressBar()
    {
        yield return new WaitForSeconds(1.0f);
        progressBar.gameObject.SetActive(false);
    }

    public void HideProgressBar()
    {
        StartCoroutine(VanishProgressBar());
    }

    public void UpdateTimerText(float time)
    {
        if (timerText != null)
            timerText.text = FormatTime(time);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    public void ShowTimer(bool show = true)
    {
        if (clock != null)
            clock.gameObject.SetActive(show);
    }

    public void OnStartGame()
    {
        startScreen.gameObject.SetActive(false);
        helpButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        levelImage.gameObject.SetActive(true);
        progressBar.gameObject.SetActive(true);

        player.GetComponent<FPSInput>().enabled = true;
        player.GetComponent<MouseLook>().enabled = true;
        mainCamera.GetComponent<MouseLook>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        SetShowCrosshair(true);

        SetPlayerHint("Press T to use your torch");
        SetPlayerHintEnabled(true);
    }

    public void UpdateHighScore(float score)
    {
        if (highScoreStartScreen == null || highScoreEndScreen == null)
            return;
        if (score <= 0f)
        {
            highScoreStartScreen.text = "--:--";
            highScoreEndScreen.text = "--:--";
            return;
        }
        highScoreStartScreen.text = FormatTime(score);
        highScoreEndScreen.text = FormatTime(score);
    }

    public void OnRestartGame()
    {
        SetShowCrosshair(false);
        Start();
    }

    public void OnShowLeaderboard()
    {
        leaderboardScreen.gameObject.SetActive(true);
        startScreen.gameObject.SetActive(false);
    }

    public void OnShowLeaderboardFromEndScreen()
    {
        fromEndScreen = true;
        leaderboardScreen.gameObject.SetActive(true);
        endScreen.gameObject.SetActive(false);
    }

    public void OnCloseLeaderboard()
    {
        leaderboardScreen.gameObject.SetActive(false);
        if (fromEndScreen)
        {
            endScreen.gameObject.SetActive(true);
            fromEndScreen = false;
            return;
        }
        startScreen.gameObject.SetActive(true);
    }

    public void UpdateGrenadeCount()
    {
        Debug.Log(mainCamera.GetComponent<Shooter>().grenades);
        grenadeCounterText.text = mainCamera.GetComponent<Shooter>().grenades.ToString();
    }

    public void SwitchDoubleJumpActive(bool active)
    {
        bool activeImageValue = true;
        bool inactiveImageValue = false;
        if (!active)
        {
            activeImageValue = false;
            inactiveImageValue = true;
        }

        doubleJumpActive.enabled = activeImageValue;
        doubleJumpInactive.enabled = inactiveImageValue;
    }

    public void SwitchGrapplingHookActive(bool active)
    {
        bool activeImageValue = true;
        bool inactiveImageValue = false;
        if (!active)
        {
            activeImageValue = false;
            inactiveImageValue = true;
        }

        grapplingHookActive.enabled = activeImageValue;
        grapplingHookInactive.enabled = inactiveImageValue;
    }

    public void UpdateLeaderboard(List<SceneController.ScoreEntry> bestScores)
    {
        for (int i = 0; i < leaderboardNames.Length; i++)
        {
            if (i < bestScores.Count)
            {
                if (!leaderboardNames[i].gameObject.activeSelf)
                    leaderboardNames[i].gameObject.SetActive(true);
                leaderboardNames[i].text = (i + 1) + ". " + bestScores[i].playerName;
                leaderboardScores[i].text = FormatTime(bestScores[i].score);
            }
            else
            {
                leaderboardNames[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnSaveScore()
    {
        if (scoreSaved) return;

        string playerName = playerNameInput.text;
        playerName = Sanitize(playerName);
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Player";
        }
        sceneController.SaveScore(playerName);
        scoreSaved = true;
    }

    private string Sanitize(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        input = input.Trim(); // remove leading and trailing whitespace
        char[] invalidChars = { '<', '>', '/', '\\', '|', ':', '"', '?', '*', '&' };
        foreach (char c in invalidChars)
        {
            input = input.Replace(c.ToString(), ""); // remove invalid characters
        }
        if (input.Length > 16)
        {
            input = input.Substring(0, 16); // limit length to 16 characters
        }
        return input;
    }

    public void OnShowEndScreen()
    {
        sceneController.StopTimer();
        endScreen.gameObject.SetActive(true);
        scoreSaved = false;
        SetShowCrosshair(false);
    }

    public void ResetScores()
    {
        UpdateLeaderboard(new List<SceneController.ScoreEntry>());
        UpdateHighScore(0f);
        scoreSaved = false;
    }

    public void UpdateHelpTextColor(int idx, HelpTextState state)
    {
        if (idx >= helpTexts.Length)
        {
            return;
        }

        switch ( state )
        {
            case HelpTextState.ActiveIncomplete:
                helpTexts[idx].color = helpTextActiveIncompleteColor;
                return;
            case HelpTextState.ActiveComplete:
                helpTexts[idx].color = helpTextActiveCompleteColor;
                return;
            case HelpTextState.InactiveIncomplete:
                helpTexts[idx].color = helpTextInactiveIncompleteColor;
                return;
            case HelpTextState.InactiveComplete:
                helpTexts[idx].color = helpTextInactiveCompleteColor;
                return;
            default:
                return;
        }
    }

    public void SetHelpText(int idx, string text)
    {
        if (idx >= helpTexts.Length)
        {
            return;
        }

        helpTexts[idx].text = (idx+1).ToString() + ". " + text;
    }

    public void SetShowHelpText(int idx, bool value)
    {
        if (idx >= showHelpTexts.Length)
        {
            return;
        }

        Debug.Log("UIController: set " + helpTexts[idx].name + " to " + value);
        showHelpTexts[idx] = value;
    }

    public void SetPlayerHint(string value)
    {
        playerHint.text = value;
    }

    public void SetPlayerHintEnabled(bool value)
    {
        playerHint.enabled = value;
    }

    public void SetShowCrosshair(bool value)
    {
        showCrosshair = value;
    }
}
