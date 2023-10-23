using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public static class Grid
{
    /** The grid is filled with squares but not necessarily with the same amount in X and Y direction.
     * The center of the grid is 0,0 in the world space. The 0,0 of the grid is in the bottom left-hand corner
     * If the number of tile on the colums or row is odd, then the last is at the top or right of the grid
     **/
    public static List<GameObject>[,] pheroGridtoFood;
    public static List<GameObject>[,] pheroGridtoBase;

    private static int tileSize;
    private static int nbTileX, nbTileY;
    private static float offsetX, offsetY;

    // Start is called before the first frame update
    public static void Init(int viewDistance)
    {
        tileSize = viewDistance;
        nbTileX = (int)Mathf.Ceil(Simulation.bounds.x*2 / tileSize);
        nbTileY = (int)Mathf.Ceil(Simulation.bounds.y*2 / tileSize);
        Debug.Log("Generation de la grille d'optimisation des pheromones...");
        Debug.Log("tileSize = " + tileSize);
        Debug.Log("nbTile = " + nbTileX + ", " + nbTileY);

        offsetX = Simulation.bounds.x * 2 - (int)Mathf.Floor(Simulation.bounds.x * 2 / tileSize) * tileSize;
        offsetY = Simulation.bounds.y * 2 - (int)Mathf.Floor(Simulation.bounds.y * 2 / tileSize) * tileSize;
        Debug.Log("Offset = " + offsetX + ", " + offsetY);


        // Generation of the grid
        pheroGridtoFood = new List<GameObject>[nbTileX, nbTileY];
        pheroGridtoBase = new List<GameObject>[nbTileX, nbTileY];

        for (int i = 0; i<nbTileX; i++)
        {
            for(int j = 0; j<nbTileY; j++)
            {
                pheroGridtoFood[i, j] = new List<GameObject>();
                pheroGridtoBase[i, j] = new List<GameObject>();
            }
        }
    }
    
    public static void addPherotoFood(GameObject pheromone)
    {
        (int gridPosX, int gridPosY) = Grid.wold2gridPos(pheromone.transform.position);
        pheromone.GetComponent<Pheromone>().xGridCoord = gridPosX;
        pheromone.GetComponent<Pheromone>().yGridCoord = gridPosY;
        pheroGridtoFood[gridPosX, gridPosY].Add(pheromone);        
    }

    public static void addPherotoBase(GameObject pheromone)
    {
        (int gridPosX, int gridPosY) = Grid.wold2gridPos(pheromone.transform.position);
        pheromone.GetComponent<Pheromone>().xGridCoord = gridPosX;
        pheromone.GetComponent<Pheromone>().yGridCoord = gridPosY;
        pheroGridtoBase[gridPosX, gridPosY].Add(pheromone);
    }

    public static void removePherotoGrid(GameObject pheromone)
    {

        if (pheromone.GetComponent<Pheromone>().getType() == true)
        {
            pheroGridtoFood[pheromone.GetComponent<Pheromone>().xGridCoord, pheromone.GetComponent<Pheromone>().yGridCoord].Remove(pheromone);
        }
        else
        {
            pheroGridtoBase[pheromone.GetComponent<Pheromone>().xGridCoord, pheromone.GetComponent<Pheromone>().yGridCoord].Remove(pheromone);
        }
    }

    /**
     * Check the 8 squares around the ant + the one where the ant is
     **/
    public static List<GameObject> getFoodPheromonesAround(GameObject ant)
    {
        List<GameObject> pheromonesAround = new List<GameObject>();

        (int tileX, int tileY) = Grid.wold2gridPos(ant.transform.position);
        int currXtile, currYtile;

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                currXtile = tileX + i;
                currYtile = tileY + j;
                if (0 <= currXtile && currXtile < nbTileX && 0 <= currYtile && currYtile < nbTileY)
                {
                    pheromonesAround.AddRange(pheroGridtoFood[currXtile, currYtile]);
                }
            }
        }

        return pheromonesAround;
    }
    public static List<GameObject> getBasePheromonesAround(GameObject pheromone)
    {
        List<GameObject> pheromonesAround = new List<GameObject>();
        int currXtile, currYtile;

        (int tileX, int tileY) = Grid.wold2gridPos(pheromone.transform.position);

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                currXtile = tileX + i;
                currYtile = tileY + j;
                if (0 <= currXtile && currXtile < nbTileX && 0 <= currYtile && currYtile < nbTileY)
                {
                    pheromonesAround.AddRange(pheroGridtoBase[currXtile, currYtile]);
                }
            }
        }
        return pheromonesAround;
    }

    public static void showGrid(GameObject greyGrid, GameObject whiteGrid)
    {
        greyGrid.transform.localScale = new Vector3(tileSize, tileSize, 1);
        whiteGrid.transform.localScale = new Vector3(tileSize, tileSize, 1);

        for (int col = 0; col < nbTileX; col++)
        {
            for (int row = 0; row < nbTileY; row++)
            {
                Vector2 pos = new Vector2(col * tileSize - (nbTileX * tileSize) / 2f + tileSize/2f, row * tileSize - (nbTileY * tileSize) / 2f + tileSize / 2f);
                if ((col+row) % 2 == 0)
                {
                    GameObject.Instantiate(whiteGrid, pos, Quaternion.identity);
                }
                else
                {
                    GameObject.Instantiate(greyGrid, pos, Quaternion.identity);
                }
            }
        }
    }

    public static (int, int) wold2gridPos(Vector2 worldPos)
    {
        float X = (worldPos.x + (nbTileX * tileSize) / 2f) / tileSize;
        float Y = (worldPos.y + (nbTileY * tileSize) / 2f) / tileSize;

        return ((int)X, (int)Y);
    }

    public static Vector2 grid2worldPos(Vector2 gridPos)
    {
        float X = tileSize * (gridPos.x - (nbTileX / 2f) + 0.5f);
        float Y = tileSize * (gridPos.y - (nbTileY / 2f) + 0.5f);

        return new Vector2 (X, Y);

    }
}
