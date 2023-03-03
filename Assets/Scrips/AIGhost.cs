using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class AIGhost : NetworkBehaviour
{
    public NavMeshAgent agent;
    public GameObject player;
    public Rigidbody rb;

    //////////////NETWORK VARIABLES//////////
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();    
    public NetworkVariable<bool> eatable = new NetworkVariable<bool>();    
    public Material normalMaterial,eatableMaterial;
    

    /// If the object is a server, it will start the coroutine EatableEnergy, if not, it will start the
    /// coroutine StartCoroutineResquestServerRpc.
    public void Eatable(){
        if (IsServer)
        {
            StartCoroutine(EatableEnergy());
        }else{
            //Como es una IA, lo más probable es que no entre
            StartCoroutineResquestServerRpc();
        }
    }

    /// If there are players, return the closest one, otherwise return a random position
    Vector3 PresaCercana(){
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0){
            float distance = 100f;
            foreach (GameObject pj in players)
            {
                float dAux = Vector3.Distance(pj.transform.position,transform.position);
                if (dAux<distance)
                {
                    distance = dAux;
                    player = pj;
                }
            }
            return new Vector3(player.transform.position.x, 0, player.transform.position.z);
        }else{
            //Sino tenemos jugadores
            if (GameObject.Find("Plataforma"))
            {
                return new Vector3(Random.Range(-25,25),0,Random.Range(-25,25));
            }else{
                switch (gameObject.name)
                {
                    case "Inky":
                    return new Vector3(-1,0,-1);
                    case "Pinky":
                    return new Vector3(-1,0,1);
                    case "Blinky":
                    return new Vector3(1,0,-1);
                    case "Clyde":
                    return new Vector3(1,0,1);        
                    default:
                    return new Vector3(0,0,0);        
                }
            }
        }
    }

    Vector3 PresaLejana(){
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0){
            float distance = 0f;
            foreach (GameObject pj in players)
            {
                float dAux = Vector3.Distance(pj.transform.position,transform.position);
                if (dAux>distance)
                {
                    distance = dAux;
                    player = pj;
                }
            }
            return new Vector3(player.transform.position.x, 0, player.transform.position.z);
        }
    }

    Vector3 DestinoHuida(){
        Vector3 pos =  Vector3.zero;
        
        switch (gameObject.name){
                    case "Inky":
                    return new Vector3(-1,0,-1);
                    case "Pinky":
                    return new Vector3(-1,0,1);
                    case "Blinky":
                    return new Vector3(1,0,-1);
                    case "Clyde":
                    return new Vector3(1,0,1);        
                    default:
                    return new Vector3(0,0,0);        
        }
    }
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        gameObject.GetComponent<Light>().enabled = false;
        gameObject.GetComponent<Light>().enabled = true;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

    }

    // Update is called once per frame
    void Update()
    {
        if(agent.isOnOffMeshLink){
            gameObject.layer = 3;
        }else{
            gameObject.layer = 0;
        }
    }
    private void FixedUpdate() {
        
    }
    public Vector3 GhostMove(){
        return new Vector3(0, 0, 0);
    }
    public void Move(Vector3 position){
        Position.Value = position;  
        if (agent.isOnNavMesh)
        {
            agent.destination = Position.Value;
        }
    }
    // void Turn(Vector3 rotation){
    //     transform.LookAt(Rotation.Value);
    //     RotationDriveMode.Value = rotation;
    // }

    IEnumerator EatableEnergy(){
        yield return new WaitForSeconds(2f);
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Position.Value = gameObject.GetComponent<NavMeshAgent>().destination;
        }
    }


    [ServerRpc]
    void StartCoroutineResquestServerRpc(){
        StartCoroutine(EatableEnergy());
    }
}
