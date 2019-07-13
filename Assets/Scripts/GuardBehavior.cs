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
    public Vector2 lastKnownPos;
    public Transform PlayerPos;
    private Transform Target;
    public Pathfinding.AIPath AStarScript;
    public Pathfinding.Seeker mySeeker;
    private int nextSpot;
    private bool foodDetected;
    private bool hasLineOfSight;
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
        Target = moveSpots[1];
        mySeeker.StartPath(transform.position, Target.position);
        hasLineOfSight = false;
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
            if (foodDetected)
                MoveToFood();
            else
                Chase();
        }

        RotateFOV();    // This must be done on each update

    }

    void RotateFOV()
    {
        Vector3 direction;
        if (isChaseActive)
        {
            direction = lastKnownPos;
        }
        else
        {
            direction = moveSpots[nextSpot].position;            
        }
        float angle = Mathf.Sin((transform.position.y - direction.y) / (transform.position.x - direction.x)) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Transform[] toRotate = this.GetComponentsInChildren<Transform>();

        for (int i = 0; i < toRotate.Length; i++)
        {
            if (toRotate[i].name == "FOV")
            {
                // toRotate[i].rotation = Quaternion.Slerp(toRotate[i].rotation, rotation, speed * Time.deltaTime);
                var newRotation = Quaternion.LookRotation((toRotate[i].position - direction), Vector3.forward);
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
        float distanceToPlayer = Vector2.Distance(transform.position, PlayerPos.position);
        Vector2 directionToPlayer = (PlayerPos.position - transform.position).normalized;

        RaycastHit2D[] raycastHitObjects = Physics2D.RaycastAll(transform.position, directionToPlayer, distanceToPlayer);      // Try calculating the player direction instead of destination normalize

        foreach (RaycastHit2D hit in raycastHitObjects)
        {
            if (hit.collider.tag == "Wall")     // the case where the guard loses line of sight
            {
                hasLineOfSight = false;
                return;
            }                   // As long as this is the first case, the last known position will only update when there is no wall between the guard and the player         
        }
        lastKnownPos = PlayerPos.position;     // If the guard can see the player, save the player's position as the last known
        hasLineOfSight = true;
        return;

    }

    void Chase()
    {
        if (hasLineOfSight)
            RaycastToPlayer();      // use a raycast to update last known position
        if (transform.position.x < lastKnownPos.x)
            transform.GetComponent<Animator>().SetBool("IsFacingLeft", false);
        else
            transform.GetComponent<Animator>().SetBool("IsFacingLeft", true);

        float distance = Vector2.Distance(transform.position, lastKnownPos);
        if (distance < 0.2f)    // if the guard reaches the last known position and raycastToPlayer returns false, the player has lost the guard
        {
            if (!hasLineOfSight)
            {
                isChaseActive = false;
                detectedAnimator.SetBool("IsDetected", isChaseActive);
            }                
        }
        // Move toward the last known position        
        Target.position = lastKnownPos;
        myDestSetter.target = Target;
    }

    public void OnDetection(Transform detectedPos)
    {
        if (isChaseActive)
            return;         // don't need to do anything if the player is already being chased
        isChaseActive = true;
        lastKnownPos = detectedPos.position;
        hasLineOfSight = true;
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
