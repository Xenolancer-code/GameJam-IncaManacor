using System;
using Unity.VisualScripting;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float rotationSpeed = 180;
    private Vector3 lastPosition;
    private float distanceTravelled = 0f;
    [SerializeField] private float maxDistance = 12f;
    private int hitPlayerHP = 1;

    private Transform target;
    private Vector3 startPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    public void Init(Transform _target)
    {
        target = _target;
        startPosition = transform.position;
        transform.LookAt(target);

        // // Rotar visualmente hacia donde dispara
        // transform.forward = direction;
    }

    private void Update()
    {
        // transform.position += direction * speed * Time.deltaTime;
        
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        RotateToPlayer();

        // Calcular cuánto se movió este frame
        float frameDistance = Vector3.Distance(transform.position, lastPosition);
        distanceTravelled += frameDistance;

        lastPosition = transform.position;

        if (distanceTravelled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void RotateToPlayer()
    {
        Vector3 directionToPlayer = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
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