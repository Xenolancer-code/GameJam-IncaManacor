using System;
using UnityEngine;

public class MessageCentral : MonoBehaviour
{
    public static event Action OnDieEnemy; //Mi enemigo muere
    
    public static void DieEnemy()
    {
        OnDieEnemy?.Invoke();
    }


    public static event Action OnStart; //Juego iniciado
    public static void Start()
    {
        OnStart?.Invoke();
    }


    public static event Action<bool> OnDashinActivated; // Activaron el Dash
    public static void DashinActivated(bool isDashing)
    {
        OnDashinActivated?.Invoke(isDashing);
    }



    public static event Action OnDamagedEnemy; // Hicieron daño a los enemigos
    public static void DamagedEnemy() {
        OnDamagedEnemy?.Invoke();
    }

}
