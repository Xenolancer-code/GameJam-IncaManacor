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
    [SerializeField] private float rotaX = 180; // Rotacion para mirar al player

    private Transform player;
    private NavMeshAgent enemyAgent;
    private Animator animator;

    private bool isKnockback = false; 
    //private float knockbackDuration = 0.2f; // <<--- velocidad del knockback
    //[SerializeField] private float StandingDuration = 2f; //Por usar
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
        
        //Rotar para mirar player
        float rotationValue = rotaX * Time.deltaTime;
        transform.Rotate(rotationValue, 0, 0);

        //Perseguir al player
        enemyAgent.SetDestination(player.transform.position);
        animator.SetFloat("velocity", enemyAgent.velocity.magnitude);
    }

    private void PlayerisDead()
    {
        enemyAgent.isStopped = true;
    }
    
    // ========================================================
    //        KNOCKBACK (BORRAR PORQUE YA NO EMPUJA)
    // ========================================================
    //public void ApplyKnockback(Vector3 direction, float distance)
    //{
    //    if (!isKnockback) StartCoroutine(KnockbackRoutine(direction, distance));
    //}

    //private IEnumerator KnockbackRoutine(Vector3 direction, float distance)
    //{
    //    isKnockback = true;
    //    animator.SetBool("isKnockBack",true);

    //    // Desactivar el NavMeshAgent para moverlo manualmente
    //    enemyAgent.enabled = false;

    //    Vector3 startPos = transform.position;
    //    Vector3 targetPos = startPos + direction.normalized * distance;

    //    float elapsed = 0;

    //    while (elapsed < knockbackDuration)
    //    {
    //        transform.position = Vector3.Lerp(startPos, targetPos, elapsed / knockbackDuration);
    //        elapsed += Time.deltaTime;
    //        yield return null;
    //    }

    //    transform.position = targetPos;

    //    // Reactivar NavMeshAgent
    //    enemyAgent.enabled = true;

    //    isKnockback = false;
    //    animator.SetBool("isKnockBack", false);
    }//Revisa de hacer alomejor una corutina para el stun del enemigo una vez empujado






