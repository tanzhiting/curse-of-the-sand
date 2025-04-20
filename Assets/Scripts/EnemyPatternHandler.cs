using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PatternType { A, B, C } // 你有多少种图案，就列多少种

public enum EnemyRank
{
    Low,    // Mummy
    Medium, // Anubis
    High    // Pharaoh
}

public class EnemyPatternHandler : MonoBehaviour
{
    public List<PatternType> patternSequence = new List<PatternType>();
    private int currentIndex = 0;

    public Image[] iconSlots; // 绑定 Canvas 里的图案图标
    public Sprite[] iconSprites; // 图案Sprite资源

    public float revealDistance = 5f;
    private Transform player;

    private Canvas canvas;
    private EnemyController enemyController;

    public EnemyRank rank = EnemyRank.Low;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        canvas = GetComponentInChildren<Canvas>();
        enemyController = GetComponent<EnemyController>();

        GenerateRandomPattern();
        UpdateIcons();
        canvas.enabled = false;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        canvas.enabled = dist <= revealDistance;
    }

    void GenerateRandomPattern()
    {
        int basePattern = 1;

        switch (rank)
        {
            case EnemyRank.Low: basePattern = 1; break;
            case EnemyRank.Medium: basePattern = 2; break;
            case EnemyRank.High: basePattern = 3; break;
        }

        // 加上微调的随机性（-1, 0, +1）
        int finalPatternCount = Mathf.Clamp(basePattern + Random.Range(-1, 2), 1, 3);

        patternSequence.Clear();

        int lastType = -1; // 确保不是三个都一样

        for (int i = 0; i < finalPatternCount; i++)
        {
            PatternType randomType;
            int retry = 0;

            do
            {
                randomType = (PatternType)Random.Range(0, iconSprites.Length);
                retry++;
            } while (i > 0 && (int)randomType == lastType && retry < 10); // 避免全部重复

            lastType = (int)randomType;
            patternSequence.Add(randomType);
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
            iconSlots[currentIndex].color = Color.gray; // 可做击中效果
            currentIndex++;

            if (currentIndex >= patternSequence.Count)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            currentIndex = 0;
            UpdateIcons(); 
        }
    }

    void LateUpdate()
    {
        if (canvas != null)
        {
            canvas.transform.LookAt(Camera.main.transform);
        }
    }
}

