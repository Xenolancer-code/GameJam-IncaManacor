using System;
using TMPro;
using UnityEngine;

public class HealthEnemyController : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private TextMeshProUGUI textHealth;
    [SerializeField] private GameObject dropPrefab;
    [SerializeField] private GameObject bloodparticle;
    
    private void OnEnable()
    {
        currentHealth = maxHealth;
        textHealth.text = maxHealth.ToString();
    }

    public void GetDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        textHealth.text = currentHealth.ToString();
        MessageCentral.DamagedEnemy();
        Debug.Log(currentHealth);
        if(currentHealth <= 0) {
            currentHealth = 0;
            Die();        
        }
    }
    private void Die()
    {
        MessageCentral.DieEnemy();
        PoolManager.ReturnObjectToPool(gameObject);
    }

    private void OnDisable()
    {
        Vector3 arriba = new Vector3 (0,0.5f,0);
        //GameObject particle = PoolManager.SpawnObject(bloodparticle, transform.position+arriba, Quaternion.identity);
        GameObject drop = PoolManager.SpawnObject(dropPrefab, transform.position+arriba, Quaternion.identity);
    }
}
