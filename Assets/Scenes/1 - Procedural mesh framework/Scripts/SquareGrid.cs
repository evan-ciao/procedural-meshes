using UnityEngine;
using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace ProceduralMeshes.Generators
{
    public struct SquareGrid : IMeshGenerator
    {
        public int VertexCount => 4 * Resolution * Resolution;

        public int IndexCount => 6 * Resolution * Resolution;

        public int JobLength => Resolution;

        public Bounds Bounds => new Bounds(new Vector3(0.5f, 0.5f), new Vector3(1, 0, 1));  // bounds are glitched

        public int Resolution { get; set; }

        public void Execute<S>(int i, S streams) where S : struct, IMeshStreams
        {
            // -offset the indices (vertices, triangles) by the job index-

            // jobLength changed to just Resolution to enable vectorization through jobs
            // therefore offsets need an update
            int vi = 4 * Resolution * i;
            int ti = 2 * Resolution * i;

            // grid coordinates
            int z = i;

            // generate row per job
            for (int x = 0; x < Resolution; x++, vi += 4, ti += 2)
            {
                //var coordinates = float4(x, x + 1, z, z + 1);   // save space by storing all the relatives positions (up, right, ...)
                //coordinates = coordinates / Resolution - 0.5f;  // center the grid

                // splitted to optimize
                var xCoordinates = float2(x, x + 1f) / Resolution - 0.5f;   // subdivision of the surface
				var zCoordinates = float2(z, z + 1f) / Resolution - 0.5f;

                // generate the square grid we all know and love
                var vertex = new Vertex();
                vertex.normal.z = -1;
                vertex.tangent.xw = float2(1f, -1f);
                vertex.position.x = xCoordinates.x;
                vertex.position.z = zCoordinates.x;
                streams.SetVertex(vi + 0, vertex);

                vertex.position.x = xCoordinates.y;
                vertex.texcoord0 = float2(1f, 0f);
                streams.SetVertex(vi + 1, vertex);

                vertex.position.x = xCoordinates.x;
                vertex.position.z = zCoordinates.y;
                vertex.texcoord0 = float2(0f, 1f);
                streams.SetVertex(vi + 2, vertex);

                vertex.position.x = xCoordinates.y;
                vertex.texcoord0 = 1f;
                streams.SetVertex(vi + 3, vertex);

                streams.SetTriangle(ti + 0, vi + int3(0, 2, 1));
                streams.SetTriangle(ti + 1, vi + int3(1, 2, 3));
            }
        }
    }
}