using Unity.VisualScripting;
using UnityEngine;
public class PlayerAtk : MonoBehaviour
{
    //private CharacterController cc;
    private Animator animator;
    public float aoeRadius = 4f;
    public float knockbackDistance = 3f;
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
    }
   
    public void RightClickPressed()
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
}
