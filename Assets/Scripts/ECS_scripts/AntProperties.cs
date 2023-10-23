using Unity.Entities;
using Unity.Mathematics;


namespace antsimu
{

    public struct AntProperties : IComponentData
    {
        public Entity Ant;
        public Random randomizer;
        public float randomFloat => randomizer.NextFloat();
        public float2 randomVector;
    }
}