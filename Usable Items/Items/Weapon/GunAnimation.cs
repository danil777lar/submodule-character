using System;
using Larje.Character.Abilities;
using UnityEngine;

public class GunAnimation : MonoBehaviour
{
    [SerializeField] private AnimatorOverrideController animationOverride;

    private Animator _animator;
    private GunUsableItem _gun;
    private CharacterWalk _characterWalk;
    private CharacterRun _characterRun;

    private void Awake()
    {
        _gun = GetComponent<GunUsableItem>();
        _animator = GetComponentInChildren<Animator>();
        _characterWalk = GetComponentInParent<CharacterWalk>();
        _characterRun = GetComponentInParent<CharacterRun>();

        _animator.runtimeAnimatorController = animationOverride;
    }

    private void Update()
    {
        AnimationType currentAnimation = GetCurrentAnimation();
        float currentAnimationTime = GetCurrentAnimationTime(currentAnimation);
        
        _animator.SetFloat("AnimationTime", currentAnimationTime);
        
        float idleBlend = Mathf.Lerp(_animator.GetFloat("IdleBlend"), _characterRun.Running ? 1f : 0f, 0.2f); 
        _animator.SetFloat("IdleBlend", idleBlend);
        
        foreach (AnimationType anim in Enum.GetValues(typeof(AnimationType)))
        {
            _animator.SetBool(anim.ToString(), anim == currentAnimation);
        }
    }

    private AnimationType GetCurrentAnimation()
    {
        return _gun.IsShootInProgress ? AnimationType.Shoot :  AnimationType.Idle;
    }

    private float GetCurrentAnimationTime(AnimationType currentAnimation)
    {
        switch (currentAnimation)
        {
            case AnimationType.Idle: return 0f;
            case AnimationType.Aim: return 0f;
            case AnimationType.Equip: return 0f;
            case AnimationType.Unequip: return 0f;
            case AnimationType.Shoot: return _gun.ShootProgress;
            case AnimationType.Reload: return 0f;
        }

        return 0f;
    }

    private enum AnimationType
    {
        Idle,
        Aim,
        Equip,
        Unequip,
        Shoot,
        Reload
    }
}
