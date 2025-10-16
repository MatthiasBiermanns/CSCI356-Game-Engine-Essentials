using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] UIController controller;
    [SerializeField] GameObject player;
    public Color currentKey = Color.None;
    public int currentLevel = 0;

    public float highScore = 0f;
    private bool timerRunning = false;
    private float elapsedTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        controller.currentKeyText.text = "Current Key: " + Enums.ColorToString(currentKey);
        controller.levelLabel.text = currentLevel.ToString();
        player = GameObject.FindGameObjectWithTag("Player");
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
    public void PickUpKey(Color key)
    {
        controller.currentKeyText.text = "Current Key: " + Enums.ColorToString(key);
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
        timerRunning = false;
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

    public void SetHighScore(float score)
    {
        highScore = score;
        controller.UpdateHighScore(FormatTime(highScore));
    }

    public void ResetHighScore()
    {
        highScore = 0f;
        controller.UpdateHighScore("--:--");
    }

    public void OnStartGame()
    {
        controller.OnStartGame();

        CharacterController cc = player.GetComponent<CharacterController>();
        cc.enabled = false;
        player.transform.position = new Vector3(-10f, 1f, -3f);
        cc.enabled = true;
    }

    public void OnSkipTutorial()
    {
        controller.OnStartGame();
        LevelUp();
        controller.HideProgressBar();
        controller.ShowTimer(true);
        
        CharacterController cc = player.GetComponent<CharacterController>();
        cc.enabled = false;
        player.transform.position = new Vector3(0f, 1f, 8.5f);
        cc.enabled = true;

        StartTimer();
    }

    public void OnRestartGame()
    {
        controller.OnRestartGame();
        currentLevel = 0;
        controller.levelLabel.text = currentLevel.ToString();

        ResetTimer();
        ResetKeyText();
    }
}
