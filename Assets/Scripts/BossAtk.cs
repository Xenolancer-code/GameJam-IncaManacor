using System;
using UnityEngine;

public class BossAtk : MonoBehaviour
{
    private Animator animator;
    
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float knockbackForce;
    [Header("Collider Tiggers")]
    [SerializeField] private Collider armColliderRight;
    [SerializeField] private Collider armColliderLeft;
    [SerializeField] private Collider handAttackRight;
    [SerializeField] private Collider handAttackLeft;

    //private int instakill = 2;
    private int hitPlayerHP = 1;
    private bool playerInsideAttackRange = false;
    private bool playerIsDead = false;
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
}
