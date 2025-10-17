using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerCubeAnimator : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void ResetSettings()
    {
        SetWalkBool(false);
        _animator.Rebind();
        _animator.Update(0f);
    }

    public void EnableAnimator(bool value)
    {
        _animator.enabled = value;
    }

    public void SetAvailableTrigger()
    {
        _animator.SetTrigger("isAvailable");
    }

    public void SetWalkBool(bool value)
    {
        _animator.SetBool("isWalk", value);
    }
}
