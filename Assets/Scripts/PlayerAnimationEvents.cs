using UnityEngine;
using UnityEngine.Audio;

public class PlayerAnimationEvents : MonoBehaviour
{
    // Referencia al PlayerAtk del padre
    private PlayerAtk atk;
    private AudioSource audioSource;

    private void Awake()
    {
        atk = GetComponentInParent<PlayerAtk>();
        audioSource = GetComponent<AudioSource>();
    }
 
    public void DoAoEKnockback()
    {
        atk.DoAoEKnockback();
    }
    // Nuevo: reproducir sonido
    public void PlayShoutSound()
    {
        Debug.Log("Reproduciendo sonido...");
        if (audioSource != null)
            audioSource.Play();
    }
}
