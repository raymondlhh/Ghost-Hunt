using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float gameDuration = 300f; // 5 minutes in seconds
    public int targetScore = 1000; // Target score to win
    
    [Header("Layer Settings")]
    public LayerMask hiraganaLayer; // Layer for hiragana objects
    
    [Header("UI References")]
    public Text timerText;
    public Text scoreText;
    public Text gameOverText;
    public GameObject gameOverPanel;
    public Button restartButton;
    
    [Header("Game State")]
    public bool isGameActive = false;
    public bool isGameOver = false;
    
    // Private variables
    private float currentTime;
    private int currentScore;
    private List<GameObject> spawnedKanji = new List<GameObject>();
    
    void Start()
    {
        InitializeGame();
        SetupUI();
    }
    
    void Update()
    {
        if (isGameActive && !isGameOver)
        {
            UpdateTimer();
            CheckGameEnd();
        }
    }
    
    void InitializeGame()
    {
        currentTime = gameDuration;
        currentScore = 0;
        isGameActive = true;
        isGameOver = false;
        
        // Hide game over UI
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
    
    void SetupUI()
    {
        // Setup restart button
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        UpdateUI();
    }
    
    void UpdateTimer()
    {
        currentTime -= Time.deltaTime;
        UpdateUI();
        
        if (currentTime <= 0)
        {
            currentTime = 0;
            EndGame();
        }
    }
    
    void UpdateUI()
    {
        // Update timer display
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        
        // Update score display
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();
        
        // Visual feedback for scoring
        StartCoroutine(ScorePopup(points));
    }
    
    IEnumerator ScorePopup(int points)
    {
        // Create a temporary score popup
        GameObject scorePopup = new GameObject("ScorePopup");
        Text popupText = scorePopup.AddComponent<Text>();
        
        // Set up the popup text
        popupText.text = "+" + points.ToString();
        popupText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        popupText.fontSize = 24;
        popupText.color = Color.green;
        
        // Position the popup
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            scorePopup.transform.SetParent(canvas.transform, false);
            scorePopup.transform.position = Input.mousePosition + Vector3.up * 50f;
        }
        
        // Animate the popup
        float duration = 1f;
        float elapsed = 0f;
        Vector3 startPos = scorePopup.transform.position;
        Vector3 endPos = startPos + Vector3.up * 100f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            
            scorePopup.transform.position = Vector3.Lerp(startPos, endPos, progress);
            popupText.color = new Color(popupText.color.r, popupText.color.g, popupText.color.b, 1f - progress);
            
            yield return null;
        }
        
        Destroy(scorePopup);
    }
    
    void CheckGameEnd()
    {
        if (currentScore >= targetScore)
        {
            EndGame(true); // Win
        }
    }
    
    void EndGame(bool isWin = false)
    {
        isGameActive = false;
        isGameOver = true;
        
        // Show game over UI
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (gameOverText != null)
        {
            if (isWin)
            {
                gameOverText.text = "Congratulations! You've mastered KanjiCraft!\nFinal Score: " + currentScore;
                gameOverText.color = Color.green;
            }
            else
            {
                gameOverText.text = "Time's up! Your final score: " + currentScore + "\nTry to beat " + targetScore + " points next time!";
                gameOverText.color = Color.white;
            }
        }
        
        // Display all created kanji
        ShowCreatedKanji();
    }
    
    void ShowCreatedKanji()
    {
        Debug.Log("=== Your Kanji Creations ===");
        foreach (GameObject kanji in spawnedKanji)
        {
            if (kanji != null)
            {
                Debug.Log("Created: " + kanji.name);
            }
        }
        Debug.Log("Total Kanji Created: " + spawnedKanji.Count);
    }
    
    public void AddKanjiToList(GameObject kanji)
    {
        if (kanji != null)
        {
            spawnedKanji.Add(kanji);
        }
    }
    
    public void RestartGame()
    {
        // Clear existing kanji
        foreach (GameObject kanji in spawnedKanji)
        {
            if (kanji != null)
            {
                Destroy(kanji);
            }
        }
        spawnedKanji.Clear();
        
        // Reset game state
        InitializeGame();
    }
    
    // Public getter for hiragana layer
    public LayerMask GetHiraganaLayer()
    {
        return hiraganaLayer;
    }
}
