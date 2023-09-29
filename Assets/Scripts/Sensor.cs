using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public GameObject sensorL, sensorR, sensorM;

    private float angleLimit, cosinus, dotRotated90; 
    private float leftPheroPow, rightPheroPow, frontPheroPow; // how much pheromone power receive the sensor
    private float viewDistance;     // how far the sensors detects 
    private bool closeFood;         // is there some pheromone not too far ?
    private Vector3 ant_position;   // the call to parent.transform.position needs some resource 

    // Start is called before the first frame update
    void Start()
    {
        this.angleLimit = Mathf.Sqrt(2) / 1.5f;
        this.viewDistance = Ant.viewDistance - 0.2f;
    }

    /**
     *  Sert a savoir de quel cote la fourmis doit tourner en fonction de l'intensite des pheromones autour d'elle
     */
    public Vector2 updateSensors(bool toFood, float[] rotMatrix)
    {
        List<GameObject> closePheromones;
        ant_position = this.transform.parent.transform.position;

        if (toFood)
        {
            closePheromones = Grid.getFoodPheromonesAround(this.transform.parent.gameObject);
        }
        else
        {
            closePheromones = Grid.getBasePheromonesAround(this.transform.parent.gameObject);
        }

        resetValues();

        Vector2 forward = (this.transform.position - ant_position).normalized;
        (frontPheroPow, rightPheroPow, leftPheroPow) = computeStrength(closePheromones, forward);

        if (closePheromones.Count > 0 && closeFood)
        {
            if (frontPheroPow > leftPheroPow && frontPheroPow > rightPheroPow)
            {
                return forward;
            }
            else if (rightPheroPow > leftPheroPow) // right, turn clockwise
            {
                float X = forward.x * rotMatrix[2] - forward.y * rotMatrix[3];
                float Y = forward.x * rotMatrix[3] + forward.y * rotMatrix[2];
                return new Vector2(X, Y);
            }
            else    // left, turn anti-clockwise
            {
                float X = forward.x * rotMatrix[0] - forward.y * rotMatrix[1];
                float Y = forward.x * rotMatrix[1] + forward.y * rotMatrix[0];
                return new Vector2 (X, Y);
            }
        }
        return Vector2.zero;
    }

    /**
     *  Calcul la somme des intensites des pheromones qui sont a bonne distance a droite de la fourmi, devant et a gauche
     */
    private (float, float, float) computeStrength(List<GameObject> pheroList, Vector2 forward)
    {
        float front = 0, right = 0, left = 0;
        Vector2 toPheromone;

        foreach (GameObject pheromone in pheroList)
        {
            toPheromone = pheromone.transform.position - ant_position;

            if (toPheromone.magnitude < viewDistance)
            {
                toPheromone.Normalize();

                cosinus = forward.x * toPheromone.x + forward.y * toPheromone.y;
                // la dotRotated permet de savoir si le pheromone est a gauche ou a droite du vecteur forward
                dotRotated90 = forward.x * -toPheromone.y + forward.y * toPheromone.x;

                if (cosinus >= 0) // front
                {
                    closeFood = true;
                    if (cosinus > angleLimit)
                    {
                        front += pheromone.GetComponent<Pheromone>().lifeDuration;
                    }
                    else if (dotRotated90 > 0)
                    {
                        right += pheromone.GetComponent<Pheromone>().lifeDuration;
                    }
                    else
                    {
                        left += pheromone.GetComponent<Pheromone>().lifeDuration;
                    }
                }
            }
        }

        return (front, right, left);
    }

    private void resetValues()
    {
        closeFood = false;

        leftPheroPow = 0;
        rightPheroPow = 0;
        frontPheroPow = 0;
    }
}
