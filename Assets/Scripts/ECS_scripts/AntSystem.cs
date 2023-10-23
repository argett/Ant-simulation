using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace antsimu
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct AntSystem : ISystem
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

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ;
        }

        public void createRandomNormalizedVector(AntProperties ant)
        {
            float rand1 = ant.randomizer.NextFloat();
            float rand2 = ant.randomizer.NextFloat();
            float sum = rand1 + rand2;
            ant.randomVector = new float2(rand1 / sum, rand2 / sum);
        }
    }

}