using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private float damage;
    
    void Start()
    {
        Destroy(gameObject,lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("enemigos entrando");
        if(other.TryGetComponent(out HealthEnemyController healthcontroller))
        {
            healthcontroller.GetDamage(damage);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("enemigos se qeudan adentro");
        if (other.TryGetComponent(out HealthEnemyController healthcontroller))
        {
            healthcontroller.GetDamage(damage);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.darkGreen;
        Gizmos.DrawWireSphere(transform.position, 6f);
    }
}
