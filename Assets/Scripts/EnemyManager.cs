using System.Runtime.CompilerServices;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyManager : MonoBehaviour
{
    [SerializeField] private float rotaX = 180; // Rotacion para mirar al player
    [SerializeField] private float attackCooldown = 1f;  // Tiempo entre ataques
    private float lastAttackTime = 0f;
    private Transform player;
    private NavMeshAgent enemyAgent;
    private bool playerInsideAttackRange = false;
    private GameObject body;
    



    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
   
        player = GameObject.FindWithTag("Player").transform;
       
    }


    void Update()
    {
        float rotationValue = rotaX * Time.deltaTime;
        transform.Rotate(rotationValue, 0, 0);
        enemyAgent.SetDestination(player.transform.position);
        if (playerInsideAttackRange)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        // Control de cooldown
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;

        Debug.Log("Ataque al player via Trigger");

        // Aquí puedes añadir daño o animación
        // rightArm.GetComponent<Animator>().SetTrigger("Attack");
        
    }

    // Detecta cuando el player entra en el rango
    private void OnTriggerEnter(Collider detect)
    {
        if (detect.tag.Equals("Player"))
        {
            playerInsideAttackRange = true;
        }
    }

    // Detecta cuando el player sale del rango
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            playerInsideAttackRange = false;
        }
    }
}



