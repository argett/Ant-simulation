using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Random = Unity.Mathematics.Random;

namespace antsimu
{
    [BurstCompile]
    public partial struct RandomSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            ;
        }

        [BurstCompile]
        public void OnStartRunning()
        {

            ;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            UnityEngine.Debug.Log("Randomizing ant... --RandomSystem");

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            uint cpt = 5;   // valeur aléatoire
            foreach (var (properties, antEntity) in SystemAPI.Query<RefRW<AntProperties>>().WithEntityAccess())
            {
                properties.ValueRW.randomizer = Random.CreateFromIndex(cpt*cpt);

                var antAspect = SystemAPI.GetAspect<AntAspect>(antEntity);
                LocalTransform newAntTransform = antAspect.initSpawn();
                ecb.SetComponent(antEntity, newAntTransform);

                cpt++;
            }
            ecb.Playback(state.EntityManager);
        }

        public void OnDestroy()
        {
            ;
        }
    }
}