using MonoBehaviuors;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace System
{
    public partial struct UnitMoverSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            UnitMoverJob unitMoverJob = new UnitMoverJob()
            {
                deltaTime = SystemAPI.Time.DeltaTime
            };
            unitMoverJob.ScheduleParallel();
            // foreach (var (localTransform, unitMover,physicsVelocity)in 
            //          SystemAPI.Query<RefRW<LocalTransform>, RefRO<UnitMover>,RefRW<PhysicsVelocity>>())
            // {
            //     float3 targetPosition = unitMover.ValueRO.targetPosition;
            //     float3 moveDirection = targetPosition - localTransform.ValueRO.Position;
            //     moveDirection = math.normalize(moveDirection);
            //     localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, quaternion.LookRotation(moveDirection, 
            //         math.up()), unitMover.ValueRO.rotationSpeed * SystemAPI.Time.DeltaTime);
            //     physicsVelocity.ValueRW.Linear = moveDirection * unitMover.ValueRO.moveSpeed;
            //     physicsVelocity.ValueRW.Angular= float3.zero;
            //     //localTransform.ValueRW.Position += moveDirection * moveSpeed.ValueRO.moveSpeed * SystemAPI.Time.DeltaTime;
            // }
        }

        [BurstCompile]
        public partial struct UnitMoverJob : IJobEntity
        {
            public float deltaTime;
            public void Execute(ref LocalTransform localTransform,in UnitMover unitMover,ref PhysicsVelocity physicsVelocity)
            {
                float3 targetPosition = unitMover.targetPosition;
                float3 moveDirection = targetPosition - localTransform.Position;
                float reachedTargetDistanceSq = 2f;
                //在精度不需要特别准确的情况下，使用平方距离进行比较可以避免开平方运算，提高性能
                if (math.lengthsq(moveDirection) < reachedTargetDistanceSq)
                {
                    physicsVelocity.Linear=float3.zero;
                    physicsVelocity.Angular=float3.zero;
                    return;
                }
                moveDirection = math.normalize(moveDirection);
                localTransform.Rotation = math.slerp(localTransform.Rotation, quaternion.LookRotation(moveDirection, 
                    math.up()), unitMover.rotationSpeed * deltaTime);
                physicsVelocity.Linear = moveDirection * unitMover.moveSpeed;
                physicsVelocity.Angular= float3.zero;
            }
        }
    }
}