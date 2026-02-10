using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerModeSwap : MonoBehaviour
{
    [SerializeField] private Material materialLuz;
    [SerializeField] private Material materialOscuro;
    private PlayerMov playerMov;
    [SerializeField] private float altura;
    [SerializeField] private float duracionImpulso;
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
        }
        StartCoroutine(PortalImpulse());
    }
    //Corutina aqui
    private IEnumerator PortalImpulse()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * altura;
        float timer = 0f;

        //Impulso (rápido al inicio, suave al final)
        while (timer < duracionImpulso)
        {
            timer += Time.deltaTime;
            float interpolator = timer / duracionImpulso;
            transform.position = Vector3.Lerp(startPos, endPos, interpolator);
            yield return null;
        }
        
        transform.position = endPos;
        //yield return new WaitForSeconds();
    }
}
