using UnityEngine;

public interface IProjectileTarget
{
    void OnHit(Vector3 hitPoint, Vector3 hitNormal, Vector3 hitDirection);
}
