using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // Llista estàtica de pools: un pool per cada tipus de prefab.
    private static readonly List<Pool> ObjectPools = new List<Pool>();

    // GameObject contenidor per mantenir els pooled objects agrupats a la jerarquia.
    private static GameObject _objectPoolEmptyHolder;

    private void Awake()
    {
        // Inicialitza el node pare dels objectes pooled.
        _objectPoolEmptyHolder = new GameObject("PooledObjects");
    }

    public static GameObject SpawnObject(GameObject prefabToSpawn, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        // Cercam el pool que correspon al nom del prefab.
        Pool pool = ObjectPools.Find(p => p.poolName == prefabToSpawn.name);

        // si el pool no existeix en cream un amb el nom del gameObject que volem obtenir
        if (pool == null)
        {
            pool = new Pool() { poolName = prefabToSpawn.name };
            ObjectPools.Add(pool);
        }

        // Comprovar si hi ha qualque gameObject inactiu per utilitzar
        GameObject spawnableOject = pool.inactiveObjects.FirstOrDefault();

        if (spawnableOject == null)
        {
            // Si no hi ha cap objecte lliure, en cream una instància nova.
            spawnableOject = Instantiate(prefabToSpawn, spawnPosition, spawnRotation);
            
            // El deixam penjat del contenidor general del pool.
            spawnableOject.transform.SetParent(_objectPoolEmptyHolder.transform);
        }
        else
        {
            // Si n'hi ha un disponible, l'actualitzam i el reactivam.
            spawnableOject.transform.position = spawnPosition;
            spawnableOject.transform.rotation = spawnRotation;
            pool.inactiveObjects.Remove(spawnableOject);
            spawnableOject.SetActive(true);
        }
        
        return spawnableOject;
    }
    
    public static GameObject SpawnObject(GameObject prefabToSpawn, Transform parentTransform)
    {
        // Variante de spawn amb pare específic.
        Pool pool = ObjectPools.Find(p => p.poolName == prefabToSpawn.name);

        // si el pool no existeix en cream un amb el nom del gameObject que volem obtenir
        if (pool == null)
        {
            pool = new Pool() { poolName = prefabToSpawn.name };
            ObjectPools.Add(pool);
        }

        // Comprovar si hi ha qualque gameObject inactiu per utilitzar
        GameObject spawnableOject = pool.inactiveObjects.FirstOrDefault();

        if (spawnableOject == null)
        {
            // Crea una instància nova com a fill del transform rebut.
            spawnableOject = Instantiate(prefabToSpawn, parentTransform);
        }
        else
        {
            // Reutilitza una instància inactiva del pool.
            pool.inactiveObjects.Remove(spawnableOject);
            spawnableOject.SetActive(true);
        }
        
        return spawnableOject;
    }

    public static void ReturnObjectToPool(GameObject obj)
    {
        // Recuperam el nom del prefab (sense el sufix "(Clone)").
        String trueObjectName = obj.name.Substring(0, obj.name.Length - "(Clone)".Length);
        Pool pool = ObjectPools.Find(p => p.poolName == trueObjectName);

        if (pool == null)
        {
            Debug.LogWarning("Pool " + obj.name + " was not found");
            // TODO: Tractar excepció
        }
        else
        {
            // En lloc de destruir, desactivam i tornam l'objecte al pool.
            obj.SetActive(false);
            pool.inactiveObjects.Add(obj);
        }
    }
}

public class Pool
{
    // Identificador del pool (normalment el nom del prefab original).
    public string poolName;
    // Col·lecció d'instàncies disponibles per reutilitzar.
    public List<GameObject> inactiveObjects = new List<GameObject>();
}