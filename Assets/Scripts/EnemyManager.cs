using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovment : MonoBehaviour
{
    [SerializeField] private float rotaX = 180;
    private GameObject player;
    private NavMeshAgent enemyAgent;
    private GameObject rightArm;

    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        rightArm = GameObject.Find("RightArm");
        player = GameObject.FindWithTag("Player");
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
}
