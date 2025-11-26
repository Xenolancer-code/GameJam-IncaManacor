using System;
using UnityEngine;

public class MessageCentral : MonoBehaviour
{
    public static event Action OnDieEnemy; //Mi enemigo muere
    
    public static void DieEnemy()
    {
        OnDieEnemy?.Invoke();
    }
}
