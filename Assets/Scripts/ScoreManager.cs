using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class HighScoreEntry
{
    public int timeScore;
    public int pointsScore;
    public int totalScore;

    // Konstruktor
    public HighScoreEntry(int time, int points)
    {
        timeScore = time;
        pointsScore = points;
        totalScore = time + points;
    }
}

[System.Serializable]
public class HighScoreList
{
    public List<HighScoreEntry> entries = new List<HighScoreEntry>();
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI References")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highscoreText;

    [Header("Settings")]
    [SerializeField] private float scoreIncreaseInterval = 1f;
    [SerializeField] private int powerUpPoints = 5;
    [SerializeField] private string gameOverSceneName = "GameOverScene";
    [SerializeField] private int maxHighscoreEntries = 10;


    [Header("Highscore UI References")]
    [SerializeField] private Transform highscoreEntryContainer;
    [SerializeField] private GameObject highscoreEntryPrefab;

    private Dictionary<string, TMP_Text> uiTextCache = new Dictionary<string, TMP_Text>();
    private HighScoreList highScores = new HighScoreList();

    private int currentTimeScore;
    private int currentPointsScore;
    private int highscore;
    private float elapsedTime;
    private bool isGameActive = true;

    void Awake()
    {
        // Singleton-Pattern implementieren
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadHighscores();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        uiTextCache.Clear();

        if (scene.name == gameOverSceneName)
        {
            StartCoroutine(InitializeGameOverUI());
        }
        else
        {
            InitializeUIReferences();
            ResetScore();
            isGameActive = true;
        }
    }

    void InitializeUIReferences()
    {
        timeText = FindAndCacheText("TimeText");
        scoreText = FindAndCacheText("ScoreText");
        highscoreText = FindAndCacheText("HighscoreText");

        if (timeText == null && scoreText != null)
        {
            Debug.LogWarning("TimeText not found. Using existing ScoreText for time.");
            timeText = scoreText;
        }

        UpdateTimeDisplay();
        UpdateScoreDisplay();
        UpdateHighscoreDisplay();
    }

    private IEnumerator InitializeGameOverUI()
    {
        yield return null;

        TMP_Text finalTimeText = FindAndCacheText("FinalTimeText");
        TMP_Text finalScoreText = FindAndCacheText("FinalScoreText");
        TMP_Text finalTotalScoreText = FindAndCacheText("FinalTotalScoreText");
        highscoreText = FindAndCacheText("HighscoreText");

        highscoreEntryContainer = GameObject.Find("HighscoreEntryContainer")?.transform;

        if (highscoreEntryContainer == null)
        {
            Transform scrollView = GameObject.Find("HighscoreScrollView")?.transform;
            if (scrollView != null)
            {
                Transform viewport = scrollView.Find("Viewport");
                if (viewport != null)
                {
                    highscoreEntryContainer = viewport.Find("Content");
                }
            }
        }

        if (finalTimeText != null)
            finalTimeText.text = $"Time: {FormatTime(currentTimeScore)}";

        if (finalScoreText != null)
            finalScoreText.text = $"Points: {currentPointsScore}";

        if (finalTotalScoreText != null)
            finalTotalScoreText.text = $"Score: {GetTotalScore()}";

        UpdateHighscoreDisplay();

        if (highscoreEntryContainer != null && highscoreEntryPrefab != null)
        {
            DisplayHighscores();
        }
        else
        {
            Debug.LogWarning("HighscoreEntryContainer or Prefab not found. Highscore list cannot be displayed.");
        }
    }

    private TMP_Text FindAndCacheText(string name)
    {
        if (uiTextCache.TryGetValue(name, out TMP_Text cachedText))
            return cachedText;

        TMP_Text text = GameObject.Find(name)?.GetComponent<TMP_Text>();
        if (text != null)
            uiTextCache[name] = text;

        return text;
    }

    private void ListChildren(Transform parent, int depth)
    {
        string indent = new string(' ', depth * 2);
        foreach (Transform child in parent)
        {
            Debug.Log($"{indent}- {child.name}");
            ListChildren(child, depth + 1);
        }
    }

    private string FormatTime(int seconds)
    {
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;
        return $"{minutes:00}:{remainingSeconds:00}";
    }

