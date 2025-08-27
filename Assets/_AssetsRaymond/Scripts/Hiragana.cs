using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hiragana : MonoBehaviour
{
    [Header("Hiragana Properties")]
    public string hiraganaCharacter; // The hiragana character this object represents
    public GameObject kanjiPrefab; // The kanji prefab to spawn when combining
    public int pointValue = 10; // Points awarded for combining
    
    [Header("Combination Settings")]
    public float combinationRadius = 2f; // How close hiragana need to be to combine
    public LayerMask hiraganaLayer; // Layer for other hiragana objects
    
    private bool isCombining = false;
    private GameManager gameManager; // Reference to game manager for scoring
    
    void Start()
    {
        // Find the game manager
        gameManager = FindObjectOfType<GameManager>();
        
        // Set the layer to hiragana layer if not already set
        if (gameManager != null)
        {
            hiraganaLayer = gameManager.GetHiraganaLayer();
        }
    }
    
    void Update()
    {
        if (!isCombining)
        {
            CheckForCombination();
        }
    }
    
    void CheckForCombination()
    {
        // Find nearby hiragana objects
        Collider[] nearbyHiragana = Physics.OverlapSphere(transform.position, combinationRadius, hiraganaLayer);
        
        foreach (Collider col in nearbyHiragana)
        {
            if (col.gameObject != gameObject)
            {
                Hiragana otherHiragana = col.GetComponent<Hiragana>();
                if (otherHiragana != null && !otherHiragana.isCombining)
                {
                    // Check if these two hiragana can form a valid combination
                    if (CanFormKanji(otherHiragana.hiraganaCharacter))
                    {
                        StartCombination(otherHiragana);
                        break;
                    }
                }
            }
        }
    }
    
    bool CanFormKanji(string otherHiragana)
    {
        // Define valid hiragana combinations that form kanji
        // You can expand this dictionary with more combinations
        Dictionary<string, string[]> validCombinations = new Dictionary<string, string[]>
        {
            { "ね", new string[] { "こ" } }, // ね + こ = 猫 (cat)
            { "い", new string[] { "ぬ" } }, // い + ぬ = 犬 (dog)
            { "さ", new string[] { "く" } }, // さ + く = 咲く (bloom)
            { "た", new string[] { "べ" } }, // た + べ = 食べ (eat)
            { "の", new string[] { "み" } }, // の + み = 飲み (drink)
        };
        
        // Check if this combination is valid
        if (validCombinations.ContainsKey(hiraganaCharacter))
        {
            return System.Array.Exists(validCombinations[hiraganaCharacter], x => x == otherHiragana);
        }
        
        return false;
    }
    
    void StartCombination(Hiragana otherHiragana)
    {
        isCombining = true;
        otherHiragana.isCombining = true;
        
        // Start combination animation/effect
        StartCoroutine(CombineHiragana(otherHiragana));
    }
    
    IEnumerator CombineHiragana(Hiragana otherHiragana)
    {
        // Visual feedback - make both hiragana glow or animate
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Renderer[] otherRenderers = otherHiragana.GetComponentsInChildren<Renderer>();
        
        // Flash effect
        for (int i = 0; i < 3; i++)
        {
            foreach (Renderer r in renderers)
                r.material.color = Color.yellow;
            foreach (Renderer r in otherRenderers)
                r.material.color = Color.yellow;
                
            yield return new WaitForSeconds(0.1f);
            
            foreach (Renderer r in renderers)
                r.material.color = Color.white;
            foreach (Renderer r in otherRenderers)
                r.material.color = Color.white;
                
            yield return new WaitForSeconds(0.1f);
        }
        
        // Spawn the kanji prefab at the midpoint
        Vector3 spawnPosition = (transform.position + otherHiragana.transform.position) / 2f;
        if (kanjiPrefab != null)
        {
            GameObject kanjiObject = Instantiate(kanjiPrefab, spawnPosition, Quaternion.identity);
            
            // Add to game manager's kanji list
            if (gameManager != null)
            {
                gameManager.AddKanjiToList(kanjiObject);
            }
            
            // Add some visual flair
            StartCoroutine(AnimateKanjiSpawn(kanjiObject));
        }
        
        // Award points
        if (gameManager != null)
        {
            gameManager.AddScore(pointValue);
        }
        
        // Destroy both hiragana
        Destroy(otherHiragana.gameObject);
        Destroy(gameObject);
    }
    
    IEnumerator AnimateKanjiSpawn(GameObject kanjiObject)
    {
        // Scale up animation
        Vector3 originalScale = kanjiObject.transform.localScale;
        kanjiObject.transform.localScale = Vector3.zero;
        
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            kanjiObject.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, progress);
            yield return null;
        }
        
        kanjiObject.transform.localScale = originalScale;
    }
    
    // Visualize the combination radius in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, combinationRadius);
    }
}
