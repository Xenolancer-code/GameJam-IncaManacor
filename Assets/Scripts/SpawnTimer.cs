using UnityEngine;
public class SpawnTimer : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private float spawnTime = 2f;
    [SerializeField] private float timeForDestroy = 5f;
    private float timer;


    void Start()
    {
        timer = 0f;
    }


    void Update()
    {
        if (timer > spawnTime)
        {
            GameObject spawner = Instantiate(enemy, transform.position, transform.rotation);

            Destroy(spawner, timeForDestroy);
            timer = 0f;
        }
        timer += Time.deltaTime;
    }
}
