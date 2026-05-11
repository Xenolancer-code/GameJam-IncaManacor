using System;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    //[SerializeField] private int quality =1;
    private void OnEnable()
    {
        MessageCentral.OnSwapScene += CleanSceneOfDrops;
    }

    private void OnDisable()
    {
        MessageCentral.OnSwapScene -= CleanSceneOfDrops;
    }

    private void OnTriggerEnter(Collider detect)
    {
        if (detect.CompareTag("Player"))
        {
            MessageCentral.PickupSample(20);
            PoolManager.ReturnObjectToPool(gameObject);
            AudioManager.I.PlaySound(SoundName.DropsSounds,transform.position,1f);
        }
    }

    private void CleanSceneOfDrops()
    {
        PoolManager.ReturnObjectToPool(gameObject);
    }
}
