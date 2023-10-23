using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace antsimu
{
    public readonly partial struct AntAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<AntProperties> _antsProperties;
        private readonly RefRW<LocalTransform> _transformAspect;

        public Entity AntPrefab => _antsProperties.ValueRO.Ant;
        public Random randomizer => _antsProperties.ValueRW.randomizer;

        public LocalTransform initSpawn()
        {
            float2 nx = randomizer.NextFloat2(-2, 2);
            float nr = randomizer.NextInt(-10, 10);
            return new LocalTransform
            {
                Position = { x = nx.x, y = nx.y, z = 0 }, // TODO : a changer par la fourmilliere
                Rotation = quaternion.RotateZ(nr),
                Scale = 1f
            };

            // change scale : ApplyScale(float x)
        }
    }

}