using UnityEngine;
using Unity.Mathematics;

namespace ProceduralMeshes
{
    public struct Vertex
    {
        public float3 position;
        public float3 normal;
        public float4 tangent;
        public float2 texcoord0;
    }
}