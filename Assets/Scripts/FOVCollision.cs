using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVCollision : MonoBehaviour
{
    public GuardBehavior ParentGuardScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.position;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            // Make the guard chase the player
            Transform playerPos = col.gameObject.transform;  
            ParentGuardScript.OnDetection(playerPos);
        }
    }
}
