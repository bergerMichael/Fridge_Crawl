using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenFridge : MonoBehaviour
{
    public GameObject foodPrefab;
    public AudioSource fridge_open;
    public AudioSource fridge_close;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private GameObject InstantiateFoodPrefab()
    {
        GameObject food = Instantiate(foodPrefab);
        food.GetComponent<Transform>().position = transform.position;
        return food;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GetComponent<Animator>().SetBool("Opened", true);
            spawnFood();
            fridge_open.Play();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GetComponent<Animator>().SetBool("Opened", false);
            fridge_close.Play();
        }
    }

    private void spawnFood()
    {
        Vector2 randDes = new Vector2();
        randDes.x = 2f;
        randDes.y = 0f;
        float rSpeed = Random.Range(8f, 16f);

        InstantiateFoodPrefab().GetComponent<FoodBehavior>().TakeMovementParams(randDes, rSpeed);
    }
}
