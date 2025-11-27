using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// Script per tal que la càmera del nostre joc segueixi el jugador
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -8f);
    //[SerializeField] private float rotationSpeed = 5.0f;


    void Update()
    {
        if (target == null)
            return;

        // Calculate la posició desitjada de la càmera darrere el target
       // Vector3 desiredPosition = target.position + (target.forward * offset.z) + (target.up * offset.y);

        // colocam la càmera en la posició desitjada
        transform.position = target.position + offset;

        // Mirem al target
        transform.LookAt(target);

        //Debug.Log("Forward global: " + Vector3.forward);
        //Debug.Log("Forward PJ: " + target.forward);
    }

    //Versió suavitzada
    //void Update()
    //{
    //    if (target != null)
    //    {

    //        Vector3 desiredPosition = target.position + (target.forward * offset.z) + (target.up * offset.y);


    //        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * rotationSpeed);


    //        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
    //        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    //    }
    //}
}