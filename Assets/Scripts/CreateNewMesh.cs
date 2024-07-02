using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNewMesh : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    [SerializeField] MeshCollider _meshCollider;
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = new Mesh();
        _skinnedMeshRenderer.BakeMesh(mesh);
        _meshCollider.sharedMesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
