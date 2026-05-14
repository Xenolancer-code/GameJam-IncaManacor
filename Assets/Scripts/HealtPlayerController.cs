using System.Collections;
using UnityEngine;

public class HealtPlayerController : MonoBehaviour
{
   [SerializeField] private ParticleSystem bloodParticles;
   [SerializeField]  private ParticleSystem healParticles;
    [Header("Life")]
    private int hpPoints=2;
    private bool playerIsDamaged = false;
    private bool playerIsDead=false;
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
        if(playerIsDead ||shieldGatingOn || playerIsDamaged) return;
        hpPoints -= hitPlayerHP;
        
        if(hpPoints >= 1)
        {
            animator.SetBool("PlayerIsDamaged", true);
            animator.SetTrigger("TakeHit");
            AudioManager.I?.PlaySound(SoundName.PlayerInjured,transform,1f);
            Vector3 arriba = new Vector3(0, 0.75f, 0);
            Instantiate(bloodParticles,transform.position+arriba,Quaternion.identity);
            MessageCentral.DamagedPlayer(true);
            TrytoShieldRecover();
            StartCoroutine(ShieldGating());
        }
        if(hpPoints <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        playerIsDead = true;
        MessageCentral.DiePlayer();
        animator.SetBool("Die",true);
        AudioManager.I?.PlaySound(SoundName.PlayerDie,transform,1f);
    }

    public void TrytoShieldRecover()
    {
        if (playerIsDamaged && playerIsDead) return;
        //Invoke("ShieldRecoverAlt", shieldRecoverTime);
        StartCoroutine(ShieldRecover());
    }
    private IEnumerator ShieldRecover()
    {
        yield return new WaitForSeconds(shieldRecoverTime);
        if (playerIsDead) yield break;
        Instantiate(healParticles,transform.position,healParticles.transform.rotation,transform);
        hpPoints = 2;
        AudioManager.I?.PlaySound(SoundName.PlayerRecover,transform,1f);
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
