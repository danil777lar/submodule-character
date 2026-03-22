using Larje.Character.Abilities;
using Larje.Core;
using UnityEngine;

public class PlayerTopDownControl : MonoBehaviour
{
    [InjectService] private InputService _inputService;

    private void Start()
    {
        DIContainer.InjectTo(this);

        CharacterWalk walk = GetComponentInParent<CharacterWalk>();
        walk.Input.AddValue(GetInput, () => 1);
    }

    private Vector2 GetInput()
    {
        return _inputService.PlayerMovement;
    }
}
