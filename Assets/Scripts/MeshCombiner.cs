using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour
{
    [SerializeField] private bool combineOnStart = true;
    [SerializeField] private bool destroyOriginalChildren = true;
    [SerializeField] private bool useStaticBatching = false;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        if (combineOnStart) CombineMeshes();
    }

    public void CombineMeshes()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        List<MeshFilter> childMeshFilters = new List<MeshFilter>();
        GetChildMeshFilters(transform, childMeshFilters);

        if (childMeshFilters.Count == 0)
        {
            Debug.LogWarning("No child meshes found to combine.");
            return;
        }

        if (useStaticBatching)
        {
            CombineWithStaticBatching(childMeshFilters);
        }
        else
        {
            CombineManually(childMeshFilters);
        }

        if (destroyOriginalChildren) DestroyOriginalChildren();
    }

    private void GetChildMeshFilters(Transform parent, List<MeshFilter> meshFilters)
    {
        foreach (Transform child in parent)
        {
            MeshFilter childMeshFilter = child.GetComponent<MeshFilter>();
            if (childMeshFilter != null && childMeshFilter.sharedMesh != null)
            {
                meshFilters.Add(childMeshFilter);
            }

            GetChildMeshFilters(child, meshFilters);
        }
    }

    private void CombineManually(List<MeshFilter> meshFilters)
    {
        List<CombineInstance> combineInstances = new List<CombineInstance>();
        List<Material> materials = new List<Material>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            MeshRenderer renderer = meshFilter.GetComponent<MeshRenderer>();
            if (renderer == null) continue;

            for (int i = 0; i < meshFilter.sharedMesh.subMeshCount; i++)
            {
                CombineInstance combineInstance = new CombineInstance
                {
                    mesh = meshFilter.sharedMesh,
                    subMeshIndex = i,
                    transform = meshFilter.transform.localToWorldMatrix
                };
                combineInstances.Add(combineInstance);

                if (i < renderer.sharedMaterials.Length)
                {
                    materials.Add(renderer.sharedMaterials[i]);
                }
            }
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combineInstances.ToArray(), false);

        meshFilter.mesh = combinedMesh;

        if (materials.Count > 0)
        {
            meshRenderer.materials = materials.ToArray();
        }
    }

    private void CombineWithStaticBatching(List<MeshFilter> meshFilters)
    {
        List<GameObject> combineObjects = new List<GameObject>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter.gameObject != gameObject)
            {
                combineObjects.Add(meshFilter.gameObject);
            }
        }

        StaticBatchingUtility.Combine(combineObjects.ToArray(), gameObject);
    }

    private void DestroyOriginalChildren()
    {
        List<Transform> childrenToDestroy = new List<Transform>();

        foreach (Transform child in transform)
        {
            if (child.GetComponent<MeshFilter>() != null)
            {
                childrenToDestroy.Add(child);
            }
            else
            {
                DestroyOriginalChildrenRecursive(child, childrenToDestroy);
            }
        }

        foreach (Transform child in childrenToDestroy)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    private void DestroyOriginalChildrenRecursive(Transform parent, List<Transform> destroyList)
    {
        foreach (Transform child in parent)
        {
            if (child.GetComponent<MeshFilter>() != null)
            {
                destroyList.Add(child);
            }
            DestroyOriginalChildrenRecursive(child, destroyList);
        }
    }

    public void Uncombine()
    {
        if (meshFilter != null)
        {
            meshFilter.mesh = null;
        }
    }
}