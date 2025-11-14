using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BeastAnimator : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void ResetSettings()
    {
        _animator.speed = 1.0f;
        SetWalkBool(false);
        _animator.Rebind();
        _animator.Update(0f);
    }

    public void EnableAnimator(bool value)
    {
        _animator.enabled = value;
    }

    public void SetWalkBool(bool value)
    {
        _animator.SetBool("isWalk", value);
    }
}
