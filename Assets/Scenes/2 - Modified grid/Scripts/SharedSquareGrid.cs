using UnityEngine;
using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace ProceduralMeshes.Generators
{
    public struct SharedSquareGrid : IMeshGenerator
    {
        // in a shared grid the vertex count is (R + 1)^2
        public int VertexCount => (Resolution + 1) * (Resolution + 1);

        public int IndexCount => 6 * Resolution * Resolution;

        public int JobLength => Resolution + 1;

        public Bounds Bounds => new Bounds(new Vector3(0.5f, 0.5f), new Vector3(2, 2, 2));

        public int Resolution { get; set; }

        public void Execute<S>(int i, S streams) where S : struct, IMeshStreams
        {
            // rows of vertices instead of rows of quads
            int vi = (Resolution + 1) * i;  // index of the first vertex (shouldnt this just be i? are we walking backwards? [R + 1 * i .. i]?)
                                            // im dumb. it is 0 for the first row and then it gradually grows. yes, its right.

            int ti = 2 * Resolution * (i - 1);

                // ...
            
            var vertex = new Vertex();
            vertex.normal.y = 1;
            vertex.tangent.xw = float2(1, -1);

            // the left-most vertex of the row
            vertex.position.x = -0.5f;
            vertex.position.z = (float)i / (float)Resolution /* <- 0..1 */ - 0.5f;
            // shared uvs
            vertex.texcoord0.x = 0;
            vertex.texcoord0.y = (float)i / Resolution;
            streams.SetVertex(vi, vertex);

            vi += 1;

            // to recap this loop that is a little esoteric
            // - after having created the left-most vertex for this very row jump to the next vertex in line
            // - set the position for this vertex
            // - create triangles with relatives offsets of nearby vertices (row below and current), add them to the current vertex's coords
            for (int x = 1; x <= Resolution; x++, vi++, ti += 2)
            {
                // set the vertices
                vertex.position.x = (float)x / Resolution - 0.5f;
                vertex.texcoord0.x = (float)x / Resolution;
                streams.SetVertex(vi, vertex);

                // set the triangles
                // relatives offsets

                // i modified the offsets a little because i wasn't sure the second was going in the right order
                
                if (i > 0)
                {
                    streams.SetTriangle(
                        ti, vi + int3(-Resolution - 2, -1, -Resolution - 1)
                    );
                    streams.SetTriangle(
                        ti + 1, vi + int3(-1, 0, -Resolution - 1)
                    );
                }
            }
        }
    }
}