using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SimpleProceduralMesh : MonoBehaviour
{
    private void OnEnable()
    {
        var mesh = new Mesh { name = "Procedural Mesh" };

        GenerateQuad(ref mesh);
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void GenerateTriangle(ref Mesh mesh)
    {
        mesh.vertices = new Vector3[] {
            Vector3.zero,
            Vector3.right,
            Vector3.up
        };

        // clock-wise vertices determine front faces
        mesh.triangles = new int[] { 0, 2, 1 };

        mesh.normals = new Vector3[] {
            Vector3.back,
            Vector3.back,
            Vector3.back
        };

        mesh.uv = new Vector2[] {
            Vector2.zero,
            Vector2.right,
            Vector2.up
        };

        // tangents are used to convert normal-maps from texture-space to world-space
        // only the right axis is specified, the third axis needed is going to be calculated through a cross product with the normal and right vector.
        // we only need to specify the direction of the third axis (-1 or 1).
        mesh.tangents = new Vector4[] {
            new Vector4(1, 0, 0, -1),
            new Vector4(1, 0, 0, -1),
            new Vector4(1, 0, 0, -1),
        };
    }

    private void GenerateQuad(ref Mesh mesh)
    {
        mesh.vertices = new Vector3[] {
            Vector3.zero,
            Vector3.right,
            Vector3.up,
            new Vector3(1, 1, 0)
        };

        // clock-wise vertices determine front faces
        mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };

        mesh.normals = new Vector3[] {
            Vector3.back,
            Vector3.back,
            Vector3.back,
            Vector3.back
        };

        mesh.uv = new Vector2[] {
            Vector2.zero,
            Vector2.right,
            Vector2.up,
            new Vector2(1, 1)
        };

        // tangents are used to convert normal-maps from texture-space to world-space
        // only the right axis is specified, the third axis needed is going to be calculated through a cross product with the normal and right vector.
        // we only need to specify the direction of the third axis (-1 or 1).
        mesh.tangents = new Vector4[] {
            new Vector4(1, 0, 0, -1),
            new Vector4(1, 0, 0, -1),
            new Vector4(1, 0, 0, -1),
            new Vector4(1, 0, 0, -1)
        };
    }
}