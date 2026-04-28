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
    private Transform player;
    private NavMeshAgent enemyAgent;
    private Animator animator;

    private bool takingDamage = false;
    [SerializeField] private float tiempoStuned =2f;

    //private float knockbackDuration = 0.2f; // <<--- velocidad del knockback
    //[SerializeField] private float StandingDuration = 2f; //Por usar
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
    }


    void Update()
    {
        // Si hay knockback → no hacer nada
        if (takingDamage) return;

        //Perseguir al player
        enemyAgent.SetDestination(player.transform.position);
        animator.SetFloat("velocity", enemyAgent.velocity.magnitude);
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






