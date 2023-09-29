using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    /**
     * Each food is in a list, and there are as many list as many spawn point, stored in the Simulation class
     * Each time an ant put its food in the anthill, the food is removed from its list
     * */

    private bool notFoundYet;
    private int spawnPointIndex;

    private void Awake()
    {
        int spawnPoint = Random.Range(0, 3);

        if (spawnPoint == 0)
        {
            this.transform.position = new Vector2(normalizedRandom(-25, -15), normalizedRandom(15, 25));
            Simulation.allFoods[0].Item2.Add(this.gameObject);
            this.spawnPointIndex = 0;
        }
        else if (spawnPoint == 1)
        {
            this.transform.position = new Vector2(normalizedRandom(-2, 2), normalizedRandom(-28, -22));
            Simulation.allFoods[1].Item2.Add(this.gameObject);
            this.spawnPointIndex = 1;
        }
        else
        {
            this.transform.position = new Vector2(normalizedRandom(-50, -40), normalizedRandom(25, 29));
            Simulation.allFoods[2].Item2.Add(this.gameObject);
            this.spawnPointIndex = 2;
        }
        notFoundYet = true;
    }

    /**
     * Genere une zone de spawn des pheromones
     **/
    private float normalizedRandom(int minVal, int maxVal)
    {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minVal + maxVal) / 2.0f;
        float sigma = (maxVal - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minVal, maxVal);
    }

    public void setFound()
    {
        notFoundYet=false;
    }

    public bool canBeFound()
    {
        return notFoundYet;
    }

    public int getSpawnIndex()
    {
        return spawnPointIndex;
    }
}
