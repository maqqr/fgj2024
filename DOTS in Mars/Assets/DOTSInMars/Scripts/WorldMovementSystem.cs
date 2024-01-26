using DOTSInMars;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace InMars
{
    [BurstCompile]
    public partial struct WorldMovementSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            var centralComplexEntities = SystemAPI.QueryBuilder()
                .WithAllRW<CentralComplex, LocalTransform>().Build();

            var moveCentral = new MoveCentralComplexEntityJob
            {
                DeltaTime = deltaTime,
                Direction = new float2(1, 0)
            };

            var moveCentralHandle = moveCentral.ScheduleParallel(centralComplexEntities, state.Dependency);

            state.Dependency = JobHandle.CombineDependencies(state.Dependency, moveCentralHandle);
        }
    }

    [BurstCompile]
    public partial struct MoveCentralComplexEntityJob : IJobEntity
    {
        public float DeltaTime;
        public float2 Direction;

        private void Execute(in CentralComplex complex, ref LocalTransform transform)
        {
            transform.Position.x *= DeltaTime * Direction.x;
            transform.Position.y *=  DeltaTime * Direction.y;
        }
    }
}
