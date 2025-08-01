using System;
using Larje.Character.Abilities;
using Larje.Core.Tools.CompositeProperties;
using UnityEngine;
using UnityEngine.Serialization;
using Character = Larje.Character.Character;

namespace Larje.Character
{
    public class CharacterOrientation : CharacterAbility
    {
        [SerializeField] private OrientationTarget bodyTarget;
        [SerializeField] private OrientationTarget lookTarget;
        
        private CharacterWalk _walk;
        
        public OrientationTarget BodyTarget => bodyTarget;
        public OrientationTarget LookTarget => lookTarget;
        
        protected override void OnInitialized()
        {
            _walk = character.GetComponent<CharacterWalk>();
            
            InitializeTarget(bodyTarget);
            InitializeTarget(lookTarget);
        }

        private void Update()
        {
            if (!Permitted) return;
            
            if (Initialized)
            {
                RotateTarget(bodyTarget);
                RotateTarget(lookTarget);
            }
        }

        private void InitializeTarget(OrientationTarget target)
        {
            switch (target.AutoDirection)
            {
                case AutoDirection.None:
                    break;
                case AutoDirection.MoveDirection:
                    target.InputDirection.AddValue(GetAutoDirectionMovement, () => -1);
                    target.InputSpeedPercent.AddValue(() => _walk.SpeedPercent, () => -1);
                    break;
            }
        }

        private void RotateTarget(OrientationTarget target)
        {
            target.InputDirection.TryGetValue(out Vector3 targetDirection);
            targetDirection = Vector3.Scale(targetDirection, target.Axis).normalized;
            
            target.InputSpeedPercent.TryGetValue(out float speedPercent);
            
            if (targetDirection != Vector3.zero)
            {
                float rotationSpeed = Mathf.Lerp(target.MinRotationSpeed, target.MaxRotationSpeed, speedPercent) * Time.deltaTime;

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion currentRotation = Quaternion.RotateTowards(target.Transform.rotation,
                    targetRotation, rotationSpeed);
                
                target.Transform.rotation = currentRotation;
            }
        }

        private Vector3 GetAutoDirectionMovement()
        {
            return physics.Velocity.normalized;
        }

        [Serializable]
        public class OrientationTarget
        {
            [field: SerializeField] public Transform Transform { get; set; }
            [field: SerializeField] public Vector3 Axis { get; set; } = new  Vector3(1f, 0f, 1f);
            [field: Space] 
            [field: SerializeField] public float MinRotationSpeed { get; set; } = 90f;
            [field: SerializeField] public float MaxRotationSpeed { get; set; } = 360f;
            [field: Space] 
            [field: SerializeField] public AutoDirection AutoDirection { get; set; } = AutoDirection.MoveDirection;
            
            public PriotizedProperty<Vector3> InputDirection { get; } = new PriotizedProperty<Vector3>();
            public PriotizedProperty<float> InputSpeedPercent { get; } = new PriotizedProperty<float>();

            public Vector3 CurrentDirection => Transform.forward;
        }
        
        public enum AutoDirection
        {
            None,
            MoveDirection
        }
    }
}