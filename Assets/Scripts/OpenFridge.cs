using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenFridge : MonoBehaviour
{
    public GameObject foodPrefab;
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
        return Instantiate(foodPrefab, transform);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GetComponent<Animator>().SetBool("Opened", true);
            spawnFood();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GetComponent<Animator>().SetBool("Opened", false);
        }
    }

    private void spawnFood()
    {
        Vector2 randDes = new Vector2();
        randDes.x = Random.Range(-2.0f, 2f);
        randDes.y = Random.Range(-2.0f, 2f);
        float rSpeed = Random.Range(8f, 16f);

        InstantiateFoodPrefab().GetComponent<FoodBehavior>().TakeMovementParams(randDes, rSpeed);
    }
}
