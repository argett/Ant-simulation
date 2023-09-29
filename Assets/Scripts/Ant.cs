using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Ant : MonoBehaviour
{
    public float speed = 8;
    public float exploration = 0.1f;    // how much the explorate changes
    public float inertia = 0.8f;        // how much the ant explorate
    public static float viewDistance = 5;      // how far the ant see the food
    public int turnAngle = 10;          // turn angle in degree

    private Vector2 deviation;          // the direction of the randomness / exploration
    private Vector2 momentum, acceleration;
    private Vector2 bounds;             // TO DELETE AGAINST RAY CASTING, bounds of the map
    private GameObject food;            // The food chosen / taken vu the ant
    private GameObject fourmilliere;    // Ant's base
    private GameObject sensor;          // Position de la tete de la fourmi, là où elle regarde ou sont les pheromones
    private bool gotoBase;
    private float[] rotationMatrix = new float[4];  // to rotate the ant to the food

    private void Awake()
    {
        this.fourmilliere = GameObject.FindGameObjectWithTag("Respawn");
        this.transform.position = fourmilliere.transform.position;
        this.gotoBase = false;
        this.bounds = Simulation.bounds;
        this.sensor = this.transform.Find("tete").gameObject; // no need to get both sensors
        this.food = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.momentum = Vector2.zero;
        this.deviation = Random.insideUnitCircle;
        this.acceleration = Random.insideUnitCircle;
        this.rotationMatrix[0] = Mathf.Cos(this.turnAngle * Mathf.Deg2Rad);
        this.rotationMatrix[1] = Mathf.Sin(this.turnAngle * Mathf.Deg2Rad);
        this.rotationMatrix[2] = Mathf.Cos(-this.turnAngle * Mathf.Deg2Rad);
        this.rotationMatrix[3] = Mathf.Sin(-this.turnAngle * Mathf.Deg2Rad);

        StartCoroutine(dropPheromones());
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movingDir = this.acceleration;

        if (food == null)       // still searching food
        {
            findFood();
            if (food == null)   // follow pheromones to food
            {
                movingDir = this.sensor.GetComponent<Sensor>().updateSensors(true, this.rotationMatrix);
            }
        }
        else if (this.gotoBase) // having food, returning home
        {
            float spawnDist = (this.transform.position - this.fourmilliere.transform.position).magnitude;
            if (spawnDist < 2f) // has reached home
            {
                dropFood();
                this.acceleration = -this.acceleration; // demi tour
            }
            else if (spawnDist < 5f)
            {
                movingDir = fourmilliere.transform.position - this.transform.position;
            }
            else
            {
                movingDir = this.sensor.GetComponent<Sensor>().updateSensors(false, this.rotationMatrix);
            }
        }
        else    // found food close
        {
            Vector2 toFood = this.food.transform.position - this.transform.position;
            if (toFood.magnitude < 0.3f)   // on the food
            {
                grabFood();
            }
            else
            {
                if (foodStillReachable())
                {
                    movingDir = toFood;
                }
                else
                {
                    this.food = null;
                }
            }
        }

        mooveAnt(movingDir);
    }

    void mooveAnt(Vector2 direction)
    {
        // chose the direction of the next step
        setDirection(direction);

        // compute the T+1 position
        float timeSpeed = Time.deltaTime * this.speed;
        Vector2 antNewPosition = new Vector2(this.transform.position.x + this.acceleration.x * timeSpeed,
                                     this.transform.position.y + this.acceleration.y * timeSpeed);

        // Be sure to not go out of the camera
        antNewPosition = checkBounds(antNewPosition);

        momentum = new Vector2(antNewPosition.x - this.transform.position.x, antNewPosition.y - this.transform.position.y);
        momentum.Normalize();

        // rotate the ant forward, the real formula is (momentum.x * 0 + momentum.y * 1)
        float alpha = Mathf.Acos(momentum.y) * Mathf.Rad2Deg;
        // make the ant go to forward
        if (alpha > 0.01f)
        {
            if (this.acceleration.x > 0)
                alpha = -alpha;

            this.transform.SetPositionAndRotation(antNewPosition, Quaternion.Euler(0f, 0f, alpha));
        }
        else    // if the angle is to small
            this.transform.SetPositionAndRotation(antNewPosition, Quaternion.Euler(0f, 0f, 0));
    }

    private void setDirection(Vector2 dir)
    {
        this.deviation = this.deviation * this.exploration + Random.insideUnitCircle * (1 - this.exploration);
        this.deviation.Normalize();

        // merge the desired direction and the randomness
        this.acceleration += dir.normalized * this.inertia + this.deviation * (1 - this.inertia);
        this.acceleration.Normalize();
    }

    private void findFood()
    {
        float minDist = 99999;
        // check each food spawn point
        for (int i = 0; i < Simulation.allFoods.Length; i++)
        {
            // if the ant is near the food spawn point, then compare it to all foods
            if ((new Vector2(this.transform.position.x, this.transform.position.y) - Simulation.allFoods[i].Item1).magnitude < 10)
            {
                //Debug.Log(this.name + " check spawn " + i);
                foreach (GameObject food in Simulation.allFoods[i].Item2)
                {
                    if (food.GetComponent<Food>().canBeFound())
                    {
                        float dist = (this.transform.position - food.transform.position).magnitude;
                        if (dist < minDist && dist < viewDistance)
                        {
                            minDist = dist;
                            this.food = food;
                        }
                    }
                }
            }
        }
    }

    private void grabFood()
    {
        if (this.food.GetComponent<Food>().canBeFound())
        {
            Simulation.allFoods[this.food.GetComponent<Food>().getSpawnIndex()].Item2.Remove(this.food.gameObject);
            this.food.transform.SetParent(this.transform.Find("tete"));
            this.food.transform.localPosition = Vector3.zero;
            this.food.GetComponent<Food>().setFound();
            this.acceleration = -this.acceleration; // demi tour
            this.gotoBase = true;
        }
        else
        {
            this.food = null;
        }
    }

    private void dropFood()
    {
        Destroy(this.food);
        this.gotoBase = false;
        this.food = null;

        UIText.addFood();
    }

    private bool foodStillReachable()
    {
        return this.food.GetComponent<Food>().canBeFound();
    }

    private Vector2 checkBounds(Vector2 position)
    {
        // 1 because normed vector
        if (position.x > this.bounds.x)
        {
            this.deviation.x = -1;
            this.acceleration.x = -1;
            position.x = this.bounds.x - 0.2f;
        }
        else if (position.y > this.bounds.y)
        {
            this.deviation.y = -1;
            this.acceleration.y = -1;
            position.y = this.bounds.y - 0.2f;
        }
        else if (position.x < -this.bounds.x)
        {
            this.deviation.x = 1;
            this.acceleration.x = 1;
            position.x = -this.bounds.x + 0.22f;
        }
        else if (position.y < -this.bounds.y)
        {
            this.deviation.y = 1;
            this.acceleration.y = 1;
            position.y = -this.bounds.y + 0.2f;
        }

        return position;
    }

    private IEnumerator dropPheromones()
    {
        while(true)
        {
            GameObject pheromoneWaitingtoSpawn = null;
            if(Simulation.pheromonesWaiting.TryPeek(out pheromoneWaitingtoSpawn))
            {
                // ants should not be out of bounds but sometimes it happens for some frames
                if (this.gotoBase)
                {
                    try
                    {
                        pheromoneWaitingtoSpawn.GetComponent<Pheromone>().spawntoFood(this.transform.position);
                    }
                    catch
                    {
                        Debug.Log(this.name + " est sorti du cadre");
                    }
                }
                else
                {
                    try
                    {
                        pheromoneWaitingtoSpawn.GetComponent<Pheromone>().spawntoBase(this.transform.position);
                    }
                    catch
                    {
                        Debug.Log(this.name + " est sorti du cadre");
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}