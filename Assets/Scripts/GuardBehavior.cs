using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using Pathfinding;

public class GuardBehavior : MonoBehaviour
{
    public float speed;
    private float waitTime;
    public float startWaitTime;
    public bool isChaseActive;

    public Transform[] moveSpots;
    public Transform lastKnownPos;
    public Transform PlayerPos;
    public Transform Target;
    public Pathfinding.AIPath AStarScript;
    public Pathfinding.Seeker mySeeker;
    private int nextSpot;
    private bool foodDetected;
    private Vector2 foodPos;

    public Animator detectedAnimator;
    public AIDestinationSetter myDestSetter;

    // Start is called before the first frame update
    void Start()
    {
        waitTime = startWaitTime;
        nextSpot = 0;   // assuming object starts at position index 0
        isChaseActive = false;
        //detectedAnimator = GetComponentInChildren<Animator>();   // This ensures that the get call only happens once. Otherwise, it would be called every time the animation controller is updated
        foodDetected = false;
        //Target = moveSpots[0];
        mySeeker.StartPath(transform.position, Target.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isChaseActive)  // if the guard is not chasing the player, movement continues as normal
        {
            if (foodDetected)
                MoveToFood();
            else
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
        Transform direction;
        if (isChaseActive)
        {
            direction = PlayerPos;
        }
        else
        {
            direction = moveSpots[nextSpot];            
        }
        float angle = Mathf.Sin((transform.position.y - direction.position.y) / (transform.position.x - direction.position.x)) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Transform[] toRotate = this.GetComponentsInChildren<Transform>();

        for (int i = 0; i < toRotate.Length; i++)
        {
            if (toRotate[i].name == "FOV")
            {
                // toRotate[i].rotation = Quaternion.Slerp(toRotate[i].rotation, rotation, speed * Time.deltaTime);
                var newRotation = Quaternion.LookRotation((toRotate[i].position - direction.position), Vector3.forward);
                newRotation.x = 0.0f;
                newRotation.y = 0.0f;
                toRotate[i].rotation = Quaternion.Slerp(toRotate[i].rotation, newRotation, Time.deltaTime * 8);
            }
        }            
    }

    void Move()
    {        
        if (transform.position.x < moveSpots[nextSpot].position.x)
            transform.GetComponent<Animator>().SetBool("IsFacingLeft", false);
        else
            transform.GetComponent<Animator>().SetBool("IsFacingLeft", true);
        //transform.position = Vector2.MoveTowards(transform.position, moveSpots[nextSpot].position, speed * Time.deltaTime);
        Target.position = moveSpots[nextSpot].position;
        myDestSetter.target = Target;

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

    void MoveToFood()
    {
        if (transform.position.x < moveSpots[nextSpot].position.x)
            transform.GetComponent<Animator>().SetBool("IsFacingLeft", false);
        else
            transform.GetComponent<Animator>().SetBool("IsFacingLeft", true);
        // transform.position = Vector2.MoveTowards(transform.position, foodPos, speed * Time.deltaTime);
        Target.position = foodPos;        
        myDestSetter.target = Target;
    }

    void RaycastToPlayer()
    {
        Debug.DrawLine(transform.position, PlayerPos.position, Color.green);
        RaycastHit2D raycastHitObject = Physics2D.Raycast(transform.position, PlayerPos.position);
        if (raycastHitObject.collider.tag == "Wall")
        {
            lastKnownPos.position = raycastHitObject.transform.position;
        }

    }

    void Chase()
    {
        if (transform.position.x < lastKnownPos.position.x)
            transform.GetComponent<Animator>().SetBool("IsFacingLeft", false);
        else
            transform.GetComponent<Animator>().SetBool("IsFacingLeft", true);
        // Move toward the last known position
        //transform.position = Vector2.MoveTowards(transform.position, lastKnownPos.position, speed * Time.deltaTime);
        Target.position = lastKnownPos.position;
        myDestSetter.target = Target;

        float distance = Vector2.Distance(transform.position, lastKnownPos.position);
        if (distance < 0.2f)
        {
            
        }

        // use a raycast to update last known position
        RaycastToPlayer();
    }

    public void OnDetection(Transform detectedPos)
    {
        isChaseActive = true;
        lastKnownPos = detectedPos;
        UpdateDetectionAnimControllerParameter();
    }

    private void UpdateDetectionAnimControllerParameter() // This function modifies parameters in the attached detected animation controller - "DetectionAnimController"
    {
        detectedAnimator.SetBool("IsDetected", isChaseActive);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Food")
        {
            foodDetected = true;
            foodPos = collision.transform.position;
        }
        else
            foodDetected = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Food")
        {
            Destroy(collision.gameObject);
        }
    }
}
