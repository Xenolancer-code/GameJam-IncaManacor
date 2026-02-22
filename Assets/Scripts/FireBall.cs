using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float maxDistance = 12f;
    private int hitPlayerHP = 1;

    private Vector3 direction;
    private Vector3 startPosition;

    public void Init(Vector3 dir)
    {
        direction = dir;
        startPosition = transform.position;

        // Rotar visualmente hacia donde dispara
        transform.forward = direction;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out HealtPlayerController health))
        {
            health.GetDamage(hitPlayerHP);
            Destroy(gameObject);
        }
    }
}