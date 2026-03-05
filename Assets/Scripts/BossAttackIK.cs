using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossAttackIK : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform lookTarget;
    [SerializeField] private Collider attackHitbox;

    [Header("Rangos")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackCooldown = 3f;

    [Header("Daño")]
    private int hitPlayerHP = 1;

    [Header("IK Settings")]
    [Range(0f, 1f)] [SerializeField] private float lookWeight = 1f;
    [SerializeField] private float ikBlendSpeed = 8f;
    [SerializeField] private float ikMoveDuration = 0.5f;  // Tiempo que tarda la mano en llegar

    // Estado interno
    private Animator animator;
    private float cooldownTimer = 0f;
    private float rightHandWeight = 0f;
    private float targetRightWeight = 0f;
    private Vector3 frozenTargetPos;
    private bool ikActive = false;
    private float ikTimer = 0f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (attackHitbox != null)
            attackHitbox.enabled = false;
    }

    void Update()
    {
        if (player == null) return;

        cooldownTimer -= Time.deltaTime;

        float dist = Vector3.Distance(transform.position, player.position);
        bool inRange = dist <= attackRange;

        RotateTowardsPlayer();

        if (inRange && cooldownTimer <= 0f && !ikActive)
        {
            TriggerAttack();
        }

        // Blend suave del IK weight
        rightHandWeight = Mathf.Lerp(rightHandWeight, targetRightWeight, Time.deltaTime * ikBlendSpeed);

        // Contador de duración del IK
        if (ikActive)
        {
            ikTimer -= Time.deltaTime;
            if (ikTimer <= 0f)
            {
                EndIKAndReturnToIdle();
            }
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position);
        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    void TriggerAttack()
    {
        cooldownTimer = attackCooldown;

        // Congela posición del player en este momento
        frozenTargetPos = player.position + Vector3.up * 1.2f;

        animator.SetBool("Idle", false);
        animator.SetTrigger("Right1");
    }

    // ─────────────────────────────────────────────
    // ANIMATION EVENT — un solo evento al final del clip
    // ─────────────────────────────────────────────

    // Llamar este evento en el ÚLTIMO frame del clip de ataque
    public void OnAttackAnimationEnd()
    {
        // Activa hitbox de daño
        if (attackHitbox != null)
            attackHitbox.enabled = true;

        // Activa IK hacia la última posición del player
        ikActive = true;
        targetRightWeight = 1f;
        ikTimer = ikMoveDuration;
    }

    void EndIKAndReturnToIdle()
    {
        // Desactiva IK suavemente
        ikActive = false;
        targetRightWeight = 0f;

        // Desactiva hitbox
        if (attackHitbox != null)
            attackHitbox.enabled = false;

        // Vuelve a Idle
        animator.SetBool("Idle", true);
    }

    // ─────────────────────────────────────────────
    // DAÑO
    // ─────────────────────────────────────────────

    void OnTriggerEnter(Collider other)
    {
        if (attackHitbox == null || !attackHitbox.enabled) return;

        if (player.TryGetComponent(out HealtPlayerController healtPlayer))
        {
            healtPlayer.GetDamage(hitPlayerHP);
            attackHitbox.enabled = false;
        }
            
        
    }

    // ─────────────────────────────────────────────
    // IK
    // ─────────────────────────────────────────────

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        // Look At — siempre mira al player
        if (lookTarget != null)
        {
            animator.SetLookAtWeight(lookWeight, 0.3f, 0.6f, 1f, 0.5f);
            animator.SetLookAtPosition(lookTarget.position);
        }

        // Mano derecha — solo tras la animación de ataque
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);

        if (ikActive || rightHandWeight > 0.01f)
        {
            animator.SetIKPosition(AvatarIKGoal.RightHand, frozenTargetPos);
        }
    }
}