using Larje.Character;
using Larje.Character.Abilities;
using UnityEngine;

namespace Larje.Character.Sounds
{
    public class CharacterStepSound : MonoBehaviour
    {
        [SerializeField] private float spawnDistance = 0.25f;
        [SerializeField] private SoundSettings stepSoundSettings;

        private float _distanceTraveled;
        private Vector3 _lastPosition;
        private Character _character;
        private CharacterPhysics _characterPhysics;
        private CharacterWalk _walk;

        private void Start()
        {
            _lastPosition = transform.position;
            _character = GetComponentInParent<Character>();
            _characterPhysics = GetComponentInParent<CharacterPhysics>();
            _walk = _character.FindAbility<CharacterWalk>();
        }

        private void Update()
        {
            if (_character.IsActive && _character.IsAlive && _characterPhysics.Grounded)
            {
                float distance = Vector3.Distance(transform.position, _lastPosition);
                _distanceTraveled += distance;

                if (_distanceTraveled >= spawnDistance)
                {
                    float volume = _walk.SpeedPercent;
                    stepSoundSettings.Play(a => a.SetVolume(volume));
                    _distanceTraveled = 0f;
                }

                _lastPosition = transform.position;
            }
        }
    }
}
