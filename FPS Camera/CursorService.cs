using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Larje.Core;
using MoreMountains.Tools;
using UnityEngine;

[BindService(typeof(CursorService))]
public class CursorService : Service
{
    [SerializeField] private float sensitivity = 1.0f;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float sensitivityDecreaseZone = 0.75f;
    [SerializeField] private CursorUpdateType cursorUpdateType = CursorUpdateType.Normal;
    
    [InjectService] private InputService _inputService;
    
    private float _deltaTime;
    private Camera _camera;
    private Vector2 _rotation = Vector2.zero;
    private Vector2 _delta;
    private List<Func<bool>> _isVisible = new List<Func<bool>>();
    private List<ICursorCamera> _cameras = new List<ICursorCamera>();
    private Dictionary<Action<float>, CallbackUpdateType> _onCursorUpdated = new Dictionary<Action<float>, CallbackUpdateType>();

    private Camera MainCamera
    {
        get
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
            
            return _camera;
        }
    }

    public Vector3 Origin => MainCamera.transform.position;
    public Vector3 Direction { get; private set; }
    public Vector2 ScreenPosition { get; private set; }
    public Ray Ray => new Ray(Origin, Direction);
    public bool CursorVisible => _isVisible.All(x => x.Invoke());

    public override void Init()
    {
    }

    public void AddCursorVisibilityCondition(Func<bool> isVisible)
    {
        if (!_isVisible.Contains(isVisible))
        {
            _isVisible.Add(isVisible);
        }
    }

    public void RemoveCursorVisibilityCondition(Func<bool> isVisible)
    {
        if (_isVisible.Contains(isVisible))
        {
            _isVisible.Remove(isVisible);
        }
    }
    
    public void AddCamera(ICursorCamera camera)
    {
        if (!_cameras.Contains(camera))
        {
            _cameras.Add(camera);
        }
    }
    
    public void RemoveCamera(ICursorCamera camera)
    {
        if (_cameras.Contains(camera))
        {
            _cameras.Remove(camera);
        }
    }
    
    public void AddCursorUpdatedListener(Action<float> listener, CallbackUpdateType callbackUpdateType)
    {
        if (!_onCursorUpdated.ContainsKey(listener))
        {
            _onCursorUpdated.Add(listener, callbackUpdateType);
        }
    }
    
    public void RemoveCursorUpdatedListener(Action<float> listener)
    {
        if (_onCursorUpdated.ContainsKey(listener))
        {
            _onCursorUpdated.Remove(listener);
        }
    }
    
    private void Start()
    {
        CinemachineCore.CameraUpdatedEvent.AddListener(UpdateScreen);
    }

    private void Update()
    {
        _delta += _inputService.GetActions<InputSystem_Actions.PlayerActions>().Look.ReadValue<Vector2>();
        
        if (cursorUpdateType == CursorUpdateType.Normal)
        {
            UpdateCursor(Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (cursorUpdateType == CursorUpdateType.Fixed)
        {
            UpdateCursor(Time.fixedDeltaTime);
        }
    }
    
    private void LateUpdate()
    {
        if (cursorUpdateType == CursorUpdateType.Late)
        {
            UpdateCursor(Time.deltaTime);
        }
    }
    
    private void UpdateCursor(float deltaTime)
    {
        _deltaTime = deltaTime;
        SendUpdateEvent(CallbackUpdateType.BeforeUpdate);
        if (TryGetCurrentCamera(out ICursorCamera cursorCamera))
        {
            Vector2 delta = _delta;
            _delta -= delta;
            
            _rotation.y = RotateAxis(_rotation.y, delta.x, cursorCamera.RotationLimitMin.y, cursorCamera.RotationLimitMax.y);
            _rotation.x = RotateAxis(_rotation.x, -delta.y, cursorCamera.RotationLimitMin.x, cursorCamera.RotationLimitMax.x);

            Direction = cursorCamera.DefaultDirection;
            Direction = Quaternion.AngleAxis(_rotation.y, Vector3.up) * Direction;
            Direction = Quaternion.AngleAxis(-_rotation.x, Vector3.Cross(Direction, Vector3.up)) * Direction;
            
            SendUpdateEvent(CallbackUpdateType.AfterUpdate);    
        }
    }

    private void UpdateScreen(CinemachineBrain brain)
    {
        Vector3 point = MainCamera.transform.position + Direction * 1f;
        ScreenPosition = MainCamera.WorldToScreenPoint(point);
        SendUpdateEvent(CallbackUpdateType.AfterCinemachine);
        
        MMDebug.DrawPoint(point, Color.red, 0.025f);
    }

    private void SendUpdateEvent(CallbackUpdateType callbackUpdateType)
    {
        _onCursorUpdated.Keys.ToList().FindAll(x => _onCursorUpdated[x] == callbackUpdateType)
            .ForEach(x => x.Invoke(_deltaTime));
    }

    private float RotateAxis(float value, float delta,  float min, float max)
    {
        float center = 0f;
        float nearestBound = value < center ? min : max;
        float fullDistance = Mathf.Abs(center - nearestBound);
        float distancePercent = Mathf.Abs(value / fullDistance);
        float decreasePercent = 1f - Mathf.Clamp01(distancePercent - sensitivityDecreaseZone) / (1f - sensitivityDecreaseZone);
        
        float sensDecreased = sensitivity * decreasePercent;
        bool isDecreased = Mathf.Abs(value - nearestBound) > Mathf.Abs((value + delta) - nearestBound);

        float sens = (isDecreased ? sensDecreased : sensitivity);
        value += delta * sens;
        
        if (value > 180f)
        {
            value -= 360f;
        }
        else if (value < -180f)
        {
            value += 360f;
        }
        
        return Mathf.Clamp(value, min, max);
    }

    public bool TryGetCurrentCamera(out ICursorCamera camera)
    {
        if (_cameras.Count > 0)
        {
            camera = _cameras.OrderBy(x => x.Priority).Last();
            return true;
        }

        camera = null;
        return false;
    }

    public enum CallbackUpdateType
    {
        BeforeUpdate,
        AfterUpdate,
        AfterCinemachine
    }
    
    public enum CursorUpdateType
    {
        Normal = 1,
        Fixed = 2,
        Late = 3
    }
}
