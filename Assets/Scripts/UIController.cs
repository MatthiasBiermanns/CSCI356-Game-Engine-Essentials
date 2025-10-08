using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;

public class UIController : MonoBehaviour
{
    [SerializeField] Image settingsPopup;
    [SerializeField] AudioSource music;
    [SerializeField] Slider mouseSensitivitySlider;
    [SerializeField] Slider yawPitchSlider;
    [SerializeField] Slider volumeSlider;
    public TMP_Text levelLabel;
    public TMP_Text currentKeyText;

    [SerializeField] private Image volumeImage;
    [SerializeField] private Sprite volumeOnSprite;
    [SerializeField] private Sprite volumeOffSprite;

    [SerializeField] private Image progressFill;
    [SerializeField] private Image progressBar;

    [SerializeField] public Image helpPopup;

    NavMeshAgent agent;

    GameObject player;
    GameObject mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        // don't display the popup on start
        settingsPopup.gameObject.SetActive(false);
        helpPopup.gameObject.SetActive(false);

        // get references to the player and camera
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        volumeSlider.value = AudioListener.volume;
        mouseSensitivitySlider.value = player.GetComponent<MouseLook>().sensitivityHor;

        // set the slider value to the player's speed
        //agent = player.GetComponent<NavMeshAgent>();
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
    }

    public void OnCloseSettings()
    {
        // don't display the settings popup
        settingsPopup.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(true);

        player.GetComponent<FPSInput>().enabled = true;
        player.GetComponent<MouseLook>().enabled = true;
        mainCamera.GetComponent<MouseLook>().enabled = true;
    }

    public void OnOpenSettings()
    {
        // display the settings popup
        settingsPopup.gameObject.SetActive(true);
        progressBar.gameObject.SetActive(false);

        player.GetComponent<FPSInput>().enabled = false;
        player.GetComponent<MouseLook>().enabled = false;
        mainCamera.GetComponent<MouseLook>().enabled = false;
    }

    public void OnOpenHelp()
    {
        helpPopup.gameObject.SetActive(true);
    }
    public void OnCloseHelp()
    {
        helpPopup.gameObject.SetActive(false);
    }

    public void OnMouseSensitivityChange()
    {
        // change the mouse sensitivity
        player.GetComponent<MouseLook>().sensitivityHor = mouseSensitivitySlider.value;
        player.GetComponent<MouseLook>().sensitivityVert = mouseSensitivitySlider.value;
        mainCamera.GetComponent<MouseLook>().sensitivityHor = mouseSensitivitySlider.value;
        mainCamera.GetComponent<MouseLook>().sensitivityVert = mouseSensitivitySlider.value;
    }
    public void OnYawPitchChange()
    {
        // change the yaw/pitch setting
        //agent.speed = speedSlider.value;
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
    private IEnumerator UpdateProgressCoroutine(float fillValue, float duration = 0.5f)
    {
        float startValue = progressFill.fillAmount;
        float targetValue = startValue + fillValue;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            progressFill.fillAmount = Mathf.Lerp(startValue, Mathf.Clamp01(targetValue), t / duration);
            yield return null;
        }
        progressFill.fillAmount = Mathf.Clamp01(targetValue);
    }
}
