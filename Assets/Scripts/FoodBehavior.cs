﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBehavior : MonoBehaviour
{
    private int rand;
    public bool isMoving;
    public Vector2 destination;
    public float speed;
    public bool active;


    public Sprite[] foods;

    // Start is called before the first frame update
    void Start()
    {
        rand = Random.Range(0, foods.Length);        
        this.gameObject.GetComponent<SpriteRenderer>().sprite = foods[rand];
        isMoving = false;
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
            PropellFood();
    }

    public void PropellFood()   // moves the food to a provided destination at the provided speed
    {

        this.GetComponent<Rigidbody2D>().position = Vector2.MoveTowards(this.GetComponent<Rigidbody2D>().position, destination, speed * Time.deltaTime);   // set rigidbody's position for physics

        float distance = Vector2.Distance(transform.position, destination);

        if (distance < 0.2f)
        {
            isMoving = false;
        }
    }

    public void TakeMovementParams(Vector2 des, float sp)
    {
        des.x += transform.position.x;
        des.y += transform.position.y;
        destination = des;
        speed = sp;
        isMoving = true;        
        this.gameObject.AddComponent<Collider2D>();        
        active = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)     // May only need this for the guard. If the food is collided with by the player, it is sent to the UI
    {
        if (collision.tag == "Player" || collision.tag == "Guard")
        {
            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            if (GetComponent<Rigidbody2D>() == null)
            {
                this.GetComponent<SpriteRenderer>().sortingLayerName = "Threshold_Layer";
                this.gameObject.AddComponent<Rigidbody2D>();                
                this.GetComponent<Rigidbody2D>().gravityScale = 0;
                active = true;
            }
        }
    }
}
