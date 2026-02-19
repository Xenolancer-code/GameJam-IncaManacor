using UnityEngine;
using UnityEngine.AI;

public class EnemyLuzAtk : MonoBehaviour
{
    private NavMeshAgent enemyAgent;
    private Animator animator;
    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1f;  // Tiempo entre ataques
    [SerializeField] private float stopByAtackPlayer = 1.1f; // Tiempo que el enemigos se queda parado al atacar
    [SerializeField] private float attackDistance;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask playerLayer;
    private int hitPlayerHP = 1;
    private bool playerInsideAttackRange = false;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;
    private bool playerIsDead = false;
    
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
