using UnityEngine;

public class LookAtTheCamera : MonoBehaviour
{
    private Camera camera;
    
    void Start()
    {
        camera = Camera.main;
    }

    
    void Update()
    {
        Vector3 lookPos = transform.position - camera.transform.position;
        transform.rotation = Quaternion.LookRotation(lookPos);
    }
}
