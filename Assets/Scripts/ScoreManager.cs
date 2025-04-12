using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

// Klasse für einen einzelnen Highscore-Eintrag
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

// Klasse für die gesamte Highscore-Liste
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

    // Highscore-Liste
    private HighScoreList highScores = new HighScoreList();

    private int currentTimeScore;
    private int currentPointsScore;
    private int highscore;
    private float elapsedTime;
    private bool isGameActive = true;

    [Header("Highscore UI References")]
    [SerializeField] private Transform highscoreEntryContainer;
    [SerializeField] private GameObject highscoreEntryPrefab;

    // UI-Cache für bessere Performance
    private Dictionary<string, TMP_Text> uiTextCache = new Dictionary<string, TMP_Text>();

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
        // UI-Cache bei jedem Szenenwechsel leeren
        uiTextCache.Clear();

        if (scene.name == gameOverSceneName)
        {
            // Game Over Scene geladen - UI mit Coroutine initialisieren
            StartCoroutine(InitializeGameOverUI());
        }
        else
        {
            // Spielszene - normale UI initialisieren
            InitializeUIReferences();
            ResetScore();
            isGameActive = true;
        }
    }

    void InitializeUIReferences()
    {
        // UI-Elemente finden und cachen
        timeText = FindAndCacheText("TimeText");
        scoreText = FindAndCacheText("ScoreText");
        highscoreText = FindAndCacheText("HighscoreText");

        // Fallback für timeText
        if (timeText == null && scoreText != null)
        {
            Debug.LogWarning("TimeText nicht gefunden. Verwende vorhandenes ScoreText für die Zeit.");
            timeText = scoreText;
        }

        UpdateTimeDisplay();
        UpdateScoreDisplay();
        UpdateHighscoreDisplay();
    }

    private IEnumerator InitializeGameOverUI()
    {
        // Warte einen Frame, damit alle GameObjects vollständig geladen sind
        yield return null;

        // UI-Elemente finden und cachen
        TMP_Text finalTimeText = FindAndCacheText("FinalTimeText");
        TMP_Text finalScoreText = FindAndCacheText("FinalScoreText");
        TMP_Text finalTotalScoreText = FindAndCacheText("FinalTotalScoreText");
        highscoreText = FindAndCacheText("HighscoreText");

        // Suche nach dem Container für die Highscore-Einträge
        highscoreEntryContainer = GameObject.Find("HighscoreEntryContainer")?.transform;

        if (highscoreEntryContainer == null)
        {
            // Versuche, Container über die Hierarchie zu finden
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

        // Zeige die Werte an
        if (finalTimeText != null)
            finalTimeText.text = $"Zeit: {FormatTime(currentTimeScore)}";

        if (finalScoreText != null)
            finalScoreText.text = $"Punkte: {currentPointsScore}";

        if (finalTotalScoreText != null)
            finalTotalScoreText.text = $"Gesamtpunktzahl: {GetTotalScore()}";

        UpdateHighscoreDisplay();

        // Highscore-Liste anzeigen, wenn Container gefunden wurde
        if (highscoreEntryContainer != null && highscoreEntryPrefab != null)
        {
            DisplayHighscores();
        }
        else
        {
            Debug.LogWarning("HighscoreEntryContainer oder Prefab nicht gefunden. Highscore-Liste kann nicht angezeigt werden.");
            DebugUIElements();
        }
    }

    // Hilfsmethode zum Finden und Cachen von TextMeshPro-Texten
    private TMP_Text FindAndCacheText(string name)
    {
        if (uiTextCache.TryGetValue(name, out TMP_Text cachedText))
            return cachedText;

        TMP_Text text = GameObject.Find(name)?.GetComponent<TMP_Text>();
        if (text != null)
            uiTextCache[name] = text;

        return text;
    }

    // Zur Fehlersuche: UI-Elemente auflisten
    private void DebugUIElements()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        Debug.Log($"Gefundene Canvases: {canvases.Length}");

        foreach (Canvas canvas in canvases)
        {
            Debug.Log($"Canvas: {canvas.name}");
            ListChildren(canvas.transform, 1);
        }
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
            timeText.text = $"Zeit: {FormatTime(currentTimeScore)}";
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
            scoreText.text = $"Punkte: {currentPointsScore}";
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

    // Methode zum Hinzufügen eines neuen Highscore-Eintrags
    public void AddHighscoreEntry()
    {
        // Neuen Eintrag erstellen
        HighScoreEntry entry = new HighScoreEntry(
            currentTimeScore,
            currentPointsScore
        );

        // Wenn die Liste bereits voll ist und der neue Score niedriger als der niedrigste ist
        if (highScores.entries.Count >= maxHighscoreEntries &&
            highScores.entries[highScores.entries.Count - 1].totalScore >= entry.totalScore)
        {
            // Nicht hinzufügen, da er nicht in die Top 20 kommt
            return;
        }

        // Eintrag hinzufügen und sortieren
        highScores.entries.Add(entry);
        SortHighscores();

        // Überschüssige Einträge entfernen
        while (highScores.entries.Count > maxHighscoreEntries)
        {
            highScores.entries.RemoveAt(highScores.entries.Count - 1);
        }

        SaveHighscores();
    }


    // Sortieren der Highscores (absteigend nach Gesamtpunktzahl)
    private void SortHighscores()
    {
        highScores.entries.Sort((a, b) => b.totalScore.CompareTo(a.totalScore));
    }

    // Anzeigen der Highscore-Liste
    private void DisplayHighscores()
    {
        if (highscoreEntryContainer == null || highscoreEntryPrefab == null)
        {
            Debug.LogError("Highscore-Container oder Prefab nicht gesetzt!");
            return;
        }

        // Bestehende Einträge löschen
        foreach (Transform child in highscoreEntryContainer)
        {
            Destroy(child.gameObject);
        }

        // Anzahl der anzuzeigenden Einträge bestimmen
        int entriesToShow = Mathf.Min(maxHighscoreEntries, highScores.entries.Count);

        // Einträge erstellen
        for (int i = 0; i < entriesToShow; i++)
        {
            HighScoreEntry entry = highScores.entries[i];

            GameObject entryGO = Instantiate(highscoreEntryPrefab, highscoreEntryContainer);

            // Rang
            UpdateEntryText(entryGO, "RankText", (i + 1).ToString());

            // Zeit
            UpdateEntryText(entryGO, "TimeText", FormatTime(entry.timeScore));

            // Punkte
            UpdateEntryText(entryGO, "PointsText", entry.pointsScore.ToString());

            // Gesamtpunktzahl
            UpdateEntryText(entryGO, "TotalScoreText", entry.totalScore.ToString());
        }
    }

    // Hilfsmethode zur Aktualisierung eines Text-Elements im Highscore-Eintrag
    private void UpdateEntryText(GameObject entry, string childName, string value)
    {
        TMP_Text textComponent = entry.transform.Find(childName)?.GetComponent<TMP_Text>();
        if (textComponent != null)
            textComponent.text = value;
    }

    // Speichern der Highscores mit PlayerPrefs als JSON
    private void SaveHighscores()
    {
        string json = JsonUtility.ToJson(highScores);
        PlayerPrefs.SetString("Highscores", json);
        PlayerPrefs.Save();
    }

    // Laden der Highscores aus PlayerPrefs
    private void LoadHighscores()
    {
        // Einzelnen Highscore für Kompatibilität laden
        highscore = PlayerPrefs.GetInt("Highscore", 0);

        // Highscore-Liste laden
        if (PlayerPrefs.HasKey("Highscores"))
        {
            string json = PlayerPrefs.GetString("Highscores");
            try
            {
                highScores = JsonUtility.FromJson<HighScoreList>(json);

                // Falls etwas mit der Liste nicht stimmt
                if (highScores == null || highScores.entries == null)
                {
                    Debug.LogWarning("Fehlerhafte Highscore-Liste geladen. Erstelle neue Liste.");
                    highScores = new HighScoreList();
                }

                // Falls mehr als maxHighscoreEntries vorhanden sind, überschüssige entfernen
                SortHighscores();
                while (highScores.entries.Count > maxHighscoreEntries)
                {
                    highScores.entries.RemoveAt(highScores.entries.Count - 1);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Fehler beim Laden der Highscores: " + e.Message);
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

        // Highscore aktualisieren und speichern
        CheckHighscore();
        AddHighscoreEntry();

        // Zur GameOver-Szene wechseln
        SceneManager.LoadScene(gameOverSceneName);
    }

    // Methoden zum Sortieren der Highscore-Liste nach verschiedenen Kriterien
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

    // UI-Referenzen extern setzen (für Kommunikation mit GameOverScene)
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
