using Larje.Character;
using UnityEngine;

[RequireComponent(typeof(PlayerCamera))]
public class PlayerCameraCharacterConnector : MonoBehaviour
{
    private PlayerCamera _playerCamera;
    private Character _character;

    private void Start()
    {
        _playerCamera = GetComponent<PlayerCamera>();
        _character = GetComponentInParent<Character>();
        
        _playerCamera.permitted.AddValue(() => _character.IsAlive);
        _character.IsActive.AddValue(() => _playerCamera.IsCurrent);
    }
}
