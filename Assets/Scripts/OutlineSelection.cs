using UnityEngine;

public class OutlineSelection : MonoBehaviour
{
    public LayerMask raycastMask;
    public string hoverLayerName = "OutlineSelected";
    public string defaultLayerName = "Outline";

    private Camera cam;
    private GameObject lastObject;
    private int hoverLayer;
    private int defaultLayer;

    void Start()
    {
        cam = Camera.main;
        hoverLayer = LayerMask.NameToLayer(hoverLayerName);
        defaultLayer = LayerMask.NameToLayer(defaultLayerName);
    }

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.cyan);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, raycastMask))
        {
            if (lastObject != hit.collider.gameObject)
            {
                ResetLastObject();
                lastObject = hit.collider.gameObject;
                lastObject.layer = hoverLayer;
            }
        }
        else
        {
            ResetLastObject();
        }
    }

    void ResetLastObject()
    {
        if (lastObject != null)
        {
            lastObject.layer = defaultLayer;
            lastObject = null;
        }
    }
}
