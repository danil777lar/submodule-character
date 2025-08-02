using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Larje.Core;
using Larje.Core.Services;
using MoreMountains.Tools;
using UnityEngine;

public class PlayerCamera : MonoBehaviour, ICursorCamera
{
    [Header("Limits")]
    [SerializeField] private Vector2 limitMin = new Vector2(-90f, -90f);
    [SerializeField] private Vector2 limitMax = new Vector2(90f, 90f);
    
    [Header("Sensitivity")]
    [SerializeField] private bool useDecreaseSensZone = true;
    [SerializeField] private float decreaseSensZoneMin = 50f;
    [SerializeField] private float decreaseSensZoneMax = 100f;
    [SerializeField] private AnimationCurve speedCurve;
    
    [Header("Links")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private List<Axis> axes;

    [InjectService] private CursorService _cursorService;
    
    public int Priority => virtualCamera.Priority;
    public bool IsActive => _cursorService.IsCurrent(this);
    public Vector3 DefaultDirection { get; private set; }
    public Vector2 RotationLimitMin => limitMin;
    public Vector2 RotationLimitMax => limitMax;
    
    private void Start()
    {
        DIContainer.InjectTo(this);

        DefaultDirection = virtualCamera.transform.forward;
        
        _cursorService.AddCamera(this);
        _cursorService.AddCursorUpdatedListener(UpdateAxisRotations, CursorService.CallbackUpdateType.AfterUpdate);
    }
    
    private void OnDestroy()
    {
        if (_cursorService == null) return;
        
        _cursorService.RemoveCamera(this);
        _cursorService.RemoveCursorUpdatedListener(UpdateAxisRotations);
    }

    private void UpdateAxisRotations(float deltaTime)
    {
        axes.ForEach(x => RotateAxis(x.AxisTransform, x.Mask, x.Scale, deltaTime));
    }

    private void RotateAxis(Transform axis, Vector3 mask, Vector3 scale, float deltaTime)
    {
        Quaternion from = Quaternion.LookRotation(axis.forward);
        Quaternion to = Quaternion.LookRotation(_cursorService.Direction);
        
        float targetSpeed = speedCurve.Evaluate(1f);
        if (useDecreaseSensZone)
        {
            float angle = Mathf.Max(Quaternion.Angle(from, to) - decreaseSensZoneMin, 0f);
            float t = angle / (decreaseSensZoneMax - decreaseSensZoneMin);
            targetSpeed = angle < decreaseSensZoneMin ? 0f : speedCurve.Evaluate(t);
        }

        Vector3 eulerAngles = Quaternion.Lerp(from, to, deltaTime * targetSpeed).eulerAngles;
        eulerAngles.x = Mathf.Lerp(from.eulerAngles.x, eulerAngles.x, mask.x);
        eulerAngles.y = Mathf.Lerp(from.eulerAngles.y, eulerAngles.y, mask.y);
        eulerAngles.z = Mathf.Lerp(from.eulerAngles.z, eulerAngles.z, mask.z);
        
        axis.rotation = Quaternion.Euler(eulerAngles);
        axis.localRotation = Quaternion.Euler(Vector3.Scale(axis.localRotation.eulerAngles, scale));
    }

    [Serializable]
    private class Axis
    {
        [field: SerializeField] public Transform AxisTransform { get; private set; }
        [field: SerializeField] public Vector3Int Mask { get; private set; }
        [field: SerializeField] public Vector3Int Scale { get; private set; } = Vector3Int.one;
    }
}
