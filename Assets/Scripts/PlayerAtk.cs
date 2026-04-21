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
    [SerializeField] private GameObject impacateffects;
    
    private bool pausedGame = false;
    
    [Header("Damage")]
    public int finalDamage; //Crear Maximo dps de 30 o 25
    public int finalRange;
    //[SerializeField] private float damageAmount; - Manejado por GameManager
    
    [Header("Aoe")]
    //[SerializeField] public float aoeRadius = 4f; - Manejado por prefab instanciado
    [SerializeField] private GameObject zone;
    public bool canAoe = false;
    
    //--Combo State--
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
    private void OnEnable()
    {
        MessageCentral.OnPausedGame += IsGamePaused;
    }
    private void OnDisable()
    {
        MessageCentral.OnPausedGame -= IsGamePaused;
    }


    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        playerMov = GetComponent<PlayerMov>();
    }
    
    public void BasicAtk(bool performed)
    {
        if(pausedGame) return;
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
        List<EnemyHitInfo> closeEnemies = new List<EnemyHitInfo>();

        foreach (Collider collEnemy in collidedEnemies)
        {
            var go = collEnemy.gameObject;
            
            EnemyHitInfo enemyHitInfo = new EnemyHitInfo();
            enemyHitInfo.target = go; //Deim que els targets son tots els gameobjects dins l'Array de Colliders
            enemyHitInfo.distance = Vector3.Distance(attackPoint.position, go.transform.position);//Sabem la distancia entre el player i els enemics
            enemyHitInfo.hitPoint = collEnemy.ClosestPoint(attackPoint.position);
            closeEnemies.Add(enemyHitInfo);// Afagim els datos dins la llista
        }

        // Aqui tenim la llista de impactes ordenada
        closeEnemies.Sort((a, b) => a.distance.CompareTo(b.distance));
        int hitIndex = 0;
        for(int i = 0; i<closeEnemies.Count && hitIndex < maxSimultaneousHits; i++)
        {
            EnemyHitInfo hitInfo = closeEnemies[i];
            IDamageable damageable = GetDamageable(hitInfo.target);
            if (damageable != null)
            {
                Instantiate(impacateffects,hitInfo.hitPoint, Quaternion.identity);
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
    
    public void StartAnimation(int fase) //Llamado por Event en animation
    {
        if (playerMov != null)
        {
            playerMov.SetMovementLocked(true);
            Debug.Log("Movimiento bloqueado " + fase);
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
    
    public void EndAnimation(int fase) //Llamado por Event en animation
    {
        if (playerMov != null)
        {
            playerMov.SetMovementLocked(false);
            Debug.Log("Movimiento desbloqueado " + fase);
        }
    }
    // ----------------------
    //  AOE (sin cambios)
    // ----------------------
    public void AoEAtk()
    {
        if(pausedGame)return;
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
    
    public class EnemyHitInfo
    {
        //Clase per poder fer una llista i aixi ordenar els enemics i la seva distancia sobre el player
        public GameObject target;
        public float distance;
        public Vector3 hitPoint;
    }

    private void IsGamePaused(bool paused)
    {
        pausedGame = paused;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    //-------------------------------------------------
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
