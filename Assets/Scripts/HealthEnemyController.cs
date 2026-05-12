using TMPro;
using UnityEngine;
using System.Collections;

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
        MessageCentral.OnAllSpawnersDestroyed += EnemyDead;
        MessageCentral.OnDieBoss += EnemyDead;
    }

    private void OnDisable()
    {
        MessageCentral.OnAllSpawnersDestroyed -= EnemyDead;
        MessageCentral.OnDieBoss -= EnemyDead;
    }

    public void GetDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        MessageCentral.DamagedEnemy();
        animator.SetTrigger("TakeHit");
        
        if (Random.value < 0.5f)
            AudioManager.I?.PlaySound(SoundName.EnemyInjured,gameObject.transform.position,1f);
        
        MessageCentral.DamagedEnemy();
        if(gameObject.activeSelf && currentHealth <= 0) {
            EnemyDead();        
        }
    }
    private void EnemyDead()
    {
        
        MessageCentral.DieEnemy();
        PoolManager.ReturnObjectToPool(gameObject);
        if (Random.value < 0.50f)
            DropOrb();
    }
    
    private void DropOrb()
    {
        Vector3 arriba = new Vector3 (0,0.5f,0);
        PoolManager.SpawnObject(dropPrefab, transform.position+arriba, Quaternion.identity);
    }

    
}
