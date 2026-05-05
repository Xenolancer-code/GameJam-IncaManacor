using System;
using UnityEngine;

public class MessageCentral : MonoBehaviour
{
    public static event Action OnDieEnemy; //Mi enemigo muere
    public static void DieEnemy()
    {
        OnDieEnemy?.Invoke();
    }

    public static event Action OnDieBoss; //Boss derrotado
    public static void DieBoss()
    {
        OnDieBoss?.Invoke();
    }
    
    public static event Action OnDiePlayer;
    public static void DiePlayer()
    {
        OnDiePlayer?.Invoke();
    }

    public static event Action OnStart; //Juego iniciado
    public static void Start()
    {
        OnStart?.Invoke();
    }
    
    public static event Action<bool> OnDashinActivated; // Activar?n el Dash
    public static void DashinActivated(bool isDashing)
    {
        OnDashinActivated?.Invoke(isDashing);
    }

    public static event Action OnDamagedEnemy; // Hicier?n da?o a los enemigos
    public static void DamagedEnemy() {
        OnDamagedEnemy?.Invoke();
    }
    public static event Action OnHandDestroyed; //Mano destruida

    public static void HandDestroyed()
    {
        OnHandDestroyed?.Invoke();
    }
    
    public static event Action OnHandHeal; //Mano destruida

    public static void HandHeal()
    {
        OnHandHeal?.Invoke();
    }
    
    public static event Action<bool> OnDamagedPlayer; // Hicier?n da?o al Player
    public static void DamagedPlayer(bool playerIsDamaged)
    {
        OnDamagedPlayer?.Invoke(playerIsDamaged);
    }


    public static event Action<int> OnPickupSample; // Se recogio una muestra dropeada por enemigos
    public static void PickupSample(int sampleQuality)
    {
        OnPickupSample?.Invoke(sampleQuality);
    }

    public static event Action<bool> OnBarFull;

    public static void BarFull(bool barFilled)
    {
        OnBarFull?.Invoke(barFilled);
    }
    
    public static event Action OnAllSpawnersDestroyed;

    public static void AllSpawnersDestroyed()
    {
        OnAllSpawnersDestroyed?.Invoke();
    }
    
    public static event Action OnSpawnerDestroyed;

    public static void SpawnerDestroyed()
    {
        OnSpawnerDestroyed?.Invoke();
    }
    
    public static event Action OnSwapScene;

    public static void SwapScene()
    {
        OnSwapScene?.Invoke();
    }

    public static event Action<bool> OnPausedGame;

    public static void PausedGame(bool paused)
    {
        OnPausedGame?.Invoke(paused);
    }

    public static event Action<bool> OnPlayerWins;

    public static void PlayerWins(bool playerWins)
    {
        OnPlayerWins?.Invoke(playerWins);
    }
}
