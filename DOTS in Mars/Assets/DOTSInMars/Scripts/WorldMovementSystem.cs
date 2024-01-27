//using DOTSInMars;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.Burst;
//using Unity.Burst.Intrinsics;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Jobs.LowLevel.Unsafe;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;
//using UnityEngine;
//using UnityEngine.Diagnostics;

//namespace InMars
//{
//    [BurstCompile]
//    public partial struct WorldMovementSystem : ISystem
//    {
//        private EntityQuery centralComplexQuery;

//        public void OnCreate(ref SystemState state)
//        {
//            centralComplexQuery = SystemAPI.QueryBuilder()
//                .WithAllRW<CentralComplex, LocalTransform>().Build();
//            state.RequireForUpdate(centralComplexQuery);
//        }


//        [BurstCompile]
//        public void OnUpdate(ref SystemState state)
//        {
//            var deltaTime = SystemAPI.Time.DeltaTime;

//            var moveCentral = new MoveCentralComplexEntityJob
//            {
//                DeltaTime = deltaTime,
//                Direction = new float2(1, 0),
//                MovementSpeed = 1,
//                LocalTransformHandle = SystemAPI.GetComponentTypeHandle<LocalTransform>(),
//                CentralComplexHandle = SystemAPI.GetComponentTypeHandle<CentralComplex>()
//            };

//            var moveCentralHandle = moveCentral.ScheduleParallel(centralComplexQuery, state.Dependency);

//            state.Dependency = JobHandle.CombineDependencies(state.Dependency, moveCentralHandle);
//        }
//    }

//    [BurstCompile]
//    public partial struct MoveCentralComplexEntityJob : IJobChunk
//    {
//        public float DeltaTime;
//        public float2 Direction;
//        public float MovementSpeed;
//        public ComponentTypeHandle<LocalTransform> LocalTransformHandle;
//        public ComponentTypeHandle<CentralComplex> CentralComplexHandle;

//        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
//                in v128 chunkEnabledMask)
//        {
//            var chunkLocalTransformData = chunk.GetNativeArray(ref LocalTransformHandle);
//            var chunkCentralComplexData = chunk.GetNativeArray(ref CentralComplexHandle);

//            for (int i = 0; i < chunk.Count; i++)
//            {
//                var localTransform = chunkLocalTransformData[i];
//                var centralComplex = chunkCentralComplexData[i];

//                var movement = new float3(localTransform.Position.x + DeltaTime * Direction.x *
//                    MovementSpeed *
//                    centralComplex.Direction,
//                    localTransform.Position.y,
//                    localTransform.Position.y + DeltaTime * Direction.y * MovementSpeed);

//                localTransform.Position = movement;

//                if (localTransform.Position.x > 5)
//                {
//                    centralComplex.Direction = -1;
//                }
//                if (localTransform.Position.x < -5)
//                {
//                    centralComplex.Direction = 1;
//                }

//                // Write back to chunk data
//                {
//                    chunkLocalTransformData[i] = localTransform;
//                    chunkCentralComplexData[i] = centralComplex;
//                }
//            }
//        }
//    }
//}
