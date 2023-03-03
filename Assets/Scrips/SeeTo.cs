using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeTo : MonoBehaviour
{
    public Transform target;

    void Start(){
       
    }
    // Update is called once per frame
    void Update()
    {
        enabled = false;
        if (target){
            transform.LookAt(target);
        }
        enabled = true;
    }
    void FixedUpdate() {
        if (target)
        {
            transform.LookAt(target);
        }
    }
        
}
