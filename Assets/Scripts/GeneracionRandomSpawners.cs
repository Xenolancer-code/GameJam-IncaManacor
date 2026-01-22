using UnityEngine;

public class GeneracionRandomSpawners : MonoBehaviour
{
    [SerializeField] private GameObject spawnerPrefab;
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
            // posición aleatoria dentro de un círculo
            Vector2 circle = Random.insideUnitCircle * radio;
            Vector3 pos = new Vector3(circle.x, 0, circle.y) + transform.position;
            return pos;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.lightPink;
        Gizmos.DrawWireSphere(transform.position, radio);
    }
}
