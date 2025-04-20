using UnityEngine;

public enum HealingType
{
    Potion,
    Totem
}

public class HealingItem : MonoBehaviour
{   
    public HealingType itemType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats stats = other.GetComponent<PlayerStats>();
            
            if(stats != null)
            {
                switch(itemType)
                {
                    case HealingType.Potion:
                        stats.UsePotion();
                        break;
                    case HealingType.Totem:
                        stats.UseTotem();
                        break;    
                }

                Destroy(gameObject);
            }
        }
    }
}
