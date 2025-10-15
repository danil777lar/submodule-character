using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class FPSCameraShaker : MonoBehaviour
{
    [SerializeField] private Transform cameraPositionTransform;
    [SerializeField] private Transform cameraRotationTransform;
    
    private float _defaultFov;
    private Vector3 _defaultPosition;
    private Vector3 _defaultRotation;
    private CinemachineCamera _cam;
    
    private List<Shake> _shakes = new List<Shake>();

    public void AddShake(Shake shake)
    {
        _shakes.Add(shake);
    }

    private void Start()
    {
        _defaultPosition = cameraPositionTransform.localPosition;
        _defaultRotation = cameraRotationTransform.localEulerAngles;
        
        _cam = GetComponentInChildren<CinemachineCamera>();
        _defaultFov = _cam.Lens.FieldOfView;
    }

    private void FixedUpdate()
    {
        ComputeShakes();
    }

    private void ComputeShakes()
    {
        Vector3 position = _defaultPosition;
        Vector3 rotation = _defaultRotation;
        float fov = _defaultFov;
        
        List<Shake> shakesToRemove = new List<Shake>();
        foreach (Shake shake in _shakes)
        {
            shake.PassedTime += Time.deltaTime * (shake.Speed?.Invoke() ?? 1f);
            if (shake.PassedTime >= shake.Animation.Duration)
            {
                if (shake.Looped)
                {
                    shake.PassedTime = 0f;
                }
                else
                {
                    shakesToRemove.Add(shake);
                    continue;
                }
            }

            float percent = shake.PassedTime / shake.Animation.Duration;
            float intensity = shake.Intensity?.Invoke() ?? 1f;

            position += new Vector3(shake.Animation.XPosition.Evaluate(percent), 
                            shake.Animation.YPosition.Evaluate(percent),
                            shake.Animation.ZPosition.Evaluate(percent)) *
                        intensity;
            
            rotation += new Vector3(shake.Animation.XRotation.Evaluate(percent),
                            shake.Animation.YRotation.Evaluate(percent),
                            shake.Animation.ZRotation.Evaluate(percent)) *
                        intensity;
            
            fov *= Mathf.Lerp(1f, shake.Animation.Fov.Evaluate(percent), intensity);
        }
        
        cameraPositionTransform.localPosition = position;
        cameraRotationTransform.localEulerAngles = rotation;
        _cam.Lens.FieldOfView = fov;
        
        foreach (Shake shake in shakesToRemove)
        {
            _shakes.Remove(shake);
        }
    }

    public class Shake
    {
        public float PassedTime;
        
        public readonly FPSCameraShakeAnimation Animation;
        public readonly bool Looped;
        public readonly Func<float> Speed;
        public readonly Func<float> Intensity;
        
        public Shake(FPSCameraShakeAnimation animation, bool looped = false, Func<float> speed = null, Func<float> intensity = null)
        {
            Animation = animation;
            Looped = looped;
            
            Speed = speed;
            Intensity = intensity;
        }
    }
}
