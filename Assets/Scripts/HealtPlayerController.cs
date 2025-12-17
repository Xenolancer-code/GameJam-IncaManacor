using System.Collections;
using UnityEngine;

public class HealtPlayerController : MonoBehaviour
{
    [Header("Life")]
    private int hpPoints=2;
    private bool playerIsDamaged = false;
    [Header("Shield")]
    [SerializeField] private float shieldRecoverTime;
    [Header("ShieldGating")]
    private bool shieldGatingOn = false;
    [SerializeField] private float shieldGatingTime;

    private Animator animator;

    private void Awake()
    {
        hpPoints = 2;
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        MessageCentral.DamagedPlayer(false);
    }
    public void GetDamage(int hitPlayerHP)
    {
        /*TODO
         * Manejar la "Vida"
         * Avisar al HUD Manager para que cambie el Icon
         * Sin "Vida" pueeess -> Die()
         */
        if (shieldGatingOn == true) return;
        hpPoints -= hitPlayerHP;
        if(hpPoints == 1)
        {
            animator.SetBool("PlayerIsDamaged", true);
            animator.SetTrigger("TakeHit");
            MessageCentral.DamagedPlayer(true);
            TrytoShieldRecover();
            StartCoroutine(ShieldGating());
        }
        if(hpPoints == 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Soy el Player y me han matado");
        animator.SetTrigger("Die");
        /*TODO
         * Matar al Player(animacion o destroy)
         * Sistema de Particulas con la muerte
         * Avisar GameManager para Resetear el juego y Volver al menu inicial (alomejor hacer cambios de escena)
         */
    }

    public void TrytoShieldRecover()
    {
        if (playerIsDamaged) return;
        //Invoke("ShieldRecoverAlt", shieldRecoverTime);
        StartCoroutine(ShieldRecover());
    }
    private IEnumerator ShieldRecover()
    {
        yield return new WaitForSeconds(shieldRecoverTime);
        hpPoints = 2;
        MessageCentral.DamagedPlayer(false);
        animator.SetBool("PlayerIsDamaged", false);
    }

    private IEnumerator ShieldGating()
    {
        //Inmunidad al romper escudo para no recibir hits continuos
        //TODO Particulas para indicar la invulnerabilidad
        shieldGatingOn = true;
        yield return new WaitForSeconds(shieldGatingTime);
        shieldGatingOn = false;
    }
}
