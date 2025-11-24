using Unity.VisualScripting;
using UnityEngine;
public class PlayerAtk : MonoBehaviour
{
    //private CharacterController cc;
    [Header("Attack Controller")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask enemyLayer;
    [Header("Damage Amount Controller")]
    [SerializeField] private float damageAmount = 100f;
    [Header("Knockback Controller")]
    [SerializeField] public float aoeRadius = 4f;
    [SerializeField] public float knockbackDistance = 3f;
    private Animator animator;
    private HealthEnemyController hc;

    private void Awake()
    {
        //cc = gameObject.GetComponent<CharacterController>();
    }
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void BasicAtk()
    {
        animator.SetTrigger("LeftClick");
        Debug.Log("Estoy atacando al enemigo");
        var collidedEnemies = Physics.OverlapSphere(attackPoint.position, attackRadius, enemyLayer);
        if (collidedEnemies == null) return;
        if (TryGetComponent(out HealthEnemyController healthcontroller) != null) {

            healthcontroller.GetDamage(damageAmount);
        }
    }
       

   
    public void AoEAtk()
    {
        animator.SetTrigger("RightClick");
        Debug.Log("Estoy golpeando en area");
    }


    public void DoAoEKnockback()
    {
        Debug.Log("KNOCKBACK ejecutado desde la animación");
        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyManager kb = hit.GetComponent<EnemyManager>();
                if (kb != null)
                {
                    Vector3 dir = (hit.transform.position - transform.position).normalized;
                    kb.ApplyKnockback(dir, knockbackDistance);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
