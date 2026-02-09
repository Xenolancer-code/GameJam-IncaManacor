using UnityEngine;

public class OutlineMenu : MonoBehaviour
{
    public Camera cam;
    public float maxDistance = 100f;
    public LayerMask layerMask;
    RaycastHit hit;
    Collider lastHit;
    [SerializeField] private Animator animatorBook;
    [SerializeField] private Animator animatorGramofono;

    void LateUpdate()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
        {
            Debug.Log("Hit: "+hit.collider.name);

            Outline outline = hit.collider.GetComponent<Outline>();
            if (outline != null)
            {
                outline.OutlineWidth = 10;

                if(hit.collider.name=="tapa")
                {
                    //outline.OutlineMode = Outline.Mode.OutlineAll;
                    animatorBook.SetBool("Close2",false);
                   
                }
                if(hit.collider.name=="gramo")
                {
                    //outline.OutlineMode = Outline.Mode.OutlineAll;
                    animatorGramofono.SetBool("Settings",true);
                   
                }
                lastHit = hit.collider;
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
                Debug.Log("Release: "+lastHit.name);
                //outline.OutlineMode = Outline.Mode.OutlineHidden;
                outline.OutlineWidth = 3;

                if (lastHit.name == "tapa")
                {
                    animatorBook.SetBool("Close2",true);
                }
                if (lastHit.name == "gramo")
                {
                    animatorGramofono.SetBool("Settings",false);
                }
            }
            lastHit = null;
        }
    }
}

