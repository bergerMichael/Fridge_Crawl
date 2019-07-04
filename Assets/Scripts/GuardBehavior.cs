using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBehavior : MonoBehaviour
{
    public float speed;
    private float waitTime;
    public float startWaitTime;
    public bool isChaseActive;

    public Transform[] moveSpots;
    public Transform lastKnownPos;
    public Transform PlayerPos;
    private int nextSpot; 

    // Start is called before the first frame update
    void Start()
    {
        waitTime = startWaitTime;
        nextSpot = 0;   // assuming object starts at position index 0
        isChaseActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isChaseActive)  // if the guard is not chasing the player, movement continues as normal
        {
            Move();
        }
        else
        {
            // otherwise the chase behaviour is used
            Chase();
        }

        RotateFOV();    // This must be done on each update
        RaycastToPlayer();

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

    void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, moveSpots[nextSpot].position, speed * Time.deltaTime);

        float distance = Vector2.Distance(transform.position, moveSpots[nextSpot].position);

        if (distance < 0.2f)
        {
            if (nextSpot != moveSpots.Length - 1)
            {
                nextSpot++;
            }
            else
                nextSpot = 0;
        }
    }

    void RaycastToPlayer()
    {
        Debug.DrawLine(transform.position, PlayerPos.position, Color.green);
    }

    void Chase()
    {
        // Move toward the last known position
        transform.position = Vector2.MoveTowards(transform.position, lastKnownPos.position, speed * Time.deltaTime);
        float distance = Vector2.Distance(transform.position, lastKnownPos.position);
        if (distance < 0.2f)
        {
            // This is the case where the guard reaches the last known position
        }

        // use a raycast to update last known position
        RaycastToPlayer();
    }

    public void OnDetection(Transform detectedPos)
    {
        isChaseActive = true;
        lastKnownPos = detectedPos;
    }

}
