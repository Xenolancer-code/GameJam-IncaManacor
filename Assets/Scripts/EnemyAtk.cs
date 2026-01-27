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
    private bool playerIsDead = false;
    
    private GameObject player;
    
    private void OnEnable()
    {
        MessageCentral.OnDiePlayer += PlayerisDead;
    }

    private void OnDisable()
    {
        MessageCentral.OnDiePlayer -= PlayerisDead;
    }
    
    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
 
    void Update()
    {
        // Si est? atacando -> no rotar ni moverse
        if (isAttacking || playerIsDead) return;
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

        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;

        // Detener movimiento por completo
        enemyAgent.isStopped = true;

        // Aqu? puedes activar la animaci?n
        animator.SetBool("isMoving", false);
        animator.SetTrigger("Hit");

        yield return new WaitForSeconds(stopByAtackPlayer);  // Duraci?n del ataque

        // Despu?s del ataque
        enemyAgent.isStopped = false;
        animator.SetBool("isMoving", true);
        isAttacking = false;
    }
    public void TryToDamage()
    {
        Debug.Log("Entrando en TryToDamage");
        if (player == null) return;
       
        var collidedPlayer = Physics.OverlapSphere(attackPoint.position, attackRadius, playerLayer);
        if (collidedPlayer == null || collidedPlayer.Length == 0) return;
       
        if (player.TryGetComponent(out HealtPlayerController healtPlayer))
        {
            Debug.Log("Estoy recibiendo da?o");
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

    private void PlayerisDead()
    {
        playerIsDead = true;
        enemyAgent.isStopped = true;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

}
