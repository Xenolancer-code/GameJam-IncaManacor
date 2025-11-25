using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerAtk : MonoBehaviour
{
    //private CharacterController cc;
    [Header("Attack Controller")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float maxSimultaneousHits =2f;
    [Header("Damage Amount Controller")]
    [SerializeField] private float damageAmount;
    [Header("Knockback Controller")]
    [SerializeField] public float aoeRadius = 4f;
    [SerializeField] public float knockbackDistance = 8f;
    private Animator animator;
    


    private void Awake()
    {
        //cc = gameObject.GetComponent<CharacterController>();
    }
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private class EnemyDistance:Comparer<EnemyDistance>
    {
        public GameObject target;
        public float distance;

        public override int Compare(EnemyDistance x, EnemyDistance y)
        {
            if (x.distance < y.distance)
                return -1;
            else if (x.distance > y.distance)
                return 1;
            else return 0;
        }
    }

    public void BasicAtk()
    {
        animator.SetTrigger("LeftClick");
        Debug.Log("Estoy atacando al enemigo");
        var collidedEnemies = Physics.OverlapSphere(attackPoint.position, attackRadius, enemyLayer);
        if (collidedEnemies == null) return;
        List<EnemyDistance> closeEnemies = new List<EnemyDistance>();

        foreach (Collider collEnemy in collidedEnemies){
            var go = collEnemy.gameObject;

            EnemyDistance enemyDistance = new EnemyDistance();
            enemyDistance.target = go;
            enemyDistance.distance = Vector3.Distance(attackPoint.position, go.transform.position);

            closeEnemies.Add(enemyDistance);

            //comprovar distancia go sigui mes petita que closer distance
            // si es mes petita closerdistance = nova distancia
            // closerEnemy és aquest enemy   
        }

        // Aquí tenim la llista de impactes ordenada
        closeEnemies.Sort();
        int hitIndex = 0;
        for(int i = 0; i < closeEnemies.Count; i++)
        {
            if (hitIndex < maxSimultaneousHits){
                var enemy = closeEnemies[i].target;
                if (enemy.TryGetComponent(out HealthEnemyController healthcontroller))
                {
                    //Hacer maxSimultaneousHits  a los que esten mas cerca del punto [list]
                    healthcontroller.GetDamage(damageAmount);
                    hitIndex++;
                }
            }
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
