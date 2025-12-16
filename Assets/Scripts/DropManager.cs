using UnityEngine;

public class DropManager : MonoBehaviour
{
    //[SerializeField] private int quality =1;
   
    private void OnTriggerEnter(Collider detect)
    {
        if (detect.CompareTag("Player"))
        {
            MessageCentral.PickupSample(20);
            Destroy(gameObject);
        }
    }

}
