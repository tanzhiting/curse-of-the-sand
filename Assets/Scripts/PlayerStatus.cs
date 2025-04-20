using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth;

    private float currentHealth;

    public HealthBar healthbar;

    [Header("Poison Settings")]
    [SerializeField] private bool isPoisoned = true;
    [SerializeField] private float poisonInterval;
    [SerializeField] private float poisonDamage;
    [SerializeField] private GameObject poisonParticleSystem;
    private GameObject activePoisonEffect;

    private float poisonTimer = 0f;
    private float poisonCooldownTimer = 0f;
    private bool isImmuneToPoison = false;

    private Color poisonColor = new Color(138f / 255f, 122f / 255f, 9f / 255f);  // #8A7A09
    private Color cureColor = new Color(175f / 255f, 29f / 255f, 29f / 255f);    // #AF1D1D

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        currentHealth = maxHealth;
        healthbar.SetSliderMax(maxHealth);
        healthbar.SetBarColor(poisonColor);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UsePotion();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseTotem();
        }

        if (isPoisoned)
        {
            poisonTimer += Time.deltaTime;

            if (activePoisonEffect == null)
            {
                activePoisonEffect = Instantiate(poisonParticleSystem, transform.position, Quaternion.identity);
                activePoisonEffect.transform.SetParent(transform);
                activePoisonEffect.transform.localPosition = Vector3.zero;
            }

            if (poisonTimer >= poisonInterval)
            {   
                TakeDamage(poisonDamage);
                poisonTimer = 0f;
            }
        }

        if (isImmuneToPoison)
        {
            poisonCooldownTimer += Time.deltaTime;

            if (poisonCooldownTimer >= 5f)
            {
                isImmuneToPoison = false;
                isPoisoned = true;
                poisonCooldownTimer = 0f;
                healthbar.SetBarColor(poisonColor);
                Debug.Log("5 seconds have passed and be poisoned again.");
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        healthbar.SetSlider(currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Player has died.");
            // 可以加死亡动画、结束游戏等
        }
    }

    
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        healthbar.SetSlider(currentHealth);
    }

    public void UsePotion()
    {
        Heal(1f);
        CurePoisonTemporarily();
    }

    public void UseTotem()
    {
        Heal(2f);
        CurePoisonTemporarily();
    }

    private void CurePoisonTemporarily()
    {   
        if (activePoisonEffect != null)
        {
            Destroy(activePoisonEffect);
            activePoisonEffect = null;
        }

        isPoisoned = false;
        isImmuneToPoison = true;
        poisonCooldownTimer = 0f;

        healthbar.SetBarColor(cureColor);
    }
}