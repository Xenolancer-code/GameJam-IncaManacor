using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class BossController : MonoBehaviour
{
    [Header("Target")]
    public GameObject target;                        // El player
    
    [Header("Centro Boss")]
    [SerializeField] private Transform centerRange;
    
    [Header("Manos")]
    [SerializeField]private Animator animatorHandRight;
    [SerializeField]private Animator animatorHandLeft;
    [SerializeField] private Transform rightHand;                     // Transform de la mano derecha
    [SerializeField] private Transform leftHand;                      // Transform de la mano izquierda
    [SerializeField] private HandAtk handAtkRight;
    [SerializeField] private HandAtk handAtkLeft;

    [Header("Posiciones de Reposo de Manos")]
    [SerializeField] private Transform rightHandOrigin;               // Punto de reposo mano derecha
    [SerializeField] private Transform leftHandOrigin;                // Punto de reposo mano izquierda

    [Header("Posiciones de Inicio de Ataque")]
   [SerializeField] private Transform rightHandAttackStart;          // Levantado / inicio de manotazo derecho
   [SerializeField] private Transform leftHandAttackStart;           // Levantado / inicio de manotazo izquierdo

    [Header("Posiciones de Inicio de Barrido")]
    [SerializeField] private Transform rightHandSweepStart;           // Inicio de barrido derecho
    [SerializeField] private Transform leftHandSweepStart;            // Inicio de barrido izquierdo

    [Header("Proyectil")]
   [SerializeField] private GameObject projectilePrefab;             // Prefab del proyectil
   [SerializeField] private Transform projectileSpawnPoint;          // Punto de spawn (sobre la cabeza)
   [SerializeField] private float projectileSpeed = 30f;

    // ─────────────────────────────────────────────
    //  AJUSTES DE ATAQUE
    // ─────────────────────────────────────────────

    [Header("Umbrales de distancia")]
    [Tooltip("Distancia mínima al player para usar ataque a distancia")]
    [SerializeField] private float rangedAttackDistance = 30f;

    [Header("Velocidades de movimiento de manos")]
    [SerializeField] private float handMoveSpeed = 20f;                // Velocidad movimiento manos
    [SerializeField] private float handReturnSpeed = 30f;              // Velocidad de vuelta al origen

    [Header("Tiempos de ataque")]
    [SerializeField] private float slapLiftWait = 0.4f;               // Espera tras levantar la mano (manotazo)
    [SerializeField] private float slapHitWait  = 0.3f;               // Espera al golpear (manotazo)

    [SerializeField] private float sweepStartWait = 0.35f;            // Espera en posición inicial de barrido
    [SerializeField] private float sweepSpeed = 8f;                   // Velocidad de barrido

    [SerializeField] private float projectileSpawnWait = 0.6f;        // Pausa antes de disparar proyectil

    [Header("Cooldowns")]
    [SerializeField] private float slapCooldown    = 2.5f;
    [SerializeField] private float sweepCooldown   = 3.5f;
    [SerializeField] private float rangedCooldown  = 4f;
    [SerializeField] private float globalCooldown  = 1f;              // Tiempo mínimo entre ataques


    [Header("K.O. Stats")] 
    [SerializeField] private float durationKO =10f;
    private bool isKO = false;
    
    // ─────────────────────────────────────────────
    //  ESTADO INTERNO
    // ─────────────────────────────────────────────

    private float _slapTimer    = 0f;
    private float _sweepTimer   = 0f;
    private float _rangedTimer  = 0f;
    private float _globalTimer  = 0f;

    private bool _isAttacking   = false;

    private Animator animator;
    private void OnEnable()
    {
        MessageCentral.OnHandDestroyed += BossKO;
        MessageCentral.OnDieBoss += AnimationsHands;
    }
    
    private void OnDisable()
    {
        MessageCentral.OnHandDestroyed -= BossKO;
        MessageCentral.OnDieBoss -= AnimationsHands;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        handAtkRight.enabled = false;
        handAtkLeft.enabled = false;
    }

    // ─────────────────────────────────────────────
    //  UNITY LOOP
    // ─────────────────────────────────────────────
    private void Update()
    {
        if (target == null) return;

        // Actualizar cooldowns
        _slapTimer   = Mathf.Max(0f, _slapTimer   - Time.deltaTime);
        _sweepTimer  = Mathf.Max(0f, _sweepTimer  - Time.deltaTime);
        _rangedTimer = Mathf.Max(0f, _rangedTimer - Time.deltaTime);
        _globalTimer = Mathf.Max(0f, _globalTimer - Time.deltaTime);

        if (!_isAttacking && _globalTimer <= 0f && !isKO)
        {
            DecideAttack();
        }
    }

    // ─────────────────────────────────────────────
    //  DECISIÓN DE ATAQUE
    // ─────────────────────────────────────────────

    private void DecideAttack()
    {
        float distanceToPlayer = Vector3.Distance(centerRange.position, target.transform.position);
        bool playerIsClose     = distanceToPlayer < rangedAttackDistance;
 
        if (playerIsClose)
        {
            // ── RANGO CUERPO A CUERPO: solo manotazo o barrido ──
            bool canSlap  = _slapTimer  <= 0f;
            bool canSweep = _sweepTimer <= 0f;
 
            if (!canSlap && !canSweep) return; // Ambos en cooldown, esperar
 
            if (canSlap && canSweep)
            {
                if (Random.value < 0.5f) StartCoroutine(SlapAttack());
                else                     StartCoroutine(SweepAttack());
            }
            else if (canSlap)  StartCoroutine(SlapAttack());
            else               StartCoroutine(SweepAttack());
        }
        else
        {
            // ── FUERA DE RANGO: solo proyectil ──
            if (_rangedTimer <= 0f)
                StartCoroutine(RangedAttack());
        }
    }

    // ─────────────────────────────────────────────
    //  AYUDAS: SELECCIÓN DE MANO
    // ─────────────────────────────────────────────

    /// <summary>Devuelve la mano más cercana lateralmente al player.</summary>
    private bool UseRightHand()
    {
        // Convertir posición del player al espacio local del boss
        Vector3 localPos = transform.InverseTransformPoint(target.transform.position);
        return localPos.x >= 0f; // Player a la derecha → mano derecha
    }

    // ─────────────────────────────────────────────
    //  UTILIDAD: MOVER TRANSFORM SUAVEMENTE
    // ─────────────────────────────────────────────

    private IEnumerator MoveToTarget(Transform hand, Vector3 destination, float speed)
    {
        while (Vector3.Distance(hand.position, destination) > 0.05f)
        {
            hand.position = Vector3.MoveTowards(hand.position, destination, speed * Time.deltaTime);
            yield return null;
        }
        hand.position = destination;
    }

    // ─────────────────────────────────────────────
    //  ATAQUE 1: MANOTAZO (SLAP)
    // ─────────────────────────────────────────────

    private IEnumerator SlapAttack()
    {
        handAtkRight.enabled = true;
        handAtkLeft.enabled = true;
        _isAttacking = true;
        _slapTimer   = slapCooldown;
        _globalTimer = globalCooldown;

        bool useRight = UseRightHand();

        Transform hand        = useRight ? rightHand        : leftHand;
        Transform origin      = useRight ? rightHandOrigin  : leftHandOrigin;
        Transform attackStart = useRight ? rightHandAttackStart : leftHandAttackStart;

        // 1. Levantar la mano
        yield return MoveToTarget(hand, attackStart.position, handMoveSpeed);

        // 2. Espera con el brazo levantado (el player puede esquivar aquí)
        yield return new WaitForSeconds(slapLiftWait);

        // 3. Capturar posición del player AHORA (tras la espera)
        Vector3 strikePosition = target.transform.position;

        // 4. Golpear hacia la posición capturada
        yield return MoveToTarget(hand, strikePosition, handMoveSpeed * 2f);
        handAtkRight.enabled = false;
        handAtkLeft.enabled = false;
        // 5. Breve pausa al golpear
        yield return new WaitForSeconds(slapHitWait);

        // 6. Volver al origen
        yield return MoveToTarget(hand, origin.position, handReturnSpeed);

        _isAttacking = false;
    }

    // ─────────────────────────────────────────────
    //  ATAQUE 2: BARRIDO (SWEEP)
    // ─────────────────────────────────────────────

    private IEnumerator SweepAttack()
    {
        handAtkRight.enabled = true;
        handAtkLeft.enabled = true;
        _isAttacking = true;
        _sweepTimer  = sweepCooldown;
        _globalTimer = globalCooldown;

        bool useRight = UseRightHand();

        Transform hand       = useRight ? rightHand       : leftHand;
        Transform origin     = useRight ? rightHandOrigin : leftHandOrigin;
        Transform sweepStart = useRight ? rightHandSweepStart : leftHandSweepStart;

        // 1. Mover a posición inicial de barrido
        yield return MoveToTarget(hand, sweepStart.position, handMoveSpeed);

        // 2. Espera telegráfica (el player puede esquivar)
        yield return new WaitForSeconds(sweepStartWait);

        // 3. Capturar posición del player para el barrido
        Vector3 sweepTarget = target.transform.position;

        // 4. Barrido hasta la posición del player
        yield return MoveToTarget(hand, sweepTarget, sweepSpeed);

        // 5. Continuar el arco hasta el otro lado (posición opuesta)
        //    Calculamos un punto "más allá" del player en la misma dirección
        Vector3 sweepDirection = (sweepTarget - sweepStart.position).normalized;
        Vector3 sweepEnd = sweepTarget + sweepDirection * 2f;
        yield return MoveToTarget(hand, sweepEnd, sweepSpeed);

        // 6. Volver al origen
        yield return MoveToTarget(hand, origin.position, handReturnSpeed);
        handAtkRight.enabled = false;
        handAtkLeft.enabled = false;
        _isAttacking = false;
    }

    // ─────────────────────────────────────────────
    //  ATAQUE 3: PROYECTIL (RANGED)
    // ─────────────────────────────────────────────

    private IEnumerator RangedAttack()
    {
        _isAttacking = true;
        _rangedTimer = rangedCooldown;
        _globalTimer = globalCooldown;

        // 1. Hacer aparecer el proyectil sobre la cabeza del boss
        if (projectilePrefab == null || projectileSpawnPoint == null)
        {
            Debug.LogWarning("BossController: Falta el prefab de proyectil o el spawn point.");
            _isAttacking = false;
            yield break;
        }

        GameObject proj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        //AUDIO DE CARGAR ATK
        // Opcional: el proyectil flota un momento (telegráfico)
        AudioManager.I.PlaySound(SoundName.BossCharge,transform.position,1f);
        yield return new WaitForSeconds(projectileSpawnWait);

        // 2. Capturar posición del player y disparar
        //AUDIO DE LANZAR ATK
        AudioManager.I.PlaySound(SoundName.BossShot,transform.position,1f);
        Vector3 directionToPlayer = (target.transform.position - proj.transform.position).normalized;
        Rigidbody rb = proj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = directionToPlayer * projectileSpeed;
        }
        else
        {
            // Sin Rigidbody: adjuntar un mover simple al proyectil
            proj.AddComponent<SimpleProjectileMover>().Init(directionToPlayer, projectileSpeed);
        }

        _isAttacking = false;
    }

    // ─────────────────────────────────────────────
    //  GIZMOS (ayuda visual en el editor)
    // ─────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        // Radio de ataque a distancia
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerRange.position, rangedAttackDistance);

        // Orígenes de manos
        Gizmos.color = Color.cyan;
        if (rightHandOrigin != null)  Gizmos.DrawSphere(rightHandOrigin.position,  0.15f);
        if (leftHandOrigin  != null)  Gizmos.DrawSphere(leftHandOrigin.position,   0.15f);

        // Inicio de ataques
        Gizmos.color = Color.yellow;
        if (rightHandAttackStart != null) Gizmos.DrawSphere(rightHandAttackStart.position, 0.15f);
        if (leftHandAttackStart  != null) Gizmos.DrawSphere(leftHandAttackStart.position,  0.15f);

        // Inicio de barridos
        Gizmos.color = Color.green;
        if (rightHandSweepStart != null) Gizmos.DrawSphere(rightHandSweepStart.position, 0.15f);
        if (leftHandSweepStart  != null) Gizmos.DrawSphere(leftHandSweepStart.position,  0.15f);

        // Spawn proyectil
        Gizmos.color = Color.magenta;
        if (projectileSpawnPoint != null) Gizmos.DrawSphere(projectileSpawnPoint.position, 0.2f);
    }

