using ProceduralMeshes;
using ProceduralMeshes.Generators;
using ProceduralMeshes.Streams;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour
{
    // references to mesh jobs
    private static MeshJobScheduleDelegate[] jobs = {
        MeshJob<SquareGrid, SingleStream>.ScheduleParallel,
        MeshJob<SharedSquareGrid, SingleStream>.ScheduleParallel
    };

    public enum MeshType
    {
        SquareGrid,
        SharedSquareGrid
    }

    [SerializeField]
    private MeshType meshType;

    [SerializeField, Range(1, 10)]
    private int resolution = 1;
    
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

        //MeshJob<SquareGrid, SingleStream>.ScheduleParallel(mesh, meshData, resolution, default).Complete();
        jobs[(int)meshType](mesh, meshData, resolution, default).Complete();

        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
    }

    void Update()
    {
        GenerateMesh();
        enabled = false;
    }

    void OnValidate()
    {
        enabled = true;
    }
}