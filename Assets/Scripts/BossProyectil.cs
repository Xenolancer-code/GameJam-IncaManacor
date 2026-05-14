using System;
using System.Collections;
using UnityEngine;

public class BossProyectil : MonoBehaviour
{
    [SerializeField] private float groundY = 0f;
    [Header("Daño")]
    [SerializeField] private int hitPlayerHP = 1;

    [Header("Explosión")]
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private float lifetime = 8f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (transform.position.y <= groundY)
        {
            Explode();
        }
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
    }

    private void Explode()
    {
        if (explosionVFX != null)
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        AudioManager.I.PlaySound(SoundName.ExplosionBoss, transform.position,1f);
        Destroy(gameObject);
    }
}