using System.Collections;
using UnityEngine;

public class LuzSpawners : MonoBehaviour
{
    [SerializeField] private float awakeTime = 5f;
    private void OnEnable()
    {
        MessageCentral.OnSwapScene += CorutinaSpawnerLuz;
        
    }

    private void OnDisable()
    {
        MessageCentral.OnSwapScene -= CorutinaSpawnerLuz;
        
    }
    void Start()
    {
        /* TODO
         -Iniciar spawers X tiempo luego de que el player aparezca en plano de Luz
         -Tener la misma función de cantidad maxima de enemigos pero solo aparecen en 1 punto
         -Estos no se destruyen sino se apagan al termianr el juego
         -Cuando termina el juego tambíen se destruyen todos los enemigos
         -Y efectos visuales y/o animaciones
         */
    }

    
    void Update()
    {
        
    }

    private void CorutinaSpawnerLuz()
    {
        StartCoroutine(AwakeSpawners());
    }

    private IEnumerator AwakeSpawners()
    {
        yield return new WaitForSeconds(awakeTime);
    }
}
