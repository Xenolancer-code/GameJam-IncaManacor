using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuzSpawners : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] private float awakeTime = 5f;
    private List<GameObject> enemiesAlive = new List<GameObject>();
    [SerializeField] private float initialDelay = 5f;     
    [SerializeField] private int maxEnemies = 5;    
    [SerializeField] private GameObject enemyLuzPrefab;
    private float timer = 0f;
    public bool spawnerActivation=true;
    
    private void OnEnable()
    {
        MessageCentral.OnSwapScene += CorutinaSpawnerLuz;
        MessageCentral.OnDiePlayer += DesactiveEnemies;
    }

    private void OnDisable()
    {
        MessageCentral.OnSwapScene -= CorutinaSpawnerLuz;
        MessageCentral.OnDiePlayer -= DesactiveEnemies;
    }
        /* TODO
         -Iniciar spawers X tiempo luego de que el player aparezca en plano de Luz -
         -Tener la misma función de cantidad maxima de enemigos pero solo aparecen en 1 punto -
         -Estos no se destruyen sino se apagan al termianr el juego -
         -Cuando termina el juego tambíen se destruyen todos los enemigos
         -Y efectos visuales y/o animaciones
         */
    
    private void ControllerSpawns()
    {
        if(!spawnerActivation) return;
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
        Vector3 spawnPos = transform.position+Vector3.forward;

        if (spawnPos != Vector3.zero)
        {
            
            GameObject newEnemy = Instantiate(enemyLuzPrefab, spawnPos, Quaternion.identity);
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

    private void CorutinaSpawnerLuz()
    {
        StartCoroutine(AwakeSpawners());
    }

    private IEnumerator AwakeSpawners()
    {
        yield return new WaitForSeconds(awakeTime);
        ControllerSpawns();
    }

    private void DesactiveEnemies()
    {
        spawnerActivation = false;
    }
}
