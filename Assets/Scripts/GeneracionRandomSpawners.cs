using System.Collections.Generic;
using UnityEngine;

public class GeneracionRandomSpawners : MonoBehaviour
{
    [Header("Spawner")]
    [SerializeField] private GameObject spawnerPrefab;
    [SerializeField] private int spawnersIniciales = 4;
    private bool spawnersGenerated=false;

    [Header("Area")]
    [SerializeField] private float minDistanceToPlayer = 20f;
    [SerializeField] private float radio;

    [Header("References")]
    [SerializeField] private GameObject player;
    
    [Header("Obstáculos")]
    [SerializeField] private float minDistanceToObjects = 3f;
    [SerializeField] private LayerMask obstacleLayer; // Asigna las layers de objetos en el Inspector

    // Lista de spawners activos
    private List<GameObject> activeSpawners = new();

    // Estado
    private bool allSpawnersDestroyed = false;

    private void OnEnable()
    {
        MessageCentral.OnStart += GenerarSpawnsIniciales;
        
    }

    private void OnDisable()
    {
        MessageCentral.OnStart -= GenerarSpawnsIniciales;
        
    }

    // Genera X spawners a la vez
    private void GenerarSpawnsIniciales()
    {
        allSpawnersDestroyed = false;
        spawnersGenerated = true;
        activeSpawners.Clear();

        for (int i = 0; i < spawnersIniciales; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            if (spawnPos == Vector3.zero) continue;

            
            Vector3 direction = (player.transform.position - spawnPos).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);
            GameObject newSpawner = Instantiate(spawnerPrefab, spawnPos, rotation);

            if (newSpawner.TryGetComponent(out EnemySpawner enemySpawner))
            {
                enemySpawner.SetPlayerAtSpawner(player);
                enemySpawner.SetParentManager(this);
            }

            activeSpawners.Add(newSpawner);
        }
    }

    // Limpia spawners destruidos y detecta si no queda ninguno
    
    public void RemoveSpawner(GameObject spawner)
    {
        if (!activeSpawners.Contains(spawner)) return;

        activeSpawners.Remove(spawner);

        Debug.Log($"Spawner eliminado manualmente. Restantes: {activeSpawners.Count}");

        MessageCentral.SpawnerDestroyed();

        if (!allSpawnersDestroyed && activeSpawners.Count == 0)
        {
            allSpawnersDestroyed = true;
            Debug.Log("Todos los spawners han sido eliminados");
            MessageCentral.AllSpawnersDestroyed();
        }
    }


    // Hace intentos para generar los spawners fuera del radio del player
    private Vector3 GetRandomSpawnPosition()
    {
        int attempts = 10;

        for (int i = 0; i < attempts; i++)
        {
            Vector2 circle = Random.insideUnitCircle * radio;
            Vector3 pos = new Vector3(circle.x, 0, circle.y) + transform.position;

            bool lejosDeljugador = Vector3.Distance(pos, player.transform.position) >= minDistanceToPlayer;
            bool sinObstaculos = !Physics.CheckSphere(pos, minDistanceToObjects, obstacleLayer);

            if (lejosDeljugador && sinObstaculos)
                return pos;
        }

        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radio);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.transform.position, minDistanceToPlayer);
        
        Gizmos.color = Color.yellow;
        foreach (var spawner in activeSpawners)
        {
            if (spawner != null)
                Gizmos.DrawWireSphere(spawner.transform.position, minDistanceToObjects);
        }
    }
}
