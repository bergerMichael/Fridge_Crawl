using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    public AudioSource Chest_close;
    public AudioSource Chest_open;
    private bool Is_closed;

    // Start is called before the first frame update
    void Start()
    {
        Is_closed = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
            OperateChest();
    }

    void OperateChest()
    {
        if (Is_closed)
        {
            Is_closed = false;
            GetComponent<Animator>().SetBool("Chest_Close", false);
            Chest_open.Play();           
        }
        else
        {
            Is_closed = true;
            GetComponent<Animator>().SetBool("Chest_Close", true);
            Chest_close.Play();
        }
    }
}
