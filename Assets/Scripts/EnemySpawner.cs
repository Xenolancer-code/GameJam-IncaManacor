using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private int maxEnemies = 5;            
    [SerializeField] private float spawnRadius = 20f;      
    [SerializeField] private float minDistanceToPlayer = 5f; 
    [SerializeField] private float spawnDelay = 0.5f;        
    [SerializeField] private float initialDelay = 5f;     

    private List<GameObject> enemiesAlive = new List<GameObject>();
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer < initialDelay)
            return;

        // Si hay menos enemigos que el máximo
        if (enemiesAlive.Count < maxEnemies)
        {
            timer = 0f;
            SpawnEnemy();
        }

        // Limpiar lista de enemigos que han sido destruidos
        enemiesAlive.RemoveAll(e => e == null);
    }

    void SpawnEnemy()
    {
        Vector3 spawnPos = GetRandomSpawnPosition();

        if (spawnPos != Vector3.zero)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            enemiesAlive.Add(newEnemy);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        int attempts = 10; // intentos para no spawnear encima del jugador

        for (int i = 0; i < attempts; i++)
        {
            // posición aleatoria dentro de un círculo alrededor del centro del ring
            Vector2 circle = Random.insideUnitCircle * spawnRadius;
            Vector3 pos = new Vector3(circle.x, 0, circle.y) + transform.position;

            // distancia mínima al player
            if (Vector3.Distance(pos, player.position) >= minDistanceToPlayer)
            {
                return pos;
            }
        }

        // Si no encuentra posición válida, no spawnea
        return Vector3.zero;
    }
}
