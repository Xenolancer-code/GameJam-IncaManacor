using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LuzSpawners : MonoBehaviour
{
    private Animator animatorPortalSpawner;
    [SerializeField] private GameObject player;
    [SerializeField] private float awakeTime = 4f;
    private List<GameObject> enemiesAlive2 = new List<GameObject>();
    [SerializeField] private float initialDelay = 4f;     
    [SerializeField] private int maxEnemies = 5;    
    [SerializeField] private GameObject enemyLuzPrefab;
    private float timer = 0f;
    private bool spawnerActivation=false;

    private void Awake()
    {
        animatorPortalSpawner = GetComponent<Animator>();
    }
    private void Start()
    {
        spawnerActivation=false;
    }
    private void OnEnable()
    {
        Debug.Log("OnEnable LuzSpawners");
        MessageCentral.OnSwapScene += CorutinaSpawnerLuz;
        MessageCentral.OnDiePlayer += DesactiveEnemies;
        MessageCentral.OnDieBoss += DesactiveEnemies;
    }

    private void OnDisable()
    {
        MessageCentral.OnSwapScene -= CorutinaSpawnerLuz;
        MessageCentral.OnDiePlayer -= DesactiveEnemies;
        MessageCentral.OnDieBoss -= DesactiveEnemies;
    }
    void Update()
    {
        if(spawnerActivation)
        {
            ControllerSpawns();
        }
    }
    private void ControllerSpawns()
    {
        timer += Time.deltaTime;

        if (timer < initialDelay) return;

        // Si hay menos enemigos que el m?ximo
        if (enemiesAlive2.Count < maxEnemies)
        {
            timer = 0f;
            SpawnEnemy();
        }

        // Limpiar lista de enemigos que han sido destruidos

        enemiesAlive2.RemoveAll(e => !e.activeSelf);
    }

    void SpawnEnemy()
    {
        Vector3 spawnPos = transform.position+Vector3.forward;

        if (spawnPos != Vector3.zero)
        {
            
            GameObject newEnemy = PoolManager.SpawnObject(enemyLuzPrefab, spawnPos, Quaternion.identity);
            Debug.Log("Enemigo instanciado");
            if(newEnemy.TryGetComponent(out EnemyLuzAtk enemyLuzAtk))
            {
                enemyLuzAtk.SetPlayer2(player);
            }
            if(newEnemy.TryGetComponent(out EnemyMov enemyMov))
            {
                enemyMov.SetPlayer(player);
            }
            enemiesAlive2.Add(newEnemy);
        }
    }

    private void CorutinaSpawnerLuz()
    {
        Debug.Log("Call coroutine awake spawners");
        StartCoroutine(AwakeSpawners());
    }

    private IEnumerator AwakeSpawners()
    {
        yield return new WaitForSeconds(awakeTime);
        animatorPortalSpawner.SetTrigger("OpenPortal");
        spawnerActivation=true;
    }

    private void DesactiveEnemies()
    {
        spawnerActivation = false;
    }
}
