using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Pheromone : MonoBehaviour
{
    public float lifeDuration = 10f;    // time duration of its live in second
    public float lifeUpdate = 0.1f;     // step in seconds between two life and scale udpate

    public int xGridCoord, yGridCoord;  // position of the pheromone in its Grid

    private float initialDuration, initalScale;
    private float scale;

    private bool type;                  // 0 = to Base, 1 = to Food
    private Color green = Color.green;
    private Color blue = Color.blue;

    private void Awake()
    {
        this.initialDuration = this.lifeDuration;
        this.scale = this.initalScale = this.transform.localScale.x;
    }

    private IEnumerator updatePheromones()
    {
        while (this.lifeDuration > 0)
        {
            scale = initalScale * lifeDuration / initialDuration;
            this.transform.localScale = new Vector3(scale, scale, 1);

            float delay = Random.Range(0f, 0.2f); // the delay allows to not update every pheromone at te same frame each time

            this.lifeDuration -= (this.lifeUpdate + delay);
            yield return new WaitForSeconds(this.lifeUpdate + delay);
        }
        Simulation.pheromonesWaiting.Enqueue(this.gameObject);      // the pheromone is "dead"
        Simulation.pheromonesActivated.Remove(this.gameObject); // remove it from the "alive" list
        Grid.removePherotoGrid(this.gameObject);                // dont forget to remove it from the grid -_-'
    }

    /**
     * Makes spawn "toFood" pheromones, in the corresponding grid
     **/
    public void spawntoFood(Vector2 position)
    {
        resetState(position);
        this.type = true;
        this.GetComponent<SpriteRenderer>().color = green;
        Grid.addPherotoFood(this.gameObject);
        StartCoroutine(updatePheromones());
    }

    /**
     * Makes spawn "toBase" pheromones, in the corresponding grid
     **/
    public void spawntoBase(Vector2 position)
    {
        resetState(position);
        this.type = false;
        this.GetComponent<SpriteRenderer>().color = blue;
        Grid.addPherotoBase(this.gameObject);
        StartCoroutine(updatePheromones());
    }

    private void resetState(Vector2 position)
    {
        Simulation.pheromonesWaiting.Dequeue();
        this.lifeDuration = this.initialDuration;
        this.scale = this.initalScale;
        this.transform.position = new Vector3(position.x, position.y, 0);
        Simulation.pheromonesActivated.Add(this.gameObject);
    }

    public bool getType()
    {
        return this.type;
    }

    /*
    void Update()
    {
        //this.transform.position = new Vector2 ((Input.mousePosition.x-800) / 10, (Input.mousePosition.y - 400) / 10);
    }
    */
}
