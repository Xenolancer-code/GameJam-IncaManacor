using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLuzAtk : MonoBehaviour
{
    private NavMeshAgent enemyAgent;
    private Animator animator;
    private GameObject player;
    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1f;  // Tiempo entre ataques
    [SerializeField] private float stopByAtackPlayer = 3f; // Tiempo que el enemigos se queda parado al atacar
    
    [SerializeField] private float minAttackDistance = 3f; 
    [SerializeField] private float maxAttackDistance = 7f;
    private float attackDistance;
    [SerializeField] private LayerMask playerLayer;
    [Header("CastSettings")]
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Transform fireballPoint;
    [SerializeField] ParticleSystem fireEffect;
    
    private bool playerInsideAttackRange = false;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;
    private bool playerIsDead = false;
       
    void Awake() 
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        attackDistance = Random.Range(minAttackDistance, maxAttackDistance);//new
        MessageCentral.OnDiePlayer += PlayerisDead;
        fireEffect.Stop();
    }

    private void OnDisable()
    {
        MessageCentral.OnDiePlayer -= PlayerisDead;
    }

    public void SetPlayer2(GameObject _player)
    {
        player = _player;
    }
    
    void Update()
    {
        if (isAttacking || playerIsDead) return;
        PlayerInRange();
        if (playerInsideAttackRange)
        {
            TryAttack();
        }
    }
   
    private void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;
        if(Random.value > 0.7f)
            StartCoroutine(AttackSequence());
    }
    
    private IEnumerator AttackSequence()
    {
        isAttacking = true;

        // Detener movimiento por completo
        enemyAgent.isStopped = true;

        // Aqu? puedes activar la animacion
        animator.SetBool("isMoving", false);
        fireEffect.Play();
        animator.SetTrigger("Cast");

        yield return new WaitForSeconds(stopByAtackPlayer);  // Duracion del ataque

        // Despu?s del ataque
        enemyAgent.isStopped = false;
        animator.SetBool("isMoving", true);
        isAttacking = false;
    }

    private void PlayerInRange()
    {
        // float distance = Vector3.Distance(transform.position, player.transform.position);
        // if (distance <= attackDistance)
        // {
        //     playerInsideAttackRange = true;
        // }
        // else
        // {
        //     playerInsideAttackRange = false;
        // }
        float distance = Vector3.Distance(transform.position, player.transform.position);
        playerInsideAttackRange = distance <= attackDistance;
    }
    public void CastFireball()
    {
        GameObject fireball = Instantiate(fireballPrefab, fireballPoint.position, Quaternion.identity);
        //GameObject fireball = PoolManager.SpawnObject(fireballPrefab, fireballPoint.position, Quaternion.identity);

        fireball.GetComponent<FireBall>().Init(player.transform);

        fireEffect.Stop();
    }
    
    private void PlayerisDead()
    {
        playerIsDead = true;
        enemyAgent.isStopped = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.darkRed;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
