using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float attackRange = 5f;
    public Transform player;

    public void OnButtonClick(string pattern)
    {
        if (string.IsNullOrEmpty(pattern)) return;

        PatternType inputType = (PatternType)Enum.Parse(typeof(PatternType), pattern);
        EnemyPatternHandler[] enemies = FindObjectsByType<EnemyPatternHandler>(FindObjectsSortMode.None);

        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(player.position, enemy.transform.position);
            if (dist <= attackRange)
            {
                enemy.ReceiveInput(inputType);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(player.position, attackRange);
    }
}