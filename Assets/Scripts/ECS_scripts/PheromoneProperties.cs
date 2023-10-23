using UnityEngine;
using Unity.Entities;
using Unity.Properties;

namespace antsimu
{
    public struct PheromoneProperties : IComponentData
    {
        public Entity Pheromone;
        public float pheromoneDurability, initalScale;

        private bool type;                  // 0 = to Base, 1 = to Food
        private float scale;
    }

}