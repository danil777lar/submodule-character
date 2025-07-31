using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RayFire;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class RayfireRigidProjectileTarget : MonoBehaviour, IProjectileTarget
{
    [SerializeField] private float force;
    [SerializeField] private float maxMassToDynamic = 1f;
    [SerializeField] private string layerOnHit = "Default";
    [Space] 
    [SerializeField] private RayfireRigid rayfirePrefab;
    [SerializeField] private List<SkinnedMeshRenderer> rendsToDemolish;
    [SerializeField] private List<SkinnedMeshRenderer> rendsToBake;
    [Space]
    [SerializeField] private UnityEvent onHit;

    public void OnHit(Vector3 hitPoint, Vector3 hitNormal, Vector3 hitDirection)
    {
        RayfireRigid mainRigid = PrepareForDestroy(hitPoint, hitDirection);
        List<GameObject> allFragments = new List<GameObject>();
        foreach (RayfireRigid fragment in mainRigid.fragments)
        {
            RayfireShatter shatter = fragment.GetComponent<RayfireShatter>();
            MeshCollider meshCollider = fragment.GetComponent<MeshCollider>();

            Vector3 closestPoint = meshCollider.ClosestPointOnBounds(hitPoint);
            shatter.centerPosition = shatter.transform.InverseTransformPoint(closestPoint);
            shatter.centerDirection = Quaternion.LookRotation(shatter.transform.InverseTransformDirection(hitDirection));
            
            fragment.Initialize();
            fragment.Demolish();
            
            fragment.fragments.ForEach(x => allFragments.Add(x.gameObject));
        }
        
        List<GameObject> fragmentsToBake = SortFragments(allFragments, hitPoint, hitDirection);
        Rigidbody rb = BakeObjects(fragmentsToBake);
        rb.AddForceAtPosition(hitDirection * force * 0.1f, hitPoint, ForceMode.Impulse);

        Destroy(mainRigid.gameObject);
        
        onHit?.Invoke();
    }

    private RayfireRigid PrepareForDestroy(Vector3 hitPoint, Vector3 hitDir)
    {
        RayfireRigid rigid = Instantiate(rayfirePrefab, transform.position, transform.rotation);
        rigid.transform.parent = transform.parent;
        rigid.gameObject.SetActive(true);
        
        foreach (SkinnedMeshRenderer rend in rendsToDemolish)
        {
            if (rend.gameObject.activeSelf)
            {
                MeshRenderer bakedMesh = BakeSkinned(rend);
                bakedMesh.transform.SetParent(rigid.transform);
            }
        }
        
        rigid.Initialize();
        rigid.Demolish();

        return rigid;
    }

    private List<GameObject> SortFragments(List<GameObject> fragments, Vector3 forcePoint, Vector3 forceDir)
    {
        List<GameObject> fragmentsToBake = new List<GameObject>();
        foreach (GameObject fragment in fragments)
        {
            Rigidbody rb = fragment.GetComponent<Rigidbody>();
            MeshCollider meshCollider = fragment.GetComponent<MeshCollider>();
            
            rb.mass = meshCollider != null ? meshCollider.bounds.size.magnitude : 1f;
            rb.linearVelocity = Vector3.zero;
            rb.gameObject.layer = LayerMask.NameToLayer(layerOnHit);

            if (rb.mass > maxMassToDynamic)
            {
                fragmentsToBake.Add(rb.gameObject);
                rb.isKinematic = true;
                Destroy(rb);
            }
            else
            {
                rb.AddForceAtPosition( forceDir * force, forcePoint, ForceMode.Impulse);
            }
            
            RayfireRigid rigid = fragment.GetComponent<RayfireRigid>();
            RayfireShatter shatter = fragment.GetComponent<RayfireShatter>();
            
            Destroy(rigid);
            Destroy(shatter);
        }

        return fragmentsToBake;
    }

    private Rigidbody BakeObjects(List<GameObject> fragmentsToBake)
    {
        GameObject bakedObject = new GameObject($"{name}_Baked");
        bakedObject.transform.parent = transform.parent;
        
        Rigidbody bakedRb = bakedObject.AddComponent<Rigidbody>();
        bakedRb.isKinematic = false;
        bakedRb.mass = 1f;
        
        bakedObject.transform.position = transform.position;
        bakedObject.transform.rotation = transform.rotation;
        fragmentsToBake.ForEach(x => x.transform.SetParent(bakedObject.transform));
        
        foreach (SkinnedMeshRenderer rendsToBake in rendsToBake)
        {
            MeshRenderer bakedMesh = BakeSkinned(rendsToBake);
            bakedMesh.transform.SetParent(bakedObject.transform);
        }

        return bakedRb;
    }

    private MeshRenderer BakeSkinned(SkinnedMeshRenderer rend)
    {
        Mesh bakedMesh = new Mesh();
        rend.BakeMesh(bakedMesh);
            
        GameObject bakedMeshObject = new GameObject($"{rend.name}_Baked");
        bakedMeshObject.layer = LayerMask.NameToLayer(layerOnHit);
            
        MeshRenderer meshRenderer = bakedMeshObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = bakedMeshObject.AddComponent<MeshFilter>();
        MeshCollider meshCollider = bakedMeshObject.AddComponent<MeshCollider>();
            
        bakedMeshObject.layer = LayerMask.NameToLayer(layerOnHit);
        bakedMeshObject.transform.position = rend.transform.position;
        bakedMeshObject.transform.rotation = rend.transform.rotation;
            
        meshFilter.mesh = bakedMesh;
        meshRenderer.materials = rend.materials;
        meshCollider.convex = true;
        meshCollider.sharedMesh = bakedMesh;

        return meshRenderer;
    }
}
