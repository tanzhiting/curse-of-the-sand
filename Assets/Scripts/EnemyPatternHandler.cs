using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        canvas = GetComponentInChildren<Canvas>();
        enemyController = GetComponent<EnemyController>();

        GenerateRandomPattern();
        UpdateIcons();
        canvas.enabled = false;

                // 提高初始高度，结合等级决定基础偏移高度
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
                Destroy(gameObject);
            }
        }
        else
        {
            currentIndex = 0;
            UpdateIcons();
        }
    }

    void DropLoot()
    {
        float dropChance;
        switch (rank)
        {
            case EnemyRank.Low:
                dropChance = Random.value;
                if (dropChance < 0.4f)
                    Instantiate(potionPrefab, transform.position, Quaternion.identity);
                break;
            case EnemyRank.Medium:
                if (Random.value < 0.6f)
                    Instantiate(potionPrefab, transform.position, Quaternion.identity);
                if (Random.value < 0.2f)
                    Instantiate(totemPrefab, transform.position + Vector3.right * 0.5f, Quaternion.identity);
                break;
            case EnemyRank.High:
                Instantiate(potionPrefab, transform.position, Quaternion.identity);
                Instantiate(totemPrefab, transform.position + Vector3.right * 0.5f, Quaternion.identity);
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
        float offsetY = 0f;
        Collider[] others = Physics.OverlapSphere(transform.position, 2f); // 检查周围的敌人

        foreach (var col in others)
        {
            if (col.gameObject == this.gameObject) continue;
            EnemyPatternHandler other = col.GetComponent<EnemyPatternHandler>();
            if (other != null)
            {
                float dist = Vector3.Distance(transform.position, other.transform.position);
                if (dist < 2f)
                {
                    offsetY += 0.6f; // 位移更大，避免重叠
                }
            }
        }

        Vector3 pos = canvas.transform.localPosition;
        pos.y += offsetY;
        canvas.transform.localPosition = pos;
    }
}