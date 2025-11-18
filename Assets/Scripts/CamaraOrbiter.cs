using UnityEngine;

/// <summary>
/// Script per obtenir una càmera orbital al voltant del nostre target.
/// </summary>
public class CameraRotation : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float radius;

    void Update()
    {
        transform.position = target.position - (transform.forward * radius);
        transform.RotateAround(target.position, Vector3.up, Input.GetAxis("Mouse X"));
        transform.RotateAround(target.position, transform.right, Input.GetAxis("Mouse Y"));

    }
}