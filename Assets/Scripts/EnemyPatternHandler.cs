using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum PatternType { A, B, C }
public enum EnemyRank { Low, Medium, High }

public class EnemyPatternHandler : MonoBehaviour
{
    public List<PatternType> patternSequence = new List<PatternType>();
    private int currentIndex = 0;

    public Image[] iconSlots;
    public Sprite[] iconSprites;

    public float revealDistance = 5f;
    private Transform player;

    private Canvas canvas;
    private EnemyController enemyController;

    public EnemyRank rank = EnemyRank.Low;

    public GameObject potionPrefab;
    public GameObject totemPrefab;

    public Transform lootParent;

    void Start()
    {
        if (lootParent == null)
        {
            GameObject lootGO = GameObject.Find("LootItems");
            if (lootGO != null)
            {
                lootParent = lootGO.transform;
            }
            else
            {
                Debug.LogWarning("LootItems GameObject not found in scene.");
            }
        }

        player = GameObject.FindWithTag("Player").transform;
        canvas = GetComponentInChildren<Canvas>();
        enemyController = GetComponent<EnemyController>();

        GenerateRandomPattern();
        UpdateIcons();
        canvas.enabled = false;

        // Adjust height based on enemy rank
        float baseHeight = 2f;
        switch (rank)
        {
            case EnemyRank.Low: baseHeight = 2f; break;
            case EnemyRank.Medium: baseHeight = 2.5f; break;
            case EnemyRank.High: baseHeight = 3f; break;
        }

        canvas.transform.localPosition = new Vector3(0, baseHeight, 0);
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        canvas.enabled = dist <= revealDistance;
        AvoidOverlap();
    }

    void GenerateRandomPattern()
    {
        int patternCount = 1;
        switch (rank)
        {
            case EnemyRank.Low: patternCount = 1; break;
            case EnemyRank.Medium: patternCount = Random.Range(2, 4); break;
            case EnemyRank.High: patternCount = 3; break;
        }

        patternSequence.Clear();
        List<PatternType> used = new List<PatternType>();

        while (patternSequence.Count < patternCount)
        {
            PatternType rand = (PatternType)Random.Range(0, iconSprites.Length);
            int sameCount = patternSequence.FindAll(p => p == rand).Count;
            if (sameCount >= 2) continue;
            patternSequence.Add(rand);
        }
    }

    void UpdateIcons()
    {
        for (int i = 0; i < iconSlots.Length; i++)
        {
            if (i < patternSequence.Count)
            {
                iconSlots[i].sprite = iconSprites[(int)patternSequence[i]];
                iconSlots[i].color = Color.white;
                iconSlots[i].enabled = true;
            }
            else
            {
                iconSlots[i].enabled = false;
            }
        }
    }

    public void ReceiveInput(PatternType input)
    {
        if (input == patternSequence[currentIndex])
        {
            iconSlots[currentIndex].color = Color.gray;
            currentIndex++;

            if (currentIndex >= patternSequence.Count)
            {
                DropLoot();
                gameObject.SetActive(false); // Object pooling reuse
            }
        }
        else
        {
            currentIndex = 0;
            UpdateIcons();
        }
    }

    // Reset pattern and state
    public void InitializePattern()
    {
        currentIndex = 0;
        GenerateRandomPattern();
        UpdateIcons();
    }

    void DropLoot()
    {
        // Add loot drop logic based on enemy rank
        float dropChance;
        GameObject drop;

        switch (rank)
        {
            case EnemyRank.Low:
                dropChance = Random.value;
                if (dropChance < 0.4f)
                {
                    drop = Instantiate(potionPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                    if (lootParent != null)
                        drop.transform.SetParent(lootParent);
                }
                break;

            case EnemyRank.Medium:
                float roll = Random.value;

                if (roll < 0.6f)
                {
                    drop = Instantiate(potionPrefab, transform.position, Quaternion.identity);
                    if (lootParent != null)
                        drop.transform.SetParent(lootParent);
                }
                else if (roll < 0.8f)
                {
                    drop = Instantiate(totemPrefab, transform.position + Vector3.right * 0.5f, Quaternion.identity);
                    if (lootParent != null)
                        drop.transform.SetParent(lootParent);
                }
                break;

            case EnemyRank.High:
                roll = Random.value;

                if (roll < 0.6f)
                {
                    drop = Instantiate(potionPrefab, transform.position + Vector3.up * 0.3f, Quaternion.identity);
                    if (lootParent != null)
                        drop.transform.SetParent(lootParent);
                }
                else if (roll < 0.9f)
                {
                    drop = Instantiate(totemPrefab, transform.position + Vector3.right * 0.5f, Quaternion.identity);
                    if (lootParent != null)
                        drop.transform.SetParent(lootParent);
                }
                break;
        }
    }

    void LateUpdate()
    {
        if (canvas != null)
        {
            canvas.transform.LookAt(Camera.main.transform);
            AvoidOverlap();
        }
    }

    void AvoidOverlap()
    {
        // Prevent UI overlap with nearby enemies
        float offsetY = 0f;
        Collider[] others = Physics.OverlapSphere(transform.position, 2f);

        foreach (var col in others)
        {
            if (col.gameObject == this.gameObject) continue;
            EnemyPatternHandler other = col.GetComponent<EnemyPatternHandler>();
            if (other != null)
            {
                float dist = Vector3.Distance(transform.position, other.transform.position);
                if (dist < 2f)
                {
                    offsetY += 0.6f;  // Apply offset if overlapping
                }
            }
        }

        // Ensure the UI position stays within the screen bounds
        Vector3 pos = canvas.transform.localPosition;
        pos.y += offsetY;

        // Get the screen bounds in world space
        float screenHeight = Screen.height;
        float worldHeight = Camera.main.orthographicSize * 2f; // For orthographic camera
        float screenToWorldRatio = worldHeight / screenHeight;
        float maxOffsetY = worldHeight * 0.25f; // Max 25% of screen height

        // Limit the offset to avoid going out of the screen
        pos.y = Mathf.Clamp(pos.y, -maxOffsetY, maxOffsetY);

        // Apply the position change
        canvas.transform.localPosition = pos;
    }
}