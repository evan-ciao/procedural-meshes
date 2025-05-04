using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Rendering;
using Unity.Collections;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace ProceduralMeshes.Streams
{
    public struct SingleStream : IMeshStreams
    {
        [StructLayout(LayoutKind.Sequential)]
        struct Stream0
        {
            public float3 position;
            public float3 normal;
            public float4 tangent;
            public float2 texcoord0;
        }

        [NativeDisableContainerSafetyRestriction]
        NativeArray<Stream0> vertices;
        [NativeDisableContainerSafetyRestriction]
        NativeArray<int> indices;

        public void Setup(Mesh.MeshData meshData, Bounds bounds, int vertexCount, int indexCount)
        {
            // describe the vertex stream format for the meshData
            var descriptor = new NativeArray<VertexAttributeDescriptor>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            descriptor[0] = new VertexAttributeDescriptor(dimension: 3);
            descriptor[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3);
            descriptor[2] = new VertexAttributeDescriptor(VertexAttribute.Tangent, dimension: 4);
            descriptor[3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, dimension: 2);

            meshData.SetVertexBufferParams(vertexCount, descriptor);
            descriptor.Dispose();

            meshData.SetIndexBufferParams(indexCount, IndexFormat.UInt32);
            
            // update : added bounds
            meshData.SetSubMesh(0, new SubMeshDescriptor(0, indexCount) { bounds = bounds, vertexCount = vertexCount } , MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);

            // fill the buffer with the vertex data from the sub mesh
            vertices = meshData.GetVertexData<Stream0>();
            // fill the buffer with the indices from the sub mesh
            indices = meshData.GetIndexData<int>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertex(int index, Vertex vertex)
        {
            vertices[index] = new Stream0 {
                position = vertex.position,
                normal = vertex.normal,
                tangent = vertex.tangent,
                texcoord0 = vertex.texcoord0
            };
        }

        public void SetTriangle(int index, int3 triangle)
        {
            indices[index * 3] = triangle.x;
            indices[index * 3 + 1] = triangle.y;
            indices[index * 3 + 2] = triangle.z;
        }
    }
}