using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("Punch", 0, 0f);
    }
}
