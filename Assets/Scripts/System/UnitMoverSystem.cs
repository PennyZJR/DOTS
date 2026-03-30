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
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (localTransform, unitMover,physicsVelocity)in 
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<UnitMover>,RefRW<PhysicsVelocity>>())
            {
                float3 targetPosition = MouseWorldPosition.Instance.GetPosition();
                float3 moveDirection = targetPosition - localTransform.ValueRO.Position;
                moveDirection = math.normalize(moveDirection);
                localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, quaternion.LookRotation(moveDirection, math.up()), unitMover.ValueRO.rotationSpeed * SystemAPI.Time.DeltaTime);
                physicsVelocity.ValueRW.Linear = moveDirection * unitMover.ValueRO.moveSpeed;
                physicsVelocity.ValueRW.Angular= float3.zero;
                //localTransform.ValueRW.Position += moveDirection * moveSpeed.ValueRO.moveSpeed * SystemAPI.Time.DeltaTime;
            }
        }
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}