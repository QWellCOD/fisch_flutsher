using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highscoreText;

    [Header("Settings")]
    [SerializeField] private float scoreIncreaseInterval = 1f;

    private int currentScore;
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
        scoreText = GameObject.Find("ScoreText")?.GetComponent<TMP_Text>();
        highscoreText = GameObject.Find("HighscoreText")?.GetComponent<TMP_Text>();
        UpdateHighscoreDisplay();
    }

    void Update()
    {
        if (!isGameActive) return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= scoreIncreaseInterval)
        {
            currentScore += (int)(elapsedTime / scoreIncreaseInterval);
            elapsedTime %= scoreIncreaseInterval;

            UpdateScoreDisplay();
            CheckHighscore();
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();
        CheckHighscore();
    }

    public void ResetScore()
    {
        currentScore = 0;
        elapsedTime = 0f;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {currentScore}";
    }

    private void CheckHighscore()
    {
        if (currentScore > highscore)
        {
            highscore = currentScore;
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

    public void GameOver()
    {
        isGameActive = false;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