    void Update()
    {
        if (!isGameActive) return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= scoreIncreaseInterval)
        {
            currentTimeScore += (int)(elapsedTime / scoreIncreaseInterval);
            elapsedTime %= scoreIncreaseInterval;

            UpdateTimeDisplay();
            CheckHighscore();
        }
    }

    public void AddPowerUpPoints()
    {
        currentPointsScore += powerUpPoints;
        UpdateScoreDisplay();
        CheckHighscore();
    }

    public void AddScore(int points)
    {
        currentPointsScore += points;
        UpdateScoreDisplay();
        CheckHighscore();
    }

    public void ResetScore()
    {
        currentTimeScore = 0;
        currentPointsScore = 0;
        elapsedTime = 0f;
        UpdateTimeDisplay();
        UpdateScoreDisplay();
    }

    private void UpdateTimeDisplay()
    {
        if (timeText != null)
            timeText.text = $"Time: {FormatTime(currentTimeScore)}";
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
            scoreText.text = $"Points: {currentPointsScore}";
    }

    private void CheckHighscore()
    {
        int totalScore = GetTotalScore();

        if (totalScore > highscore)
        {
            highscore = totalScore;
            SaveHighscore();
            UpdateHighscoreDisplay();
        }
    }

    private void UpdateHighscoreDisplay()
    {
        if (highscoreText != null)
            highscoreText.text = $"Highscore: {highscore}";
    }

    public void AddHighscoreEntry()
    {
        HighScoreEntry entry = new HighScoreEntry(
            currentTimeScore,
            currentPointsScore
        );

        if (highScores.entries.Count >= maxHighscoreEntries &&
            highScores.entries[highScores.entries.Count - 1].totalScore >= entry.totalScore)
        {
            return;
        }

        highScores.entries.Add(entry);
        SortHighscores();

        while (highScores.entries.Count > maxHighscoreEntries)
        {
            highScores.entries.RemoveAt(highScores.entries.Count - 1);
        }

        SaveHighscores();
    }


    private void SortHighscores()
    {
        highScores.entries.Sort((a, b) => b.totalScore.CompareTo(a.totalScore));
    }

    private void DisplayHighscores()
    {
        if (highscoreEntryContainer == null || highscoreEntryPrefab == null)
        {
            Debug.LogError("Highscore container or prefab not set!");
            return;
        }

        foreach (Transform child in highscoreEntryContainer)
        {
            Destroy(child.gameObject);
        }

        int entriesToShow = Mathf.Min(maxHighscoreEntries, highScores.entries.Count);

        for (int i = 0; i < entriesToShow; i++)
        {
            HighScoreEntry entry = highScores.entries[i];

            GameObject entryGO = Instantiate(highscoreEntryPrefab, highscoreEntryContainer);

            UpdateEntryText(entryGO, "RankText", (i + 1).ToString());

            UpdateEntryText(entryGO, "TimeText", FormatTime(entry.timeScore));

            UpdateEntryText(entryGO, "PointsText", entry.pointsScore.ToString());

            UpdateEntryText(entryGO, "TotalScoreText", entry.totalScore.ToString());
        }
    }

    private void UpdateEntryText(GameObject entry, string childName, string value)
    {
        TMP_Text textComponent = entry.transform.Find(childName)?.GetComponent<TMP_Text>();
        if (textComponent != null)
            textComponent.text = value;
    }

    private void SaveHighscores()
    {
        string json = JsonUtility.ToJson(highScores);
        PlayerPrefs.SetString("Highscores", json);
        PlayerPrefs.Save();
    }

    private void LoadHighscores()
    {
        highscore = PlayerPrefs.GetInt("Highscore", 0);

        if (PlayerPrefs.HasKey("Highscores"))
        {
            string json = PlayerPrefs.GetString("Highscores");
            try
            {
                highScores = JsonUtility.FromJson<HighScoreList>(json);

                if (highScores == null || highScores.entries == null)
                {
                    Debug.LogWarning("Invalid highscore list loaded. Creating a new list.");
                    highScores = new HighScoreList();
                }

                SortHighscores();
                while (highScores.entries.Count > maxHighscoreEntries)
                {
                    highScores.entries.RemoveAt(highScores.entries.Count - 1);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading highscores: " + e.Message);
                highScores = new HighScoreList();
            }
        }
        else
        {
            highScores = new HighScoreList();
        }
    }

    private void LoadHighscore()
    {
        highscore = PlayerPrefs.GetInt("Highscore", 0);
    }

    private void SaveHighscore()
    {
        PlayerPrefs.SetInt("Highscore", highscore);
        PlayerPrefs.Save();
    }

    public int GetTotalScore()
    {
        return currentTimeScore + currentPointsScore;
    }

    public void GameOver()
    {
        isGameActive = false;

        CheckHighscore();
        AddHighscoreEntry();

        SceneManager.LoadScene(gameOverSceneName);
    }

    public void SortByTotalScore(bool ascending = false)
    {
        highScores.entries.Sort((a, b) => ascending ?
            a.totalScore.CompareTo(b.totalScore) :
            b.totalScore.CompareTo(a.totalScore));

        DisplayHighscores();
    }

    public void SortByTimeScore(bool ascending = false)
    {
        highScores.entries.Sort((a, b) => ascending ?
            a.timeScore.CompareTo(b.timeScore) :
            b.timeScore.CompareTo(a.timeScore));

        DisplayHighscores();
    }

    public void SortByPointsScore(bool ascending = false)
    {
        highScores.entries.Sort((a, b) => ascending ?
            a.pointsScore.CompareTo(b.pointsScore) :
            b.pointsScore.CompareTo(a.pointsScore));

        DisplayHighscores();
    }

    // ui references for highscore entry container and prefab to comunicate with other scenes
    public void SetHighscoreUIReferences(Transform container, GameObject prefab)
    {
        if (container != null)
            highscoreEntryContainer = container;

        if (prefab != null)
            highscoreEntryPrefab = prefab;

        if (highscoreEntryContainer != null && highscoreEntryPrefab != null)
            DisplayHighscores();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
