using UnityEngine;

public class Gate : MonoBehaviour
{
    private Animator animator;
    private bool isGateOpen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenGate()
    {
        if (isGateOpen) return; //Avoid reopen the gate if its already open
        animator.SetTrigger("OpenGate");
        isGateOpen = true;
    }

    public void CloseGate()
    {
        if (!isGateOpen) return; //Avoid reclose the gate if its already closed
        animator.SetTrigger("CloseGate");
        isGateOpen = false;
    }

    
}
