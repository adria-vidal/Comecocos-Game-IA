using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int width = 100;
    public int height = 100;
    public float dificulty = 0.5f;
    public GameObject wall;

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x <= width; x += 2) {
            for (int y = 0; y <= height; y += 2) {
                if (Random.value < dificulty) {
                    Vector3 pos = new Vector3(x -  width / 2f, 0, y - height / 2f);
                    Quaternion rotation;
                    if (Random.value > 0.5f) {
                        rotation = new Quaternion(0f, 0f, 0, 0);
                    } else {
                        rotation = new Quaternion(0, 90f, 0, 0);
                    }
                    Instantiate(wall, pos, rotation);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
