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
        int patternCount = 1;

        switch (rank)
        {
            case EnemyRank.Low: patternCount = 1; break;
            case EnemyRank.Medium: patternCount = Random.Range(2, 3 + 1); break;
            case EnemyRank.High: patternCount = 3; break;
        }

        patternSequence.Clear();
        List<PatternType> used = new List<PatternType>();

        while (patternSequence.Count < patternCount)
        {
            PatternType rand = (PatternType)Random.Range(0, iconSprites.Length);

            // 确保最多重复一次
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

