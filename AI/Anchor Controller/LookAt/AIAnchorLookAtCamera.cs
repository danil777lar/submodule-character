using UnityEngine;

public class AIAnchorLookAtCamera : MonoBehaviour, IAIAnchorLookAt
{
    private Camera _camera;

    public Vector3 LookAt => _camera.transform.position;

    private void Awake()
    {
        _camera = Camera.main;
    }
}
