using System;
using TMPro;
using UnityEngine;

public class HealthEnemyController : MonoBehaviour,IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private GameObject dropPrefab;
    private Animator animator;

    private void Awake()
    {
        animator=GetComponent<Animator>();
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public void GetDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        MessageCentral.DamagedEnemy();
        animator.SetTrigger("TakeHit");
        MessageCentral.DamagedEnemy();
        Debug.Log(currentHealth);
        if(gameObject.activeSelf && currentHealth <= 0) {
            Die();        
        }
    }
    private void Die()
    {
        MessageCentral.DieEnemy();
        PoolManager.ReturnObjectToPool(gameObject);
        DropOrb();
    }
    
    private void DropOrb()
    {
        Vector3 arriba = new Vector3 (0,0.5f,0);
        PoolManager.SpawnObject(dropPrefab, transform.position+arriba, Quaternion.identity);
    }

    
}
