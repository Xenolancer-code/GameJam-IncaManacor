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


    public static event Action<bool> OnDashinActivated; // Activarón el Dash
    public static void DashinActivated(bool isDashing)
    {
        OnDashinActivated?.Invoke(isDashing);
    }



    public static event Action OnDamagedEnemy; // Hicierón daño a los enemigos
    public static void DamagedEnemy() {
        OnDamagedEnemy?.Invoke();
    }

    public static event Action<bool> OnDamagedPlayer; // Hicierón daño al Player
    public static void DamagedPlayer(bool playerIsDamaged)
    {
        OnDamagedPlayer?.Invoke(playerIsDamaged);
    }


    public static event Action OnIncrementPlayerDamage; //Incrementamos el daño del Player
    public static void IncrementPlayerDamage()
    {
        OnIncrementPlayerDamage?.Invoke();
    }

    public static event Action OnIncrementPlayerRange; //Incrementamos el rango del Player
    public static void IncrementPlayerRange()
    {
        OnIncrementPlayerRange?.Invoke();
    }

}
