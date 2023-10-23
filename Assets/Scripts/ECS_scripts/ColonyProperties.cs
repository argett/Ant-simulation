using UnityEngine;
using Unity.Entities;
using Unity.Properties;


namespace antsimu
{
    public struct ColonyProperties : IComponentData
    {
        public int antsNumber;
        public Vector2 mapSize;
        public int pheromonesNumber;
    }
}