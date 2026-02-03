using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
    [SerializeField] private GameObject collectionScene;
    [SerializeField] private float rotationSpeed = 6f;
    [SerializeField] private float cooldown = 10f;

    private bool canActivate = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!canActivate) return;

        StartCoroutine(RotatePlane());
    }

    private IEnumerator RotatePlane()
    {
        canActivate = false;

        Quaternion start = collectionScene.transform.rotation;
        Quaternion end = start * Quaternion.Euler(180, 0, 0);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            collectionScene.transform.rotation = Quaternion.Slerp(start, end, t);
            yield return null;
        }

        yield return new WaitForSeconds(cooldown);
        canActivate = true;
    }
}