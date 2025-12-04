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
    [SerializeField] private int maxSimultaneousHits =2;
    [Header("Damage Amount Controller")]
    [SerializeField] private float damageAmount;
    [Header("Knockback Controller")]
    [SerializeField] public float aoeRadius = 4f;
    [SerializeField] public float knockbackDistance = 8f;
    private Animator animator;
    public int finalDamage; //Crear Maximo 30 o 25
    public int finalRange;


    private void Awake()
    {
        //cc = gameObject.GetComponent<CharacterController>();
    }
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public class EnemyDistance
    {   //Clase per poder fer una llista i aixi ordenar els enemics i la seva distancia sobre el player
        public GameObject target;
        public float distance;
    }

    public void BasicAtk()
    {
        animator.SetTrigger("LeftClick");
        Debug.Log("Estoy atacando al enemigo");
        var collidedEnemies = Physics.OverlapSphere(attackPoint.position, attackRadius, enemyLayer);
        if (collidedEnemies == null) return;
        //Llista que guarda la distancia del enemics sobre el player
        List<EnemyDistance> closeEnemies = new List<EnemyDistance>();

        foreach (Collider collEnemy in collidedEnemies)
        {
            var go = collEnemy.gameObject;

            EnemyDistance enemyDistance = new EnemyDistance();
            enemyDistance.target = go; //Deim que els targets son tots els gameobjects dins l'Array de Colliders
            enemyDistance.distance = Vector3.Distance(attackPoint.position, go.transform.position);//Sabem la distancia entre el player i els enemics
            closeEnemies.Add(enemyDistance);// Afagim els datos dins la llista
        }

    // Aquí tenim la llista de impactes ordenada
    closeEnemies.Sort((a, b) => a.distance.CompareTo(b.distance));
        int hitIndex = 0;
        for(int i = 0; i<closeEnemies.Count && hitIndex < maxSimultaneousHits; i++)
        {
            var enemy = closeEnemies[i].target;
            if (enemy.TryGetComponent(out HealthEnemyController healthcontroller))
            {
                healthcontroller.GetDamage(finalDamage);
                hitIndex++;
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
                EnemyMov kb = hit.GetComponent<EnemyMov>();
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