// ─────────────────────────────────────────────────────────────────────────────
//                              KO
// ─────────────────────────────────────────────────────────────────────────────
    private void BossKO()
    {
        animator.SetBool("KO",true);
        isKO=true;
        StartCoroutine(KOduration());
    }

    private IEnumerator KOduration()
    {
        yield return new WaitForSeconds(durationKO);
        animator.SetBool("KO",false);
        isKO=false;
        MessageCentral.HandHeal();
    }

    private void AnimationsHands()
    {
        animatorHandLeft.enabled = true;
        animatorHandRight.enabled = true;
        animatorHandLeft.SetTrigger("Defeat");
        animatorHandRight.SetTrigger("Defeat");
    }
}


// ─────────────────────────────────────────────────────────────────────────────
//  COMPONENTE AUXILIAR: Mover proyectil sin Rigidbody
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Se añade dinámicamente al proyectil si no tiene Rigidbody.
/// Puedes eliminarlo si tus proyectiles siempre usan Rigidbody.
/// </summary>
public class SimpleProjectileMover : MonoBehaviour
{
    private Vector3 _direction;
    private float   _speed;

    public void Init(Vector3 direction, float speed)
    {
        _direction = direction;
        _speed     = speed;
    }

    private void Update()
    {
        transform.position += _direction * _speed * Time.deltaTime;
    }
}