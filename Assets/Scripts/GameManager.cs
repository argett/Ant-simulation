using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject dontDestroy;
    void Awake()
    {
        if (instance == null)            //Check if instance already exists
            instance = this;
    }
}
