using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAtk : MonoBehaviour
{
    private NavMeshAgent enemyAgent;
    private Animator animator;
    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1f;  // Tiempo entre ataques
    [SerializeField] private float stopByAtackPlayer = 1.1f; // Tiempo que el enemigos se queda parado al atacar
    [SerializeField] private float attackRadius;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask playerLayer;
    private int hitPlayerHP = 1;
    private bool playerInsideAttackRange = false;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;

    private GameObject player;
    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        // Si está atacando -> no rotar ni moverse
        if (isAttacking) return;
        if (playerInsideAttackRange)
        {
            TryAttack();
        }
    }

    public void SetPlayer(GameObject _player)
    {
        player = _player;
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
    public void TryToDamage()
    {
        Debug.Log("Entrando en TryToDamage");
        if (player == null) return;
        Debug.Log(1);
        var collidedPlayer = Physics.OverlapSphere(attackPoint.position, attackRadius, playerLayer);
        if (collidedPlayer == null || collidedPlayer.Length == 0) return;
        Debug.Log(2);
        if (player.TryGetComponent(out HealtPlayerController healtPlayer))
        {
            Debug.Log("ouch");
            healtPlayer.GetDamage(hitPlayerHP);
        }

        //Vector3.Distance(attackPoint.position, player.transform.position);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

}
