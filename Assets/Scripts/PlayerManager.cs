using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private float dashCooldown=2f;
    private float dashInitTime;
    public bool isDashing=true; //True = se puede usar -- False = no se puede usar
    
     
    
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


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
