using TMPro;
using UnityEngine;

public class HealthEnemyController : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private TextMeshProUGUI textHealth;
    private void Awake()
    {
        currentHealth = maxHealth;
    }
    public void GetDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        textHealth.text = currentHealth.ToString();
        MessageCentral.DamagedEnemy();
        MessageCentral.IncrementPlayerDamage();
        MessageCentral.IncrementPlayerRange();
        Debug.Log(currentHealth);
        if(currentHealth <= 0) {
            currentHealth = 0;
            Die();        
        }
    }
    private void Die()
    {
        //TODO Logica de morirse(particulas,sonido animacion etc)
        MessageCentral.DieEnemy();
        Destroy(gameObject);
    }
}
