using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    public float speed;
    private float waitTime;
    public float startWaitTime;

    public Transform[] moveSpots;
    private int nextSpot; 

    // Start is called before the first frame update
    void Start()
    {
        waitTime = startWaitTime;
        nextSpot = 0;   // assuming object starts at position index 0
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, moveSpots[nextSpot].position, speed * Time.deltaTime);

        float distance = Vector2.Distance(transform.position, moveSpots[nextSpot].position);

        RotateFOV();

        if (distance < 0.2f)
        {
                if (nextSpot != moveSpots.Length - 1)
                {
                    nextSpot++;
                }
                else
                    nextSpot = 0;         
        }
        
        Raycasting();

    }

    void RotateFOV()
    {
        Vector2 direction = moveSpots[nextSpot].position;
        float angle = Mathf.Sin((transform.position.y - direction.y) / (transform.position.x - direction.x)) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Transform[] toRotate = this.GetComponentsInChildren<Transform>();

        for (int i = 0; i < toRotate.Length; i++)
        {
            if (toRotate[i].name == "FOV")
            {
                // toRotate[i].rotation = Quaternion.Slerp(toRotate[i].rotation, rotation, speed * Time.deltaTime);
                var newRotation = Quaternion.LookRotation((toRotate[i].position - moveSpots[nextSpot].position), Vector3.forward);
                newRotation.x = 0.0f;
                newRotation.y = 0.0f;
                toRotate[i].rotation = Quaternion.Slerp(toRotate[i].rotation, newRotation, Time.deltaTime * 8);
            }
        }            
    }

    void Raycasting()
    {
        Debug.DrawLine(transform.position, moveSpots[nextSpot].position, Color.red);
    }

    void OnDetect()
    {

    }

}
