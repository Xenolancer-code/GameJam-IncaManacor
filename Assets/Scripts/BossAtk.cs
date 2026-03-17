using System;
using UnityEngine;

public class BossAtk : MonoBehaviour
{
    private Animator animator;
    [Header("Player Effects")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private GameObject player;
    private bool playerIsDead = false;
    [Header("Empty GameObject Position References")]
    [SerializeField] private Transform ogRightHandPosition;
    [SerializeField] private Transform ogLeftHandPosition;
    [SerializeField] private Transform slamRightHandPosition;
    [SerializeField] private Transform slamLeftHandPosition;
    [SerializeField] private Transform swipeRightHandPosition;
    [SerializeField] private Transform swipeLeftHandPosition;
    [SerializeField] private Transform hornRayCast;
    [SerializeField] private Transform centerRange;
    [Header("Collider Tiggers")]
    [SerializeField] private Collider handRight;
    [SerializeField] private Collider handLeft;
    [SerializeField] private Collider damaged_area;
    [Header("Boss Parameters")] 
    [SerializeField] private float range;
    private int hitPlayerHP = 1;

    //private int instakill = 2;
    private bool playerInsideAttackRange = false;
    
    /*Todo:
     El boss tiene 2 ataques fisicos y 2 a distancia
     Fisicos:
      -Manotazo con cada mano dependiendo donde este el player mas cerca
      -Barrido que empieza por donde este mas lejos el player
      A Distancia:
      -Chasquea los dedos generando una zona de daño
      -Lanza rayo por los cuernos que luego de impactar genera onda sismica dañina
      (Posibilidad de añadir atk intsta kill?)
     */
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider detect)
    {
    }
    
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.chartreuse;
    //     Gizmos.DrawWireSphere(centerRange.position, range);
    // }
}
