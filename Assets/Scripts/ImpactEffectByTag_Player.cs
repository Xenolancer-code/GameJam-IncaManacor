using System;
using UnityEngine;

public class ImpactEffectByTag_Player : MonoBehaviour
{
    [Header("Efectos de impacto")]
    [SerializeField] private ParticleSystem meleeEffect;
    [SerializeField] private ParticleSystem zoneEffect;


    private void OnTiggerEnter(Collision collision)
    {
        if (collision.contacts.Length == 0) return;

        ContactPoint contact = collision.contacts[0];
        ParticleSystem chosenEffect = GetEffectForTag(collision.gameObject.tag);

        if (chosenEffect != null)
            SpawnEffect(chosenEffect, contact);
    }

    private ParticleSystem GetEffectForTag(string tag)
    {
        return tag switch
        {
            "Player"   => meleeEffect,
            "Zone"  => zoneEffect
        };
    }

    private void SpawnEffect(ParticleSystem prefab, ContactPoint contact)
    {
        GameObject instance = Instantiate(
            prefab.gameObject,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );

        ParticleSystem ps = instance.GetComponent<ParticleSystem>();
        ps.Play();

        
        float lifetime = ps.main.duration + ps.main.startLifetime.constantMax;
        Destroy(instance, lifetime);
    }
    
}
