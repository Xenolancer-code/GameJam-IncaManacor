using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private float damagePerTick = 20f;
    [SerializeField] private float damageInterval = 0.5f;

    private HashSet<HealthEnemyController> enemiesInside = new();

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //DA?O A ENEMIGOS
        if (other.TryGetComponent(out HealthEnemyController health))
        {
            if (enemiesInside.Add(health))
            {
                StartCoroutine(DamageOverTime(health));
            }
        }

        //DESTRUIR SPAWNER
        if (other.TryGetComponent(out EnemySpawner spawner))
        {
            Debug.Log("Estoy colisioando con el Spawner");
            spawner.spawnerActivation = false;
            spawner.smokePS.Play();
            spawner.explosionPS.Play();
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out HealthEnemyController health))
        {
            enemiesInside.Remove(health);
        }
    }

    private IEnumerator DamageOverTime(HealthEnemyController health)
    {
        while (health != null && enemiesInside.Contains(health))
        {
            health.GetDamage(damagePerTick);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
