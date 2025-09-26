using UnityEngine;

public class AIAnchorPositionStatic : MonoBehaviour, IAIAnchorPosition
{
    [SerializeField] private Transform point;
    
    public Vector3 Position => point.position;
}
