using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace antsimu
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct PheromoneSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            ;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            ;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ;
        }
    }

}