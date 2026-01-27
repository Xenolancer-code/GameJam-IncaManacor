using UnityEngine;

public class OutlineMenu : MonoBehaviour
{
    public Camera cam;
    public float maxDistance = 100f;
    public LayerMask layerMask;
    RaycastHit hit;
    Collider lastHit;
    [SerializeField] private Animator animatorBook;

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
        {
            // Si es un objeto nuevo
            if (hit.collider != lastHit)
            {
                ClearLastOutline();

                Outline outline = hit.collider.GetComponent<Outline>();
                if (outline != null)
                {
                    //outline.OutlineMode = Outline.Mode.OutlineAll;
                    outline.OutlineWidth = 10;
                    animatorBook.SetTrigger("Open");
                    lastHit = hit.collider;
                }
            }
        }
        else
        {
            ClearLastOutline();
        }
    }

    void ClearLastOutline()
    {
        if (lastHit != null)
        {
            Outline outline = lastHit.GetComponent<Outline>();
            if (outline != null)
            {
                //outline.OutlineMode = Outline.Mode.OutlineHidden;
                outline.OutlineWidth = 3;
                animatorBook.SetTrigger("Close");
            }
            lastHit = null;
        }
    }

}

