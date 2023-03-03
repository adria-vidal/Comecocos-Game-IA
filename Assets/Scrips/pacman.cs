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

public Score score;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        input =  new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
    if(IsServer){
        if(input != Position.Value){
            Move(input);

        }
    }
    }
    public void Move(Vector3 direction){
    characterController.Move(direction * speed*Time.deltaTime);

        

    }
    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("coco"))
        {
            score.AddScore(1);
            Debug.Log(score.score);
            Destroy(other.gameObject);
        }
    }
}
