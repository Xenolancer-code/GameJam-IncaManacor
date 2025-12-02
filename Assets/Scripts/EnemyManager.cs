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
    [SerializeField] private float stopByAtackPlayer = 1.1f; // Tiempo que el enemigos se queda parado al atacar
    private float lastAttackTime = 0f;

    private Transform player;
    private NavMeshAgent enemyAgent;
    private Animator animator;

    private bool playerInsideAttackRange = false;
    private bool isAttacking = false;


    private bool isKnockback = false; 
    private float knockbackDuration = 0.2f; // <<--- velocidad del knockback
    [SerializeField] private float StandingDuration = 2f;
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
        // Si hay knockback → no hacer nada
        if (isKnockback) return;
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
        isAttacking = true;

        // Detener movimiento por completo
        enemyAgent.isStopped = true;

        // Aquí puedes activar la animación
        animator.SetBool("isMoving", false);
        animator.SetTrigger("Hit");

        yield return new WaitForSeconds(stopByAtackPlayer);  // Duración del ataque

        // Después del ataque
        enemyAgent.isStopped = false;
        animator.SetBool("isMoving", true);
        isAttacking = false;
    }

  

    // ========================================================
    //        KNOCKBACK ((REVISAR FUNCIONALIDAD PARA APRENDER)
    // ========================================================
    public void ApplyKnockback(Vector3 direction, float distance)
    {
        if (!isKnockback) StartCoroutine(KnockbackRoutine(direction, distance));
    }

    private IEnumerator KnockbackRoutine(Vector3 direction, float distance)
    {
        isKnockback = true;
        animator.SetBool("isKnockBack",true);

        // Desactivar el NavMeshAgent para moverlo manualmente
        enemyAgent.enabled = false;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + direction.normalized * distance;

        float elapsed = 0;

        while (elapsed < knockbackDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / knockbackDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        // Reactivar NavMeshAgent
        enemyAgent.enabled = true;

        isKnockback = false;
        animator.SetBool("isKnockBack", false);
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




