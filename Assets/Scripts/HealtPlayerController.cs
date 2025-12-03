using UnityEngine;

public class HealtPlayerController : MonoBehaviour
{
    private int hpPoints=2;
    [SerializeField] private float shieldRecoverTime;
   
    public void GetDamage(int hitPlayerHP)
    {
        /*TODO
         * Manejar la "Vida"
         * Avisar al HUD Manager para que cambie el Icon
         * Sin "Vida" pueeess -> Die()
         */
        hitPlayerHP -= hpPoints;
        if(hpPoints == 1)
        {
            MessageCentral.DamagedPlayer();
            ShieldRecover();
        }
        if(hpPoints == 0)
        {
            Die();
        }
    }

    public void Die()
    {
        /*TODO
         * Matar al Player(animacion o destroy)
         * Sistema de Particulas con la muerte
         * Avisar GameManager para Resetear el juego y Volver al menu inicial (alomejor hacer cambios de escena)
         */
    }

    private void ShieldRecover()
    {
        float timer = 0f;
        while(timer <= shieldRecoverTime)
        {
            timer += Time.deltaTime;
        }
        hpPoints++;
    }
}
