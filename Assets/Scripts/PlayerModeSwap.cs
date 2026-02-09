using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerModeSwap : MonoBehaviour
{
    [SerializeField] private Material materialLuz;
    [SerializeField] private Material materialOscuro;
    private PlayerMov playerMov;
    private void Awake()
    {

    playerMov = GetComponent<PlayerMov>();
}

    private void OnEnable()
    {
        MessageCentral.OnSwapScene += ChangeTexture;
    }

    private void OnDisable()
    {
        MessageCentral.OnSwapScene -= ChangeTexture;
    }


    private void ChangeTexture()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            
            if (r.material.name.StartsWith(materialOscuro.name))
            {
                r.material = materialLuz;
            }
            else
            {
                r.material = materialOscuro;
            }
            playerMov.SetGravity(0);
        }
    }
}
