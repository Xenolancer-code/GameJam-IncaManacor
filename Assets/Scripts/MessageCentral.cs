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
}
