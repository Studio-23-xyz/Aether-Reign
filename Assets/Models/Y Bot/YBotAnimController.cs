using UnityEngine;

public class YBotAnimController : MonoBehaviour
{
    public Animator Animator;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Animator.SetTrigger("Run");
        if (Input.GetKeyDown(KeyCode.D))
            Animator.SetTrigger("Dance");
        if (Input.GetKeyDown(KeyCode.I))
            Animator.SetTrigger("Idle");
    }
}
