using UnityEngine;

public class LookAtGameObject : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private bool lockZ = false;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;
    [SerializeField] private bool lockX = false;
    [SerializeField] private bool lockY = false;

    void Update()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Aplicar offset
        targetRotation *= Quaternion.Euler(rotationOffset);

        // Aplicar locks de ejes
        Vector3 euler = targetRotation.eulerAngles;
        if (lockX) euler.x = transform.eulerAngles.x;
        if (lockY) euler.y = transform.eulerAngles.y;
        if (lockZ) euler.z = transform.eulerAngles.z;

        targetRotation = Quaternion.Euler(euler);

        // Rotación suavizada
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
}