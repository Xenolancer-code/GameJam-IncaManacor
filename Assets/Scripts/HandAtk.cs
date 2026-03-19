using UnityEngine;

public class HandAtk : MonoBehaviour
{
    private int hitPlayerHP = 1;
    [SerializeField] private GameObject player;
    [SerializeField] private float attackRadius;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask playerLayer;

    private void OnTriggerEnter(Collider detect)
    {
        if (player == null) return;
       
        var collidedPlayer = Physics.OverlapSphere(attackPoint.position, attackRadius, playerLayer);
        if (collidedPlayer == null || collidedPlayer.Length == 0) return;
       
        if (player.TryGetComponent(out HealtPlayerController healtPlayer))
        {
            healtPlayer.GetDamage(hitPlayerHP);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
