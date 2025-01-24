using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class MeshDeformer : MonoBehaviour
    {
        public float radius = 2f; // Radius of deformation
        public float deformationStrength = 2f; // Intensity of deformation
        public float xDeformationScale = 0.5f; // Scale for X-axis deformation
        public float yDeformationScale = 1.5f; // Scale for positive Y-axis deformation

        private Mesh mesh;
        private Vector3[] vertices, modifiedVerts;

        private void Start()
        {
            // Initialize the mesh and vertices
            mesh = GetComponentInChildren<MeshFilter>().mesh;
            if (mesh == null)
            {
                Debug.LogError("Mesh not found! Ensure the object has a MeshFilter component.");
                return;
            }

            vertices = mesh.vertices;
            modifiedVerts = mesh.vertices;

            Debug.Log("MeshDeformer initialized. Vertex count: " + vertices.Length);
        }

        private void RecalculateMesh()
        {
            // Update the mesh and collider
            Debug.Log("Recalculating mesh...");
            mesh.vertices = modifiedVerts;
            var collider = GetComponentInChildren<MeshCollider>();
            if (collider == null)
            {
                Debug.LogWarning("MeshCollider not found! Adding a new MeshCollider.");
                collider = gameObject.AddComponent<MeshCollider>();
            }
            collider.sharedMesh = mesh;
            mesh.RecalculateNormals();
            Debug.Log("Mesh recalculated.");
        }

        private void OnTriggerStay(Collider other)
        {
            // Continuously deform while the scalpel is touching
            if (other.CompareTag("Scalpel"))
            {
                Debug.Log("Scalpel is within the trigger zone. Deforming mesh...");
                Vector3 contactPoint = transform.InverseTransformPoint(other.transform.position);
                Transform scalpelTransform = other.transform;
                DeformMesh(contactPoint, scalpelTransform);
            }
        }

        private void DeformMesh(Vector3 contactPoint, Transform scalpelTransform)
        {
            Debug.Log($"Deforming mesh at contact point: {contactPoint}");

            int deformationCount = 0; // Track how many vertices are deformed

            // Loop through vertices and apply deformation
            for (int v = 0; v < modifiedVerts.Length; v++)
            {
                Vector3 distance = modifiedVerts[v] - contactPoint;

                if (distance.sqrMagnitude < radius * radius)
                {
                    // Calculate the direction vector in local space relative to the scalpel
                    Vector3 direction = distance.normalized;

                    // Transform the direction to be relative to the scalpel
                    direction = scalpelTransform.InverseTransformDirection(direction);

                    // Prevent movement in the negative Y direction
                    if (direction.y < 0) direction.y = 0;

                    // Scale deformation components
                    direction.x *= xDeformationScale;
                    direction.y *= yDeformationScale;

                    // Transform the direction back to world space
                    direction = scalpelTransform.TransformDirection(direction);

                    // Apply deformation strength in the modified direction
                    float force = deformationStrength / (1f + distance.sqrMagnitude);
                    modifiedVerts[v] += direction * force;
                    deformationCount++;
                }
            }

            Debug.Log($"Total vertices deformed: {deformationCount}");
            if (deformationCount > 0)
            {
                RecalculateMesh();
            }
            else
            {
                Debug.LogWarning("No vertices were within the deformation radius.");
            }
        }
    }
}
