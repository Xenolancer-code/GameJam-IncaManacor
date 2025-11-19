using UnityEngine;

public class PlayerAtk : MonoBehaviour
{
    //private CharacterController cc;
    private Animator animator;
    private void Awake()
    {
        //cc = gameObject.GetComponent<CharacterController>();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        
    }


    public void LeftClickPressed()
    {
        Debug.Log("Estoy atacando al enemigo");
        animator.SetTrigger("LeftClick");
    }
    public void RightClickPressed()
    {
        Debug.Log("Estoy golpeando en area");
        animator.SetTrigger("RightClick");
    }
}
