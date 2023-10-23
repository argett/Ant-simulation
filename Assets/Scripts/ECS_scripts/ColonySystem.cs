using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Random = Unity.Mathematics.Random;

namespace antsimu
{
    [BurstCompile]
    [UpdateAfter(typeof(InitializationSystemGroup))]
    public partial struct ColonySystem : ISystem
    {
        // A utiliser ? :
        // ecb.CreateCommandBuffer().AsParallelWriter()

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            UnityEngine.Debug.Log("Waiting for Properties to be created... --ColonySystem");
            state.RequireForUpdate<PheromoneProperties>();
            state.RequireForUpdate<AntProperties>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            ;
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            UnityEngine.Debug.Log("Spawning entities... --ColonySystem");
            // We spawn only the first frame

            // Pheromone spawning

            Entity PheromoneEntity = SystemAPI.GetSingletonEntity<PheromoneProperties>();
            var pheromone = SystemAPI.GetAspect<PheromoneAspect>(PheromoneEntity);
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            for (int i = 0; i < pheromone.colonyProperties.ValueRO.pheromonesNumber; i++) // je passe par pheromoneProperties pour acceder a Colony Properties pcque jesp comment y acceder directement
            {
                Entity newPheromone = ecb.Instantiate(pheromone.PheroPrefab);
                LocalTransform newPheromoneTransform = pheromone.initSpawn();
                ecb.SetComponent(newPheromone, newPheromoneTransform);
                ecb.AddComponent<PheromoneProperties>(newPheromone);
                ecb.SetName(newPheromone, "Pheromone_" + i.ToString());
            }

            // Ant spawning

            Entity AntEntity = SystemAPI.GetSingletonEntity<AntProperties>();
            AntAspect ant = SystemAPI.GetAspect<AntAspect>(AntEntity);
            for (int i = 0; i < pheromone.colonyProperties.ValueRO.antsNumber; i++)
            {
                Entity newAnt = ecb.Instantiate(ant.AntPrefab);
                ecb.AddComponent<AntProperties>(newAnt);
                ecb.SetName(newAnt, "Ant_" + i.ToString());
            }

            ecb.Playback(state.EntityManager);
            state.Enabled = false;
        }
    }

}
