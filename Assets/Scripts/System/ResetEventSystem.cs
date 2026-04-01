using Unity.Burst;
using Unity.Entities;

namespace System
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct ResetEventSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var selected in  SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>())
            {
                selected.ValueRW.OnSelected = false;
                selected.ValueRW.OnDeSelected = false;
            }

           
        }
    }
}