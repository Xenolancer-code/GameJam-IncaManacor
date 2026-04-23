
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Impulse Source")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    
    [Header("Shake Settings")]
    [SerializeField] private float shakeAmplitud=1f;
    
    [Header("Phase Durations")]
    [SerializeField] private float initialShakeDuration=0.5f;
    [SerializeField] private float PostInitialshackeDuration=1.5f;
    
    [Header("Decay Curve")]
    [Tooltip("X = tiempo normalizado (0-1), Y = intensidad (0-1)")]
    [SerializeField] private AnimationCurve decayCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    
    private Coroutine _shakeCoroutine;
    
    private void OnEnable()
    {
        MessageCentral.OnDamagedPlayer += Shake;
    }

    private void OnDisable()
    {
        MessageCentral.OnDamagedPlayer -= Shake;
    }

    private void Shake(bool playerIsDamaged)
    {
        if(playerIsDamaged) FireImpulse(shakeAmplitud);
    //     if (playerIsDamaged)
    //     {
    //         if(_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);
    //         _shakeCoroutine = StartCoroutine(ShakeCoroutine());
    //     }
    //     else
    //     {
    //         if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);
    //     }
     }
    //
    // private IEnumerator ShakeCoroutine()
    // {
    //     //Fase inicial
    //     float elapsed = 0f;
    //     while (elapsed < initialShakeDuration)
    //     {
    //         FireImpulse(shakeAmplitud);
    //         elapsed += Time.deltaTime;
    //         yield return null;
    //     }
    //     //Post Fase inicial
    //     elapsed = 0f;
    //     while (elapsed < PostInitialshackeDuration)
    //     {
    //         float t = elapsed / PostInitialshackeDuration;
    //         float intensity = decayCurve.Evaluate(t)*shakeAmplitud;
    //         FireImpulse(intensity);
    //         elapsed += Time.deltaTime;
    //         yield return null;
    //     }
    //
    //     _shakeCoroutine = null;
    // }
    //
    //
    //
    private void FireImpulse(float amplitude)
    {
        Vector3 velocity = Random.insideUnitSphere * amplitude;
        impulseSource.GenerateImpulseWithVelocity(velocity);
    }

}
