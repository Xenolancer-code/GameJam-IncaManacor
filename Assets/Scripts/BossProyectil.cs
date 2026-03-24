using UnityEngine;

public class BossProyectil : MonoBehaviour
{
    private int hitPlayerHP = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.TryGetComponent(out HealtPlayerController healtPlayer))
            {
                healtPlayer.GetDamage(hitPlayerHP);
                Destroy(gameObject);
            }
        }
    }
}
