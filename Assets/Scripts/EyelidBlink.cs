using UnityEngine;

public class EyelidBlink : MonoBehaviour
{
    [Header("Eyelids")]
    public Transform upperEyelidLeft;
    public Transform upperEyelidRight;
    public Transform lowerEyelidLeft;
    public Transform lowerEyelidRight;

    [Header("Blink Rotation (eje local)")]
    public Vector3 upperClosedRotation = new Vector3(30f, 0f, 0f);
    public Vector3 lowerClosedRotation = new Vector3(-30f, 0f, 0f);

    [Header("Timing")]
    public float blinkSpeed = 8f;
    [Range(1f, 10f)] public float minInterval = 2f;
    [Range(1f, 10f)] public float maxInterval = 5f;

    [Header("Randomness")]
    [Range(0f, 1f)] public float doubleBlink = 0.2f;
    [Range(0f, 1f)] public float halfBlink = 0.15f;

    private Quaternion upperLeftOpen, upperRightOpen;
    private Quaternion lowerLeftOpen, lowerRightOpen;
    private Quaternion upperLeftClosed, upperRightClosed;
    private Quaternion lowerLeftClosed, lowerRightClosed;

    private float blinkTimer;
    private float nextBlink;
    private bool isBlinking;
    private float blinkT;
    private bool blinkClosing;
    private float closedHoldTime = 0.05f;
    private float closedTimer;
    private float currentBlinkAmount = 1f;

    private int pendingBlinks = 0;

    void Start()
    {
        // Guardar rotaciones abiertas (estado inicial)
        if (upperEyelidLeft)  upperLeftOpen  = upperEyelidLeft.localRotation;
        if (upperEyelidRight) upperRightOpen = upperEyelidRight.localRotation;
        if (lowerEyelidLeft)  lowerLeftOpen  = lowerEyelidLeft.localRotation;
        if (lowerEyelidRight) lowerRightOpen = lowerEyelidRight.localRotation;

        // Calcular rotaciones cerradas
        if (upperEyelidLeft)  upperLeftClosed  = upperLeftOpen  * Quaternion.Euler(upperClosedRotation);
        if (upperEyelidRight) upperRightClosed = upperRightOpen * Quaternion.Euler(upperClosedRotation);
        if (lowerEyelidLeft)  lowerLeftClosed  = lowerLeftOpen  * Quaternion.Euler(lowerClosedRotation);
        if (lowerEyelidRight) lowerRightClosed = lowerRightOpen * Quaternion.Euler(lowerClosedRotation);

        ScheduleNextBlink();
    }

    void Update()
    {
        blinkTimer += Time.deltaTime;

        if (!isBlinking && blinkTimer >= nextBlink)
        {
            StartBlink();
        }

        if (isBlinking)
        {
            AnimateBlink();
        }
    }

    void ScheduleNextBlink()
    {
        blinkTimer = 0f;
        nextBlink = Random.Range(minInterval, maxInterval);
    }

    void StartBlink()
    {
        isBlinking = true;
        blinkClosing = true;
        blinkT = 0f;
        closedTimer = 0f;

        // Decidir tipo de parpadeo
        float roll = Random.value;
        if (roll < halfBlink)
            currentBlinkAmount = Random.Range(0.3f, 0.6f); // medio parpadeo
        else
            currentBlinkAmount = 1f;

        // Double blink: programar un segundo parpadeo
        if (Random.value < doubleBlink)
            pendingBlinks = 1;
    }

    void AnimateBlink()
    {
        float speed = blinkSpeed * Time.deltaTime;

        if (blinkClosing)
        {
            blinkT += speed;
            if (blinkT >= 1f)
            {
                blinkT = 1f;
                blinkClosing = false;
                closedTimer = 0f;
            }
        }
        else
        {
            // Pequeña pausa con el ojo cerrado
            closedTimer += Time.deltaTime;
            if (closedTimer >= closedHoldTime)
            {
                blinkT -= speed;
                if (blinkT <= 0f)
                {
                    blinkT = 0f;
                    isBlinking = false;
                    ApplyBlink(0f);

                    if (pendingBlinks > 0)
                    {
                        pendingBlinks--;
                        Invoke(nameof(StartBlink), Random.Range(0.08f, 0.15f));
                    }
                    else
                    {
                        ScheduleNextBlink();
                    }
                    return;
                }
            }
        }

        ApplyBlink(blinkT * currentBlinkAmount);
    }

    void ApplyBlink(float t)
    {
        float smooth = Mathf.SmoothStep(0f, 1f, t);

        if (upperEyelidLeft)
            upperEyelidLeft.localRotation  = Quaternion.Slerp(upperLeftOpen,  upperLeftClosed,  smooth);
        if (upperEyelidRight)
            upperEyelidRight.localRotation = Quaternion.Slerp(upperRightOpen, upperRightClosed, smooth);
        if (lowerEyelidLeft)
            lowerEyelidLeft.localRotation  = Quaternion.Slerp(lowerLeftOpen,  lowerLeftClosed,  smooth);
        if (lowerEyelidRight)
            lowerEyelidRight.localRotation   = Quaternion.Slerp(lowerRightOpen, lowerRightClosed, smooth);
    }

    // Permite forzar un parpadeo desde otro script
    public void ForceBlink() => StartBlink();
}