using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager1 : MonoBehaviour
{
    public static GameManager1 instance;
    public GameObject[] Ghosts;
    public float dificultad = 0.3f;
    int dX = 1, dZ = 1;
    int maxX = 5, maxZ = 5;
    int xA = -1,xB = 1,zA = -1,zB = 1,zS = 5;
    public GameObject wall,point,energy;
    public int[,] Casillas;

    // Start is called before the first frame update
    void Start()
    {
         if (instance == null)
         {
            instance =  this;
         }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Host(){

    }
    public void Stop(){
        NetworkManager.Singleton.Shutdown();
        //ButtonsPlay(false);
        Application.Quit();


    }

}
