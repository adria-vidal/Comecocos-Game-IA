using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Buttons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void host(){
        NetworkManager.Singleton.StartHost();
    }
    public void Client(){
        NetworkManager.Singleton.StartClient();
    }
    public void Server(){
        NetworkManager.Singleton.StartServer();
    }

    
}
