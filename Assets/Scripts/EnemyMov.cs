using System.Collections;
using System.Runtime.CompilerServices;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMov : MonoBehaviour
{
    [SerializeField] private Transform player;
    private NavMeshAgent enemyAgent;
    private Animator animator;
    [SerializeField] [Range(1, 3)] private float minEnemySpeed;
    [SerializeField] [Range(8, 12)] private float maxEnemySpeed;
    private bool takingDamage = false;
    [SerializeField] private float tiempoStuned;

    private float randomSpeed;

    private void OnEnable()
    {
        MessageCentral.OnDiePlayer += PlayerisDead;
        MessageCentral.OnDamagedEnemy +=TakingDamage;
    }

    private void OnDisable()
    {
        MessageCentral.OnDiePlayer -= PlayerisDead;
        MessageCentral.OnDamagedEnemy -=TakingDamage;
    }

    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        
    }


    void Start()
    {
        animator = GetComponent<Animator>();
        RandomVelocity();
    }


    void Update()
    {
        // Si hay knockback → no hacer nada
        if (takingDamage) return;

        //Perseguir al player
        enemyAgent.SetDestination(player.transform.position);
        animator.SetFloat("velocity", enemyAgent.velocity.magnitude);
    }

    private void RandomVelocity()
    {
        randomSpeed = Random.Range(minEnemySpeed, maxEnemySpeed);
        enemyAgent.speed = randomSpeed;
    }
    public void SetPlayer(GameObject _player)
    {
        player = _player.transform;
    }

    private void PlayerisDead()
    {
        enemyAgent.isStopped = true;
    }
    
    private void TakingDamage()
    {
        takingDamage=true;
        StartCoroutine(Tempo());
    }

    private IEnumerator Tempo()
    {
        yield return new WaitForSeconds(tiempoStuned);
        takingDamage = false;
    }
    
    
}






