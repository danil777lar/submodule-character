using System;
using Larje.Core;
using UnityEngine;

public class GunLookAtCursor : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    
    [InjectService] private CursorService _cursorService;
    
    private void Start()
    {
        DIContainer.InjectTo(this);
        _cursorService.AddCursorUpdatedListener(OnCursorUpdated, CursorService.CallbackUpdateType.AfterUpdate);
    }

    private void OnDestroy()
    {
        if (_cursorService != null)
        {
            _cursorService.RemoveCursorUpdatedListener(OnCursorUpdated);
        }
    }

    private void OnCursorUpdated(float deltaTime)
    {
        transform.forward = Vector3.Lerp(transform.forward, _cursorService.Direction, deltaTime * speed);
    }
}
