using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class AIGhost : NetworkBehaviour
{
    public MeshRenderer render;
    public bool IsGhostEated;
    public NavMeshAgent agent;
    public GameObject player;
    public Rigidbody rb;

    //////////////NETWORK VARIABLES//////////
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    public NetworkVariable<bool> eatable = new NetworkVariable<bool>();
    public NetworkVariable<int> i = new NetworkVariable<int>();
    public Material normalMaterial, eatableMaterial;


    /// If the object is a server, it will start the coroutine EatableEnergy, if not, it will start the
    /// coroutine StartCoroutineResquestServerRpc.
    public void Eatable()
    {
        if (IsServer)
        {
            StartCoroutine(EatableEnergy());
        }
        else
        {
            //Como es una IA, lo más probable es que no entre
            StartCoroutineResquestServerRpc();
        }
    }

    public void ghostEated()
    {
        render = GetComponent<MeshRenderer>();
        render.enabled = false;
        DestinoHuida();
    }

    /// If there are players, return the closest one, otherwise return a random position
    Vector3 PresaCercana()
    {
        /* Finding the closest player to the ghost. */
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            float distance = 100f;
            /* Finding the closest player to the ghost. */
            foreach (GameObject pj in players)
            {
                float dAux = Vector3.Distance(pj.transform.position, transform.position);
                if (pj.GetComponent<pacman>().inmortal.Value)
                {
                    player = null;
                }

                if (dAux < distance && !pj.GetComponent<pacman>().inmortal.Value)
                {
                    distance = dAux;
                    player = pj;
                }
            }
            if (player)
            {
                return new Vector3(player.transform.position.x, 0, player.transform.position.z);
            }
            else
            {
                return DestinoHuida();
            }

        }
        else
        {
            //Sino tenemos jugadores
            /* Checking if there is a gameobject called "Plataforma" in the scene, if there is, it will
            return a random position, if not, it will return a position depending on the name of the
            gameobject. */
            if (GameObject.Find("Plataforma"))
            {
                return new Vector3(Random.Range(-25, 25), 0, Random.Range(-25, 25));
            }
            else
            {
                switch (gameObject.name)
                {
                    case "Inky":
                        return new Vector3(-1, 0, -1);
                    case "Pinky":
                        return new Vector3(-1, 0, 1);
                    case "Blinky":
                        return new Vector3(1, 0, -1);
                    case "Clyde":
                        return new Vector3(1, 0, 1);
                    default:
                        return new Vector3(0, 0, 0);
                }
            }
        }
    }

    /// If the position of the object is equal to the position of the waypoint, then increment the value
    /// of i, and if the value of i is greater than the length of the array, then set the value of i to 0

    /// <returns>
    /// The position of the waypoint.

    Vector3 MoveToWayPoints()
    {
        GameObject[] waypoints = GameObject.FindGameObjectsWithTag("INKY Point");
        if (transform.position == waypoints[i.Value].transform.position)
        {
            i.Value++;
            if (i.Value > waypoints.Length)
            {
                i.Value = 0;
            }
        }
        return waypoints[i.Value].transform.position;
    }

    Vector3 PresaLejana()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            float distance = 0f;
            foreach (GameObject pj in players)
            {
                float dAux = Vector3.Distance(pj.transform.position, transform.position);
                if (dAux > distance)
                {
                    distance = dAux;
                    player = pj;
                }
            }
        }
        return new Vector3(player.transform.position.x, 0, player.transform.position.z);
    }

    Vector3 DestinoHuida()
    {
        Vector3 pos = Vector3.zero;

        switch (gameObject.name)
        {
            case "Inky":
                pos = MoveToWayPoints();
                return pos;
            case "Pinky":
                return new Vector3(-1, 0, 1);
            case "Blinky":
                return new Vector3(1, 0, -1);
            case "Clyde":
                return new Vector3(1, 0, 1);
            default:
                return new Vector3(0, 0, 0);
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
        /* Checking if there is a player in the scene, if there is, it will set the destination of the
        agent to the closest player, if not, it will set the destination to a random position. */
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            return;
        }
        agent.SetDestination(PresaCercana());

        //cambiar capa
        if (agent.isOnOffMeshLink)
        {
            gameObject.layer = 3;
        }
        else
        {
            gameObject.layer = 0;
        }

        if (IsServer)
        {
            if (IsGhostEated)
            {
                ghostEated();
            }
            agent.SetDestination(PresaCercana());
        }
        else
        {
            SubmitNavMeshServerRpc(PresaCercana());
        }


    }
    private void FixedUpdate()
    {

    }

    public Vector3 GhostMove()
    {
        return new Vector3(0, 0, 0);
    }
    public void Move(Vector3 position)
    {
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

    IEnumerator EatableEnergy()
    {
        yield return new WaitForSeconds(2f);
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Position.Value = gameObject.GetComponent<NavMeshAgent>().destination;
        }
    }
    IEnumerator EnableMesh()
    {
        yield return new WaitForSeconds(5f);
        render = GetComponent<MeshRenderer>();
        render.enabled = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<BoxCollider>().isTrigger = false;
        IsGhostEated = false;

    }

    [ServerRpc]
    public void StartCoroutineEnableMeshServerRpc()
    {
        StartCoroutine(EnableMesh());
        gameObject.GetComponent<BoxCollider>().isTrigger = false;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    [ServerRpc]
    void StartCoroutineResquestServerRpc()
    {
        StartCoroutine(EatableEnergy());
    }
    [ServerRpc]
    void SubmitNavMeshServerRpc(Vector3 position, ServerRpcParams rpcParams = default)
    {
        agent.SetDestination(position);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            pacman pacman = other.gameObject.GetComponent<pacman>();
            pacman.LifeLossServerRpc();
        }
    }

}
