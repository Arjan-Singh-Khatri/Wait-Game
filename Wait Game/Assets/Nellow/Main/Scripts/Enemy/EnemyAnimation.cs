using System.Collections;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateAnimationState(float velocity)
    {
        animator.SetFloat("FLOAT", velocity);
    }

    public void PlayLookAroundAnimation(bool val)
    {
        animator.SetBool("isLookingAround", val);
    }

}
