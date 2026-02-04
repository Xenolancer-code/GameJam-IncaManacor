using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Settings Padre")]
    private GeneracionRandomSpawners listaCriptas;
    [Header("Spawner Settings")]
    [SerializeField] private GameObject enemyPrefab;
    private GameObject player;
    [SerializeField] private int maxEnemies = 5;            
    [SerializeField] private float spawnRadius = 20f;      
    [SerializeField] private float minDistanceToPlayer = 5f; 
    //[SerializeField] private float spawnDelay = 0.5f; //Por usar
    [SerializeField] private float initialDelay = 5f;     
    [Header("ParticleSystem")]
    [SerializeField] public ParticleSystem smokePS;
    [SerializeField] public ParticleSystem explosionPS;
    

    private List<GameObject> enemiesAlive = new List<GameObject>();
    private float timer = 0f;
    public bool spawnerActivation=true;




    private void Start()
    {
        //Funcionalidad del Spawner
        /* Cuando inicia el juego hay un periodo de gracia donde no se activa
         * luego de eso hacen spawn X cantidad de enemigos a la vez, pudiendo aumentar en el tiempo
         * (pero con un numero maximo de enemigos)[maybe] cuando un enemigo muere
         * otro alomejor en 0,2s hace spawn para intentar siempre tener una buena cantidad de enemigos presentes
         */
    }
    void Update()
    {
        if(spawnerActivation)
        {
            ControllerSpawns();
        }
        else
        {
            SelfDestroy();
        }
    }

    public void SetPlayerAtSpawner(GameObject _player)
    {
        player = _player;
    }
    private void ControllerSpawns()
    {
            timer += Time.deltaTime;

            if (timer < initialDelay)
                return;

            // Si hay menos enemigos que el m?ximo
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
            if(newEnemy.TryGetComponent(out EnemyAtk enemyAtk))
            {
                enemyAtk.SetPlayer(player);
            }
            if(newEnemy.TryGetComponent(out EnemyMov enemyMov))
            {
                enemyMov.SetPlayer(player);
            }
            enemiesAlive.Add(newEnemy);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        int attempts = 10; // intentos para no spawnear encima del jugador

        for (int i = 0; i < attempts; i++)
        {
            // posici?n aleatoria dentro de un c?rculo alrededor del centro del ring
            Vector2 circle = Random.insideUnitCircle * spawnRadius;
            Vector3 pos = new Vector3(circle.x, 0, circle.y) + transform.position;

            // distancia m?nima al player
            if (Vector3.Distance(pos, player.transform.position) >= minDistanceToPlayer)
            {
                return pos;
            }
        }

        // Si no encuentra posici?n v?lida, no spawnea
        return Vector3.zero;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.orangeRed;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    private void SelfDestroy()
    {
        if (spawnerActivation)return;
        listaCriptas.CheckSpawnersState(gameObject);
    }

}
