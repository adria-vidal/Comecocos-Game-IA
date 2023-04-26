using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class pacman : NetworkBehaviour
{
    public Vector3 input;
    public float speed = 10f;
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    public CharacterController characterController;
    public NetworkVariable<bool> inmortal = new NetworkVariable<bool>();
    public NetworkVariable<int> life = new NetworkVariable<int>();



    public enum States
    {
        Pacman,
        SuperPacman
    }
    public States state;

    public Score score;

    // Start is called before the first frame update
    void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            // score.Value = 0;

        }
        characterController = GetComponent<CharacterController>();
        life.Value = 3;

    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            switch (state)
            {
                case States.Pacman:
                    normalPacman();
                    break;
                case States.SuperPacman:
                    SuperPacman();
                    break;
            }
            if (IsServer)
            {
                if (input != Position.Value)
                {
                    Move(input);
                }
            }
        }
        if (state == States.Pacman)
        {

        }
        // gameObject.GetComponent<Light>().enabled = SuperPacman;
        // GameObject.Find("GameManager").GetComponent<Score>().AddScore(5);
    }

    // void ResolveCollision(GameObject hit){
    //     if (IsServer && hit){
    //         Score(hit);
    //     }else{
    //         SubmitCollisionResquestServerRpc(hit.GetComponent<NetworkObject>());
    //     }
    // }


    public void Move(Vector3 direction)
    {
        characterController.Move(direction * speed * Time.deltaTime);
    }
    public void OnTriggerEnter(Collider other)
    {
        GameObject hit = other.gameObject;
        if (IsOwner)
        {
            // ResolveCollision(hit);
        }

        if (other.gameObject.CompareTag("coco"))
        {
            score.AddScore(1);
            Debug.Log(score.score);
            Destroy(other.gameObject);
        }

        /* This code block is checking if the Pacman collides with a game object that has a tag of
        "powerup". If it does, it will loop through all the ghosts in the game and set their
        rigidbody to be kinematic and their box collider to be a trigger. It will also add 3 points
        to the Pacman's score, set the Pacman's state to "SuperPacman", start a coroutine called
        "PowerUp" that will last for 15 seconds, log the Pacman's score to the console, and destroy
        the "powerup" game object. */
        if (other.gameObject.CompareTag("powerup"))
        {
            foreach (var ghost in GameManager1.instance.Ghosts)
            {
                ghost.GetComponent<Rigidbody>().isKinematic = true;
                ghost.GetComponent<BoxCollider>().isTrigger = true;
            }
            score.AddScore(3);
            ChangeStateServerRpc(States.SuperPacman);
            StartCoroutine(PowerUp(15f));
            Debug.Log(score.score);
            Destroy(other.gameObject);

        }


        /* This code block is checking if the Pacman collides with a game object that has a tag of
        "ghost" while in the "SuperPacman" state. If it does, it will add 100 points to the Pacman's
        score and set the "IsGhostEated" variable of the collided ghost's "AIGhost" component to
        true. This likely means that the ghost has been eaten by the Pacman while in the
        "SuperPacman" state. */
        if (state == States.SuperPacman && other.gameObject.CompareTag("ghost"))
        {
            other.gameObject.GetComponent<AIGhost>().IsGhostEated = true;
            score.AddScore(100);
            other.gameObject.GetComponent<AIGhost>().StartCoroutineEnableMeshServerRpc();
        }

    }
    public void OnCollisionEnter(Collision other)
    {
        GameObject hit = other.gameObject;
        if (IsOwner)
        {
            // ResolveCollision(hit);
        }
    }

    IEnumerator PowerUp(float t)
    {
        yield return new WaitForSeconds(t);
        ChangeStateServerRpc(States.Pacman);
    }
    //Cambia color yte hace inmortal cuando eres SuperPacman
    void SuperPacman()
    {
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("ghost");
        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<Renderer>().material = ghost.GetComponent<AIGhost>().eatableMaterial;
        }
        inmortal.Value = true;
    }
    //Cambia color yte hace mortal cuando eres SuperPacman
    void normalPacman()
    {
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("ghost");
        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<Renderer>().material = ghost.GetComponent<AIGhost>().normalMaterial;
        }
        inmortal.Value = false;
    }



    [ServerRpc]
    void ChangeStateServerRpc(States nextState)
    {
        state = nextState;
    }

    IEnumerator InmortalTime(float t)
    {
        yield return new WaitForSeconds(t);
        ChangeStateServerRpc(States.Pacman);
    }

    //Cambiamos estado y le ponemos tiempo en ese estado
    [ServerRpc]
    void InmortalServerRpc()
    {
        ChangeStateServerRpc(States.SuperPacman);
        StartCoroutine(InmortalTime(8f));
    }
    //Funcion perder una vida en red
    [ServerRpc]
    public void LifeLossServerRpc()
    {
        if (!inmortal.Value)
        {
            life.Value--;
            InmortalServerRpc();
            if (life.Value <= 0)
            {
                Destroy(gameObject);
            }
        }

    }



}


