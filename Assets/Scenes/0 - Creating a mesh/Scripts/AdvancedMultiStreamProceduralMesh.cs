using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering;
using Unity.Mathematics;
using static Unity.Mathematics.math;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AdvancedMultiStreamProceduralMesh : MonoBehaviour
{
    private void OnEnable()
    {
        // the simple API needs to convert the data provided to meshes to native formats at some point.
        // the advanced API bypasses the overhead by enabling direct operating on native memory.
        
        // mesh memory is split into regions
        // - vertex region
        //      composed of one/multiple streams of vertex data in the same format

        Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
        Mesh.MeshData meshData = meshDataArray[0];

        // define the layout for vertex attributes per vertex
        int vertexAttributesCount = 4;  // position, normal, tangent and uvs
        int vertexCount = 4;
        int triangleIndexCount = 6;
        var vertexAttributes = new NativeArray<VertexAttributeDescriptor>(vertexAttributesCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

        // describe every vertex attribute
        vertexAttributes[0] = new VertexAttributeDescriptor(dimension: 3);
        vertexAttributes[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3, stream: 1);
        vertexAttributes[2] = new VertexAttributeDescriptor(VertexAttribute.Tangent, dimension: 4, stream: 2);
        vertexAttributes[3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, dimension: 2, stream: 3);

        meshData.SetVertexBufferParams(vertexCount, vertexAttributes);  // allocate the vertex stream on meshData
        vertexAttributes.Dispose(); // no longer needed

        // actually set the vertices data now
        /* 
        After invoking SetVertexBufferParams we can retrieve native arrays for the vertex streams by invoking GetVertexData.
        The native array that it returns is in reality a pointer to the relevant section of the mesh data.
        So it acts like a proxy and there is no separate array.
        This would allow a job to directly write to the mesh data, skipping an intermediate copy step from native array to mesh data.
        */
        NativeArray<float3> positions = meshData.GetVertexData<float3>(0);
        positions[0] = 0;
        positions[1] = right();
        positions[2] = up();
        positions[3] = float3(1, 1, 0);

        NativeArray<float3> normals = meshData.GetVertexData<float3>(1);
        normals[0] = normals[1] = normals[2] = normals[3] = back();

        NativeArray<float4> tangents = meshData.GetVertexData<float4>(2);
        tangents[0] = tangents[1] = tangents[2] = tangents[3] = float4(1, 0, 0, -1);

        NativeArray<float2> uvs = meshData.GetVertexData<float2>(3);
        uvs[0] = 0;
        uvs[1] = float2(1, 0);
        uvs[2] = float2(0, 1);
        uvs[3] = 1;

        // need triangles indices
        meshData.SetIndexBufferParams(triangleIndexCount, IndexFormat.UInt16);

        var indices = meshData.GetIndexData<ushort>();
        indices[0] = 0;
        indices[1] = 2;
        indices[2] = 1;
        indices[3] = 1;
        indices[4] = 2;
        indices[5] = 3;

        // no idea what this does
        meshData.subMeshCount = 1;
        // theres shitty camera clipping most likely because the bounding box is fucked
        var bounds = new Bounds(new Vector3(0.5f, 0.5f), new Vector3(1, 1));

        meshData.SetSubMesh(0, new SubMeshDescriptor(0, triangleIndexCount)
        {
            bounds = bounds,
            vertexCount = vertexCount
        }, MeshUpdateFlags.DontRecalculateBounds);

        var mesh = new Mesh { bounds = bounds, name = "Procedural Mesh" };

        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
