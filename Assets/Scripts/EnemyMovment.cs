using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovment : MonoBehaviour
{
    private GameObject player;
    private NavMeshAgent enemyAgent;

    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }


    void Update()
    {
        if (player.transform.tag.Equals("Player"))
        {
            enemyAgent.SetDestination(player.transform.position);
        }
    }
}
