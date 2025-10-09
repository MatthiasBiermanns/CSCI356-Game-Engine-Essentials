using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] UIController controller;
    public string currentKey = "None";
    public int currentLevel = 0;

    private bool timerRunning = false;
    private float elapsedTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        controller.currentKeyText.text = "Current Key: " + currentKey;
        controller.levelLabel.text = currentLevel.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            elapsedTime += Time.deltaTime;
            controller.UpdateTimerText(FormatTime(elapsedTime));
        }
    }
    public void PickUpKey(string key)
    {
        controller.currentKeyText.text = "Current Key: " + key;
    }

    public void ResetKeyText()
    {
        controller.currentKeyText.text = "Current Key: None";
    }

    public void LevelUp()
    {
        currentLevel++;
        controller.levelLabel.text = currentLevel.ToString();
    }

    public void StartTimer()
    {
        timerRunning = true;
        elapsedTime = 0f;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        controller.UpdateTimerText(FormatTime(elapsedTime));
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
