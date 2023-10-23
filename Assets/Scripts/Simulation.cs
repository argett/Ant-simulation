using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    [Header("Simulation is the script that instantiate and controls everything")]
    public int nbAnts;
    public int nbFood;

    public static Vector2 bounds = new Vector2 (53, 30);

    public GameObject prefab_background = null;
    public GameObject prefab_ant = null;
    public GameObject prefab_food = null;
    public GameObject prefab_phero = null;

    public GameObject square;   // To debugging the Grid in the update()

    public static List<GameObject> pheromonesActivated = new List<GameObject>();    // Pheromones that are "alive"
    public static Queue<GameObject> pheromonesWaiting = new Queue<GameObject>();      // Pheromone that aren't "alive"

    // Vector2 is the center of the spawn point, the List is all the food corresponding to the spawn point
    public static (Vector2, List<GameObject>)[] allFoods = new (Vector2, List<GameObject>)[3];

    private void Awake()
    {
        //Application.targetFrameRate = 60;

        if (prefab_background != null && prefab_ant != null && prefab_food != null)
        {
            // food generation
            // there are 3 spawns
            allFoods[0] = (new Vector2(-20, 20), new List<GameObject>());
            allFoods[1] = (new Vector2(0, -25), new List<GameObject>());
            allFoods[2] = (new Vector2(-45, 27), new List<GameObject>());


            for (int i = 0; i < nbFood; i++)
            {
                GameObject foo = Instantiate(prefab_food);
                foo.name = "Food_" + i.ToString();
            }

            // ant generation
            for (int j = 0; j < nbAnts; j++)
            {
                GameObject ant = Instantiate(prefab_ant, new Vector3(4,0, 0), Quaternion.Euler(0f, 0f, 0));
                ant.name = "Ant_" + j.ToString();
            }

        }
        else { Debug.Log("/!\\ Manque un prefab dans Simulation /!\\"); }

        Grid.Init((int)Ant.viewDistance);
        //Grid.showGrid(GameObject.FindGameObjectWithTag("Grey"), GameObject.FindGameObjectWithTag("White"));
    }

    // Start is called before the first frame update
    void Start()
    {
        // pheromones pooling generation only now, because the list creation is in the Pheromones Awake()
        GameObject tmp;
        for (int phero = 0; phero < prefab_phero.GetComponent<Pheromone>().lifeDuration / prefab_phero.GetComponent<Pheromone>().lifeUpdate * (nbAnts + 1); phero++)
        {
            tmp = Instantiate(prefab_phero);
            tmp.SetActive(true);
            tmp.name = "Pheromone_" + phero.ToString();
            Simulation.pheromonesWaiting.Enqueue(tmp);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        (int x, int y) = Grid.wold2gridPos(ants[0].transform.position);
        square.transform.position = Grid.grid2worldPos(new Vector2 (x, y));
        */
        if (pheromonesActivated[0].GetComponent<Pheromone>().simulateDecrease() < 0f)
        {
            for (int i = 0; i < nbAnts; i++)
            {
                GameObject pheromone = Simulation.pheromonesActivated[0];
                Simulation.pheromonesWaiting.Enqueue(pheromone);    // the pheromone is "dead"
                Simulation.pheromonesActivated.Remove(pheromone);   // remove it from the "alive" list
                Grid.removePherotoGrid(pheromone);                  // dont forget to remove it from the grid -_-'
            }
        }

        foreach (GameObject pheromone in pheromonesActivated)
        {
            pheromone.GetComponent<Pheromone>().decreaseLife();
        }
    }
}
