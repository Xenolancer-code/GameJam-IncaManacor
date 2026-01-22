using UnityEngine;

public class BloodManager : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
