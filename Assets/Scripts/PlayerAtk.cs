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

    // [SerializeField]
    // private BossHealtController bossHealtController;
    
    [Header("Damage")]
    public int finalDamage; //Crear Maximo dps de 30 o 25
    public int finalRange;
    //[SerializeField] private float damageAmount; - Manejado por GameManager
    
    [Header("Aoe")]
    //[SerializeField] public float aoeRadius = 4f; - Manejado por prefab instanciado
    [SerializeField] private GameObject zone;
    public bool canAoe = false;
    
    //--Combo State--
    private bool isAttacking = false;
    public bool basicAttackPerformed = false;
    private Animator animator;
    private CharacterController cc;
    private PlayerMov playerMov;
    


    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        playerMov = GetComponent<PlayerMov>();
    }
    
    public void BasicAtk(bool performed)
    {
        basicAttackPerformed  = performed;
    }
  
    public void SetCanInterruptTrue()
    { 
        animator.SetBool("canInterrupt", true);   
    }
    
    public void DmgBasicAtk() //Llamado por Event en animation
    {
        AudioManager.I.PlaySound(SoundName.SlashPlayer,transform);//Sonido de SoundLibrary
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

        // Aqui tenim la llista de impactes ordenada
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
            if (enemy.TryGetComponent(out HandHealth hahe))
            {
                hahe.TakeDamage(finalDamage);
                hitIndex++;
            }
            else
            {
                //TODO: 2- Mirar component BossHealthController en els pares i si el troba...
                var bossHealtController = enemy.GetComponentInParent<BossHealtController>();
                if (bossHealtController != null)
                {
                    bossHealtController.TakeDamageBoss(finalDamage);
                    hitIndex++;
                }
            }
            
            //TODO: 1- COMPROVAR QUE AIXO SIGUI UN CAP PER NOM DE GAMEOBJECT O TAG. Si ho és usar bossHealthController

            
        }
    }
    public void StartAnimation() //Llamado por Event en animation
    {
        isAttacking = true;
        if (playerMov != null)
        {
            playerMov.SetMovementLocked(true);
            Debug.Log("Movimiento bloqueado");
            animator.SetBool("canInterrupt", false);
        }
    }
    public void EndAnimation() //Llamado por Event en animation
    {
        isAttacking = false;
        if (playerMov != null)
        {
            playerMov.SetMovementLocked(false);
            Debug.Log("Movimiento desbloqueado");
        }
    }
    // ----------------------
    //  AOE (sin cambios)
    // ----------------------
    public void AoEAtk()
    {
        animator.SetTrigger("RightClick");
        canAoe = false;
    }

    private void AoeDamageZone()
    {
        Instantiate(zone, transform.position, Quaternion.identity);
    }

    // ----------------------
    //  CLASES AUXILIARES
    // ----------------------
    public class EnemyDistance
    {
        //Clase per poder fer una llista i aixi ordenar els enemics i la seva distancia sobre el player
        public GameObject target;
        public float distance;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
