using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [System.Serializable]
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

    [System.Serializable]
    class ScoreData 
    { 
        public List<ScoreEntry> scores = new List<ScoreEntry>(); 
    }

    // Start is called before the first frame update
    void Start()
    {
        controller.currentKeyText.text = "Current Key: " + Enums.ColorToString(currentKey);
        controller.levelLabel.text = currentLevel.ToString();
        player = GameObject.FindGameObjectWithTag("Player");

        characterController = player.GetComponent<CharacterController>();

        LoadBestScores();
        if (bestScores.Count > 0)
        {
            SetHighScore(bestScores[0].score);
        }
        controller.UpdateLeaderboard(bestScores);
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

    public void LevelUp(int level)
    {
        currentLevel = level;
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
        DeleteBestScores();
    }

    public void OnStartGame()
    {
        controller.OnStartGame();

        characterController.enabled = false;
        player.transform.position = new Vector3(-10f, 1f, -3f);
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
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
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
        SaveBestScores();
    }

    public void SaveScore(string name)
    {
        AddBestScore(name, elapsedTime);
    }

    void SaveBestScores()
    {
        ScoreData data = new ScoreData();
        data.scores = bestScores;
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("BestScores", json);
        PlayerPrefs.Save();
    }

    void LoadBestScores()
    {
        if (PlayerPrefs.HasKey("BestScores"))
        {
            string json = PlayerPrefs.GetString("BestScores");
            ScoreData data = JsonUtility.FromJson<ScoreData>(json);
            bestScores = data.scores;
        }
    }

    void DeleteBestScores()
    {
        PlayerPrefs.DeleteKey("BestScores");
    }

    public void AddTimePenalty(float penalty)
    {
        elapsedTime += penalty;
        controller.UpdateTimerText(elapsedTime);
    }
}
