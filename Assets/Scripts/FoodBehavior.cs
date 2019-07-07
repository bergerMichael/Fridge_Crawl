using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBehavior : MonoBehaviour
{
    private int rand;

    public Sprite[] foods;

    // Start is called before the first frame update
    void Start()
    {
        rand = Random.Range(0, foods.Length);        
        this.gameObject.GetComponent<SpriteRenderer>().sprite = foods[rand];
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        { }
        else if (collision.tag == "Guard")
        { }
    }
}
