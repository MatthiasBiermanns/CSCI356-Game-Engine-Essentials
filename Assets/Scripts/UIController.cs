using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] Image settingsPopup;
    [SerializeField] Slider speedSlider;
    public TMP_Text doorsOpenedCounter;
    public TMP_Text currentKeyTag;


    GameObject player;
    GameObject mainCamera;
  

    void Start()
    {
        settingsPopup.gameObject.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        speedSlider.value = player.GetComponent<MoveTo>().agent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCloseSettings()
    {
        // don't display the settings popup
        settingsPopup.gameObject.SetActive(false);

        // reenable player inputs
        player.GetComponent<MoveTo>().enabled = true;
        mainCamera.GetComponent<OrbitCamera>().enabled = true;
    }

    public void OnOpenSettings()
    {
        // display the settings popup
        settingsPopup.gameObject.SetActive(true);

        // disable player inputs
        player.GetComponent<MoveTo>().enabled = false;
        mainCamera.GetComponent<OrbitCamera>().enabled = false;
        
    }

    public void OnSpeedChange()
    {
        // change the player's speed
        player.GetComponent<MoveTo>().agent.speed = speedSlider.value;
    }
}
