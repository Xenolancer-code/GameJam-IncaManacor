using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
    
    [SerializeField] private GameObject player;
    [Header("GameObjects a activar")]
    [SerializeField] private GameObject zonaOscura;
    [SerializeField] private GameObject zonaLuz;
    [Header("Rotation")]
    [SerializeField] private GameObject collectionScene;
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private float cooldown = 10f;

    private bool canActivate = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!canActivate) return;

       
        player.SetActive(false);
        StartCoroutine(RotatePlane());
    }

    private IEnumerator RotatePlane()
    {
        canActivate = false;

        Quaternion start = collectionScene.transform.rotation;
        Quaternion end = start * Quaternion.Euler(180, 0, 0);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            collectionScene.transform.rotation = Quaternion.Slerp(start, end, t);
            yield return null;
        }
        zonaLuz.SetActive(true);
        zonaOscura.SetActive(false);
        player.SetActive(true);
        MessageCentral.SwapScene();
        Debug.Log("Missatge enviat");
        AudioSource backgroundMusic = AudioManager.I?.StopBackgroundMusic();
        AudioManager.I?.PlayBackgroundSounds(SoundName.GameMusic2, ref backgroundMusic);
        
        // yield return new WaitForSeconds(cooldown);
        // canActivate = true;
    }
}