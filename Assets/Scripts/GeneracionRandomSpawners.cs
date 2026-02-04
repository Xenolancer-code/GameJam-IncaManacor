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

    private void Update()
    {
        //CheckSpawnersState();
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

            GameObject newSpawner = Instantiate(spawnerPrefab, spawnPos, Quaternion.identity);

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

            if (Vector3.Distance(pos, player.transform.position) >= minDistanceToPlayer)
            {
                return pos;
            }
        }

        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radio);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistanceToPlayer);
    }
}
