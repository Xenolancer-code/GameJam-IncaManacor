using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private float damagePerTick = 20f;
    [SerializeField] private float damageInterval = 0.5f;
    [SerializeField] private GameObject impactEffect;

    private HashSet<IDamageable> enemiesInside = new();

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = GetDamageable(other);
       
        if (damageable != null && enemiesInside.Add(damageable))
        {
            StartCoroutine(DamageOverTime(damageable,other));
        }

        // DESTRUIR SPAWNER
        if (other.TryGetComponent(out EnemySpawner spawner))
        {
            Debug.Log("Estoy colisionando con el Spawner");
            spawner.spawnerActivation = false;
            spawner.smokePS.Play();
            spawner.explosionPS.Play();
            AudioManager.I?.PlaySound(SoundName.EliminateSmoke, spawner.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IDamageable damageable = GetDamageable(other);
        
        // Solo busca BossHealtController si no encontró HandHealth
        if (damageable == null)
        {
            BossHealtController boss = other.GetComponentInParent<BossHealtController>();
            if (boss != null) damageable = boss as IDamageable;
        }
        
        if (damageable != null)
        {
            enemiesInside.Remove(damageable);
        }
    }

    private IDamageable GetDamageable(Collider other)
    {
        return other.GetComponent<HandHealth>()
               ?? other.GetComponentInParent<BossHealtController>()
               ?? other.GetComponent<HealthEnemyController>() as IDamageable;
    }

    private IEnumerator DamageOverTime(IDamageable damageable,Collider collider)
    {
        // Necesitamos el MonoBehaviour para comprobar si fue destruido
        MonoBehaviour mb = damageable as MonoBehaviour;
        Vector3 impact = collider.ClosestPoint(transform.position);
        while (mb != null && enemiesInside.Contains(damageable))
        {
            damageable.GetDamage(damagePerTick);
            Instantiate(impactEffect,impact,Quaternion.identity);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}