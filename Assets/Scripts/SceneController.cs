using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] private UIController controller;
    [SerializeField] GameObject player;
    [SerializeField] ChallengeManager tutorialChallengeManager;

    CharacterController characterController;

    public Color currentKey = Color.None;
    public int currentLevel = 0;

    public float highScore = 0f;
    private bool timerRunning = false;
    private float elapsedTime = 0f;

    private List<ScoreEntry> bestScores = new List<ScoreEntry>();
    private int maxBestScores = 5;

    public struct ScoreEntry
    {
        public string playerName;
        public float score;
        public ScoreEntry(string name, float score)
        {
            this.playerName = name;
            this.score = score;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        controller.currentKeyText.text = "Current Key: " + Enums.ColorToString(currentKey);
        controller.levelLabel.text = currentLevel.ToString();
        player = GameObject.FindGameObjectWithTag("Player");

        characterController = player.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            elapsedTime += Time.deltaTime;
            controller.UpdateTimerText(elapsedTime);
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

    public void ResumeTimer()
    {
        timerRunning = true;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        controller.UpdateTimerText(elapsedTime);
        timerRunning = false;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void SetHighScore(float score)
    {
        highScore = score;
        controller.UpdateHighScore(highScore);
    }

    public void ResetHighScore()
    {
        highScore = 0f;
        bestScores.Clear();
        controller.ResetScores();
    }

    public void OnStartGame()
    {
        controller.OnStartGame();

        characterController.enabled = false;
        player.transform.position = new Vector3(37.37f, 1f, 8f);
        characterController.enabled = true;
        tutorialChallengeManager.enabled = true;
    }

    public void OnSkipTutorial()
    {
        controller.OnStartGame();

        // skip via invoke to not miss any activities
        tutorialChallengeManager.enabled = true;
        tutorialChallengeManager.onAllChallengesCompleted.Invoke();
        tutorialChallengeManager.enabled = false;

        characterController.enabled = false;
        player.transform.position = new Vector3(0f, 1f, 8.5f);
        characterController.enabled = true;

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

    private void AddBestScore(string name, float score)
    {
        if (score <= 0f)
            return;

        ScoreEntry entry = new ScoreEntry(name, score);
        bestScores.Add(entry);
        bestScores = bestScores.OrderBy(s => s.score).ToList();
        if (bestScores.Count > maxBestScores)
        {
            bestScores.RemoveAt(bestScores.Count - 1);
        }

        SetHighScore(bestScores[0].score);
        controller.UpdateLeaderboard(bestScores);
    }

    public void SaveScore(string name)
    {
        AddBestScore(name, elapsedTime);
    }


}
