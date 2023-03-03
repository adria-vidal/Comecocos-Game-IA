using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CamManager : NetworkBehaviour
{
    public GameObject cameraGO;
    CamFollow camFollow;
    SeeTo seeTo;
    // Start is called before the first frame update
    void Start()
    {
        cameraGO = GameObject.Find("MAin Camera");
        camFollow = cameraGO.GetComponent<CamFollow>();
        seeTo = cameraGO.GetComponent<SeeTo>();

        if (IsOwner)
        {
            camFollow.ball = gameObject;
            seeTo.target = gameObject.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
