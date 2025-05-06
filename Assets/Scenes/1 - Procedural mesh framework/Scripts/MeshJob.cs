using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

namespace ProceduralMeshes
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct MeshJob<G, S> : IJobFor
    where G : struct, IMeshGenerator
    where S : struct, IMeshStreams
    {
        private G generator;
        private S streams;
        
        public void Execute(int index)
        {
            // just execute the generator and leave it be
            generator.Execute(index, streams);

            // still not sure about this index tho...
            // I guess its just an index within a chain of executions.
            // we wait for the last job to finish before starting our own.
            // since mesh generation is tailored for parallel execution we shouldnt have problems.
        }

        public static JobHandle ScheduleParallel (
            Mesh mesh, Mesh.MeshData meshData, int resolution, JobHandle dependency
        )
        {
            var job = new MeshJob<G, S>();
            job.generator.Resolution = resolution;
            job.streams.Setup(meshData, mesh.bounds = job.generator.Bounds, job.generator.VertexCount, job.generator.IndexCount);

            return job.ScheduleParallel(job.generator.JobLength, 1, dependency);
        }
    }
}