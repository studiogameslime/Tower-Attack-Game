using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void SetRunningAnimation(bool inRunning)
    {
        if (!animator) return;
        animator.SetBool("Running", inRunning);
    }
}
