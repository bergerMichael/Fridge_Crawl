using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawnBehavior : MonoBehaviour
{
    System.Random randGen;
    Vector2 spawnDir;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        randGen = new System.Random();
        spawnDir = new Vector2(transform.position.x * (randGen.Next(-5, 5)), transform.position.y * (randGen.Next(-5, 5)));
        speed = (float)randGen.Next(5,10);
    }

    // Update is called once per frame
    void Update()
    {
        if (speed > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, spawnDir, speed * Time.deltaTime);
            speed -= (float)0.2;
        }
    }
}
