using UnityEngine;

public class BossProyectil : MonoBehaviour
{
    [Header("Daño")]
    [SerializeField] private int hitPlayerHP = 1;

    [Header("Explosión")]
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private float lifetime = 8f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.TryGetComponent(out HealtPlayerController healtPlayer))
            {
                healtPlayer.GetDamage(hitPlayerHP);
                Explode();
            }
        }
        else if (other.CompareTag("Ground"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (explosionVFX != null)
            Instantiate(explosionVFX, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}