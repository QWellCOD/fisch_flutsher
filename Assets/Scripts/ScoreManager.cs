using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI References")]
    [SerializeField] private TMP_Text timeText; // Für die Zeitanzeige
    [SerializeField] private TMP_Text scoreText; // Für die Punkteanzeige
    [SerializeField] private TMP_Text highscoreText;

    [Header("Settings")]
    [SerializeField] private float scoreIncreaseInterval = 1f;
    [SerializeField] private int powerUpPoints = 5; // Punkte pro PowerUp

    private int currentTimeScore; // Zeit-basierter Score
    private int currentPointsScore; // PowerUp-basierter Score
    private int highscore;
    private float elapsedTime;
    private bool isGameActive = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

        LoadHighscore();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeUIReferences();
        ResetScore();
        isGameActive = true;
    }

    void InitializeUIReferences()
    {
        // Suche nach den korrekten UI-Elementen in der Szene
        timeText = GameObject.Find("TimeText")?.GetComponent<TMP_Text>();
        scoreText = GameObject.Find("ScoreText")?.GetComponent<TMP_Text>();
        highscoreText = GameObject.Find("HighscoreText")?.GetComponent<TMP_Text>();

        // Falls das TimeText-Element nicht existiert, aber ScoreText vorhanden ist
        if (timeText == null && scoreText != null)
        {
            Debug.LogWarning("TimeText nicht gefunden. Verwende vorhandenes ScoreText für die Zeit.");
            timeText = scoreText;
        }

        UpdateTimeDisplay();
        UpdateScoreDisplay();
        UpdateHighscoreDisplay();
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

    // PowerUps rufen diese Methode auf, wenn sie eingesammelt werden
    public void AddPowerUpPoints()
    {
        currentPointsScore += powerUpPoints;
        UpdateScoreDisplay();
        CheckHighscore();
    }

    // Allgemeine Methode zum Hinzufügen von Punkten
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
            timeText.text = $"Time: {currentTimeScore}";
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {currentPointsScore}";
    }

    private void CheckHighscore()
    {
        // Highscore = Zeit + PowerUp-Punkte
        int totalScore = currentTimeScore + currentPointsScore;

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
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
