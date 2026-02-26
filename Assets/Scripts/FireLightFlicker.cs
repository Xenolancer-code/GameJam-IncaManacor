using UnityEngine;

[RequireComponent(typeof(Light))]
public class FireLightFlicker : MonoBehaviour
{
    [Header("Intensidad")]
    public float baseIntensity = 1.2f;
    public float intensityVariation = 0.4f;
    public float flickerSpeed = 2f;

    [Header("Color")]
    public bool colorFlicker = true;
    public Color baseColor = new Color(1f, 0.5f, 0.2f);
    public float colorVariation = 0.1f;

    private Light fireLight;
    private float randomOffset;

    void Start()
    {
        fireLight = GetComponent<Light>();
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, randomOffset);

        // Intensidad
        fireLight.intensity = baseIntensity + 
                              Mathf.Lerp(-intensityVariation, intensityVariation, noise);

        // Color (opcional)
        if (colorFlicker)
        {
            float colorNoise = Mathf.PerlinNoise(Time.time * flickerSpeed * 1.3f, randomOffset + 50f);
            fireLight.color = baseColor + 
                              new Color(colorNoise * colorVariation, 0, 0);
        }
    }
}