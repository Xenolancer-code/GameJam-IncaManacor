using System.Collections;
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
    private Animator animator;

    private bool playerInsideAttackRange = false;
    private bool isAttacking = false;


    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;   
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        // Si está atacando -> no rotar ni moverse
        if (isAttacking) return;

        //Rotar para mirar player
        float rotationValue = rotaX * Time.deltaTime;
        transform.Rotate(rotationValue, 0, 0);

        //Perseguir al player
        enemyAgent.SetDestination(player.transform.position);
        //Intento de ataque
        if (playerInsideAttackRange)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;

        Debug.Log("ATACANDO AL PLAYER");

        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        animator.SetTrigger("Hit");
        isAttacking = true;

        // Detener movimiento por completo
        enemyAgent.isStopped = true;

        // Aquí puedes activar la animación
        // anim.SetTrigger("Attack");

        yield return new WaitForSeconds(2f);  // Duración del ataque (2 segundos)

        // Después del ataque
        enemyAgent.isStopped = false;
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider detect)
    {
        if (detect.CompareTag("Player"))
        {
            playerInsideAttackRange = true;
        }
    }

    private void OnTriggerExit(Collider detect)
    {
        if (detect.CompareTag("Player"))
        {
            playerInsideAttackRange = false;
        }
    }
}




