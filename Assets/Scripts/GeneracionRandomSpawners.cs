using UnityEngine;

public class GeneracionRandomSpawners : MonoBehaviour
{
    [SerializeField] private GameObject spawnerPrefab;
    [SerializeField] private float minDistanceToPlayer = 20f;
    [SerializeField] private float radio;
    [SerializeField] private GameObject player;
    private void OnEnable()
    {
        MessageCentral.OnStart += GenerarSpawns;
    }

    private void OnDisable()
    {
        MessageCentral.OnStart -= GenerarSpawns;
    }

    void GenerarSpawns()
    {
        Vector3 spawnPos = GetRandomSpawnPosition();

        if (spawnPos != Vector3.zero)
        {
            GameObject newSpawner = Instantiate(spawnerPrefab, spawnPos, Quaternion.identity);
            if (newSpawner.TryGetComponent(out EnemySpawner enemySpawner))
            {
                enemySpawner.SetPlayerAtSpawner(player);
            }
        }
    }


    Vector3 GetRandomSpawnPosition()
    {
        int attempts = 10; // intentos para no spawnear encima del jugador

        for (int i = 0; i < attempts; i++)
        {
            // posici?n aleatoria dentro de un c?rculo alrededor del centro del ring
            Vector2 circle = Random.insideUnitCircle * radio;
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
        Gizmos.color = Color.lightPink;
        Gizmos.DrawWireSphere(transform.position, radio);
        Gizmos.color = Color.pink;
        Gizmos.DrawWireSphere(transform.position,minDistanceToPlayer);
    }
}
