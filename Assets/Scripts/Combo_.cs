using UnityEngine;
using UnityEngine.InputSystem;

public class Combo_ : StateMachineBehaviour
{
    [SerializeField] private int num;
    private int valor = 0;
    private PlayerAtk playerAtk;
    
     //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.SetBool("canInterrupt", false);
       
       if (playerAtk == null)
           playerAtk = animator.GetComponent<PlayerAtk>();
    }
     //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerAtk != null && playerAtk.basicAttackPerformed && (num == 1 || animator.GetBool("canInterrupt")))
        {
            valor = num;
        }
        animator.SetInteger("control", valor);
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        valor = 0;
    }
    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
