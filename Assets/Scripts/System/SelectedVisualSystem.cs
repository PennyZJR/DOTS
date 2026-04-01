using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace System
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateBefore(typeof(ResetEventSystem))]
    public partial struct SelectedVisualSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
            foreach (var selected in SystemAPI.Query<RefRO<Selected>>().WithPresent<Selected>())
            {
                if (selected.ValueRO.OnSelected)
                {
                    var visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntitiy);
                    visualLocalTransform.ValueRW.Scale = selected.ValueRO.showScale;
                }

                if (selected.ValueRO.OnDeSelected)
                {
                    //这么写破坏了ECS的内存连续性优势，但是通过事件系统进行调用，不会每帧进行获取，所以性能影响不大
                    var visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntitiy);
                    visualLocalTransform.ValueRW.Scale = 0f;
                }
            }
        }
        
    }
}