using Unity.VisualScripting;
using UnityEngine;
public class PlayerAtk : MonoBehaviour
{
    //private CharacterController cc;
    [Header("Damage Amount Controller")]
    [SerializeField] private float damageAmount = 100f;
    [Header("Knockback Controller")]
    [SerializeField] public float aoeRadius = 4f;
    [SerializeField] public float knockbackDistance = 3f;
    private Animator animator;
    private HealthController hc;
    private GameObject target;
    private void Awake()
    {
        //cc = gameObject.GetComponent<CharacterController>();
    }
    void Start()
    {   
        animator = GetComponentInChildren<Animator>();
    }

    public void LeftClickPressed()
    {
        animator.SetTrigger("LeftClick");
        Debug.Log("Estoy atacando al enemigo");
        if(target != null)
        {
            target.GetComponent<HealthController>().GetDamage(damageAmount);
        }
    }
   
    public void RightClickPressed()
    {
        animator.SetTrigger("RightClick");
        Debug.Log("Estoy golpeando en area");
    }

    private void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == "Enemy")
        {
            Debug.Log("Colision amb enemic");
            target = coll.transform.gameObject;
        }
    }
    private void OnTriggerExit(Collider coll)
    {
        target=null;
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
}
