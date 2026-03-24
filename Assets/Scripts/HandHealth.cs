using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class HandHealth : MonoBehaviour, IDamageable
{
    [Header("Canvas")]
    [SerializeField] private Canvas handCanvas;                   // Canvas de esta mano (World Space)

    [Header("Imágenes de la barra")]
    [SerializeField] private RectTransform backgroundImage;       // Image negra (fondo)
    [SerializeField] private RectTransform healthImage;           // Image roja  (vida actual)
    [SerializeField] private RectTransform hitImage;              // Image blanca (daño recibido)
    

    // ─────────────────────────────────────────────
    //  STATS
    // ─────────────────────────────────────────────

    [Header("Vida")]
    [SerializeField] private float maxHealth      = 100f;
    [SerializeField] private float currentHealth  = 100f;

    [Header("Barra")]
    [Tooltip("Anchura máxima de la barra en unidades UI (debe coincidir con el width inicial en el Inspector)")]
    [SerializeField] private float barMaxWidth    = 100f;

    // ─────────────────────────────────────────────
    //  EFECTO DE GOLPE (hit flash)
    // ─────────────────────────────────────────────
    
    [Header("Efecto de golpe")]
    [Tooltip("Tiempo que la barra blanca permanece visible antes de reducirse")]
    private Collider collider;
    [SerializeField] private float hitHoldDuration    = 0.3f;

    [Tooltip("Tiempo que tarda la barra blanca en reducirse hasta la vida actual")]
    [SerializeField] private float hitDrainDuration   = 0.4f;

    [SerializeField] private Material materialOriginal;
    [SerializeField] private Material materialDañoRecibido;
    private SkinnedMeshRenderer smr;

    // ─────────────────────────────────────────────
    //  CÁMARA (billboard)
    // ─────────────────────────────────────────────

    private Camera _mainCamera;

    // ─────────────────────────────────────────────
    //  ESTADO INTERNO
    // ─────────────────────────────────────────────

    private float _hitBarTargetWidth;   // Ancho al que tiene que llegar la barra blanca
    private Coroutine _hitCoroutine;

    // ─────────────────────────────────────────────
    //  UNITY LOOP
    // ─────────────────────────────────────────────

    private void OnEnable()
    {
        MessageCentral.OnHandHeal += Heal;
    }
    
    private void OnDisable()
    {
        MessageCentral.OnHandHeal -= Heal;
    }
    private void Awake()
    {
        smr = GetComponentInChildren<SkinnedMeshRenderer>();
        _mainCamera = Camera.main;
        collider = GetComponent<Collider>();
        // Inicializar barras al máximo
        SetBarWidth(healthImage, barMaxWidth);
        SetBarWidth(hitImage,    barMaxWidth);
        SetBarWidth(backgroundImage, barMaxWidth);
    }

    private void LateUpdate()
    {
        // Billboard: el canvas siempre mira a la cámara
        if (handCanvas != null && _mainCamera != null)
        {
            handCanvas.transform.LookAt(
                handCanvas.transform.position + _mainCamera.transform.rotation * Vector3.forward,
                _mainCamera.transform.rotation * Vector3.up
            );
        }
    }

    // ─────────────────────────────────────────────
    //  API PÚBLICA
    // ─────────────────────────────────────────────

    /// <summary>Aplica daño a la mano.</summary>
    public void GetDamage(float damage)
    {
        if (currentHealth <= 0f) return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);

        float newHealthWidth = HealthToWidth(currentHealth);

        // 1. Actualizar barra roja inmediatamente
        SetBarWidth(healthImage, newHealthWidth);
        EjecutarCambioMaterial();
        // 2. La barra blanca QUEDA donde estaba y se drena hasta la vida actual
        //    (si hay una animación en curso, la reiniciamos con el nuevo valor)
        _hitBarTargetWidth = newHealthWidth;

        if (_hitCoroutine != null)
            StopCoroutine(_hitCoroutine);

        _hitCoroutine = StartCoroutine(HitEffect());

        if (currentHealth <= 0f)
            OnHandDestroyed();
    }

    /// <summary>Cura la mano.</summary>
    public void Heal()//float amount
    {
        currentHealth = maxHealth; //Mathf.Min(maxHealth, currentHealth + amount);
        float newWidth = HealthToWidth(currentHealth);
        SetBarWidth(healthImage, newWidth);
        SetBarWidth(hitImage,    newWidth);
    }

    // ─────────────────────────────────────────────
    //  EFECTO VISUAL DEL GOLPE
    // ─────────────────────────────────────────────

    private IEnumerator HitEffect()
    {
        // La barra blanca ya está en su posición ANTERIOR (mayor que la roja)
        // Solo hay que esperar y luego drenarla.

        yield return new WaitForSeconds(hitHoldDuration);

        // Drena la barra blanca hasta el ancho de la vida actual
        float startWidth = GetBarWidth(hitImage);
        float elapsed    = 0f;

        while (elapsed < hitDrainDuration)
        {
            elapsed += Time.deltaTime;
            float t  = Mathf.Clamp01(elapsed / hitDrainDuration);
            float w  = Mathf.Lerp(startWidth, _hitBarTargetWidth, t);
            SetBarWidth(hitImage, w);
            yield return null;
        }

        SetBarWidth(hitImage, _hitBarTargetWidth);
        _hitCoroutine = null;
    }

    private void EjecutarCambioMaterial()
    {
        if(smr ==null) return;
        StartCoroutine(CambiarYRevertir());

    }

    private IEnumerator CambiarYRevertir()
    {
        smr.material = materialDañoRecibido;
        yield return new WaitForSeconds(0.5f);
        smr.material = materialOriginal;
    }

    // ─────────────────────────────────────────────
    //  DESTRUCCIÓN
    // ─────────────────────────────────────────────

    private void OnHandDestroyed()
    {
        Debug.Log($"{gameObject.name}: mano destruida.");
        MessageCentral.HandDestroyed();
        // Añade aquí tu lógica: animación de muerte, desactivar colisionador, notificar al boss…
    }

    // ─────────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────────

    private float HealthToWidth(float health)
    {
        return (health / maxHealth) * barMaxWidth;
    }

    private void SetBarWidth(RectTransform rt, float width)
    {
        if (rt == null) return;
        Vector2 size = rt.sizeDelta;
        size.x       = width;
        rt.sizeDelta = size;
    }

    private float GetBarWidth(RectTransform rt)
    {
        return rt != null ? rt.sizeDelta.x : 0f;
    }
}