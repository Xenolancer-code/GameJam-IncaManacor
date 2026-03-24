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
    
    [Header("Attack Visuals")]
    [SerializeField] private GameObject spectralStaff;
    [SerializeField] private float spectralFadeInDuration = 0.5f;
    [SerializeField] private float spectralFadeOutDuration = 0.3f;
    
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
    
    //Spectral visuals
    private Coroutine spectralStaffEmissionRoutine;
    private Color spectralStaffOriginalEmissionColor;
    private Color spectralStaffOriginalEmissionBaseColor;
    private float spectralStaffOriginalEmissionIntensity;
    private bool spectralStaffEmissionCached;
    


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
        //AudioManager.I.PlaySound(SoundName.SlashPlayer,transform);//Sonido de SoundLibrary
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
            IDamageable damageable = GetDamageable(closeEnemies[i].target);
            if (damageable != null)
            {
                damageable.GetDamage(finalDamage);
                hitIndex++;
            }
            
        }
    }
    private IDamageable GetDamageable(GameObject other)
    {
        return other.GetComponent<HandHealth>()
               ?? other.GetComponentInParent<BossHealtController>()
               ?? other.GetComponent<HealthEnemyController>() as IDamageable;
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

    public void StartVisuals()
    {
        if (spectralStaff != null)
        {
            spectralStaff.SetActive(true);

            if (spectralStaffEmissionRoutine != null)
            {
                StopCoroutine(spectralStaffEmissionRoutine);
            }

            spectralStaffEmissionRoutine = StartCoroutine(LerpSpectralStaffEmission());
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

    private IEnumerator LerpSpectralStaffEmission()
    {
        MeshRenderer meshRenderer = spectralStaff.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            spectralStaffEmissionRoutine = null;
            yield break;
        }

        Material material = meshRenderer.material;
        if (!material.HasProperty("_EmissionColor"))
        {
            spectralStaffEmissionRoutine = null;
            yield break;
        }

        if (!spectralStaffEmissionCached)
        {
            spectralStaffOriginalEmissionColor = material.GetColor("_EmissionColor");
            spectralStaffOriginalEmissionIntensity = Mathf.Max(
                spectralStaffOriginalEmissionColor.r,
                Mathf.Max(spectralStaffOriginalEmissionColor.g, spectralStaffOriginalEmissionColor.b)
            );
            spectralStaffOriginalEmissionBaseColor = spectralStaffOriginalEmissionIntensity > 0f
                ? spectralStaffOriginalEmissionColor / spectralStaffOriginalEmissionIntensity
                : Color.black;
            spectralStaffEmissionCached = true;
        }

        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", spectralStaffOriginalEmissionBaseColor * 0f);

        float elapsed = 0f;
        while (elapsed < spectralFadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / spectralFadeInDuration);
            float intensity = Mathf.Lerp(0f, spectralStaffOriginalEmissionIntensity, t);
            material.SetColor("_EmissionColor", spectralStaffOriginalEmissionBaseColor * intensity);

            yield return null;
        }

        material.SetColor("_EmissionColor", spectralStaffOriginalEmissionColor);

        elapsed = 0f;
        while (elapsed < spectralFadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / spectralFadeOutDuration);
            float intensity = Mathf.Lerp(spectralStaffOriginalEmissionIntensity, 0f, t);
            material.SetColor("_EmissionColor", spectralStaffOriginalEmissionBaseColor * intensity);

            yield return null;
        }

        material.SetColor("_EmissionColor", spectralStaffOriginalEmissionBaseColor * 0f);
        spectralStaff.SetActive(false);

        spectralStaffEmissionRoutine = null;
    }
}
