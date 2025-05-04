using ProceduralMeshes;
using ProceduralMeshes.Generators;
using ProceduralMeshes.Streams;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour
{
    Mesh mesh;

	void Awake()
    {
		mesh = new Mesh { name = "Procedural Mesh" };
		GenerateMesh();
		GetComponent<MeshFilter>().mesh = mesh;
	}
	
	void GenerateMesh()
    {
        // allocate mesh data memory for C# jobs
        Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
        Mesh.MeshData meshData = meshDataArray[0];

        meshData.subMeshCount = 1;

        MeshJob<SquareGrid, SingleStream>.ScheduleParallel(mesh, meshData, default).Complete();

        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
    }
}