using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public GameObject ball;
    public  Vector3 offset;
    public float lerpRate;
    public bool gameOver;
    public float x,y,z;

    void Start(){
        offset = new Vector3(x,y,z);
        gameOver = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (ball)
        {
            Follow();
        }
    }
    void Follow(){
        Vector3 pos = transform.position;
        Vector3 targetPos = ball.transform.position;
        offset = new Vector3(x,y,z);
        targetPos = targetPos - offset;
        pos = Vector3.Lerp(pos, targetPos,lerpRate);
        transform.position = pos;
    }
}
