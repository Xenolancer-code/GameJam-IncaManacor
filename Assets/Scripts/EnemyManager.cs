using System.Runtime.CompilerServices;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovment : MonoBehaviour
{
    [SerializeField] private float rotaX = 180; // Rotacion para mirar al player
    [SerializeField] private float rangeAtk = 5f; // Rango de ataques
    [SerializeField] private float attackCooldown = 1f;  // Tiempo entre ataques
    [SerializeField] private float lastAttackTime = 0f;
    private Transform player;
    private NavMeshAgent enemyAgent;
    private GameObject rightArm;
    
    


    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        rightArm = GameObject.Find("RightArm");
        player = GameObject.FindWithTag("Player").transform;
    }


    void Update()
    {
        float rotationValue = rotaX * Time.deltaTime;
        transform.Rotate(rotationValue, 0, 0);
        if (player.transform.tag.Equals("Player"))
        {
            enemyAgent.SetDestination(player.transform.position);
        }
  

    }

    private void Atack()
    {
        
        if(player.transform.position <= rangeAtk)
        {

            Debug.Log("Estoy atacando al Player");
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeAtk);
    }
}
