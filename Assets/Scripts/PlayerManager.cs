using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    
    [SerializeField] private float dashCooldown=2f;
    private float dashInitTime;
    public bool isDashing=true; //True = se puede usar -- False = no se puede usar
    
   

    void Update()
    {
        if(!isDashing && Time.time - dashInitTime > dashCooldown)
        {
            isDashing = true;
        }
    }
    public void initDashCooldown() { 
        dashInitTime = Time.time;
        isDashing = false;
    }
}
