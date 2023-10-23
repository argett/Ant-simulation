using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace antsimu
{
    public readonly partial struct PheromoneAspect : IAspect
    {
        public readonly Entity Entity;

        //private readonly RefRO<AntProperties> _antsProperties;
        private readonly RefRO<PheromoneProperties> _pheromoneProperties;

        public readonly RefRO<ColonyProperties> colonyProperties;
        public readonly RefRW<LocalTransform> transformAspect;

        
        public Entity PheroPrefab => _pheromoneProperties.ValueRO.Pheromone;

        public LocalTransform initSpawn()
        {
            return new LocalTransform
            {
                Position = { x = colonyProperties.ValueRO.mapSize.x * 0.5f, y = colonyProperties.ValueRO.mapSize.y * 0.5f, z = 0 },
                Rotation = quaternion.identity,
                Scale = 1f
            };

            // change scale : ApplyScale(float x)
        }
    }

}