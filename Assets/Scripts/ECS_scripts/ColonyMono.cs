using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace antsimu
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class ColonyMono : MonoBehaviour
    {
        public GameObject PheromonePrefab;
        public GameObject AntPrefab;
        public float2 MapSize;

        public float PheromoneLifeDuration = 10f;    // time duration of its live in second
        public int AntNumber;
        //public float lifeUpdate = 0.1f;     // step in seconds between two life and scale udpate
        //public int xGridCoord, yGridCoord;  // position of the pheromone in its Grid

        private Color green = Color.green;
        private Color blue = Color.blue;
        //private uint RandomSeed = (uint)Time.time; Ne fonctionne pas, doit etre mis dans le awake

        public class ColonyBaker : Baker<ColonyMono>
        {

            public override void Bake(ColonyMono authoring)
            {
                Debug.Log("Baking components... --ColonyMono");
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AntProperties
                {
                    Ant = GetEntity(authoring.AntPrefab, TransformUsageFlags.Dynamic)
                });

                AddComponent(entity, new PheromoneProperties
                {
                    Pheromone = GetEntity(authoring.PheromonePrefab, TransformUsageFlags.Dynamic),
                    pheromoneDurability = authoring.PheromoneLifeDuration,
                    initalScale = 1f
                });

                AddComponent(entity, new ColonyProperties
                {
                    pheromonesNumber = (int)Mathf.Ceil(authoring.PheromoneLifeDuration) * authoring.AntNumber,
                    antsNumber = authoring.AntNumber,
                    mapSize = authoring.MapSize
                });
            }
        }
    }

}