using TMPro;
using UnityEngine;

public class HealthEnemyController : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private TextMeshProUGUI textHealth;
    [SerializeField] private GameObject dropPrefab;
    [SerializeField] private GameObject bloodparticle;
    
    private void Awake()
    {
        currentHealth = maxHealth;
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
        //TODO Logica de morirse(particulas,sonido animacion etc)
        MessageCentral.DieEnemy();
        Destroy(gameObject);
        Vector3 arriba = new Vector3 (0,0.5f,0);
        GameObject particle = Instantiate(bloodparticle, transform.position+arriba, Quaternion.identity);
        GameObject drop = Instantiate(dropPrefab, transform.position+arriba, Quaternion.identity);
    }
}
