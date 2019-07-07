using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerScript : MonoBehaviour
{
    public Vector2 speed = new Vector2(50, 50);
    public float rollDist;
    private Vector3 rollTo;
    public int rollCharges;
    public int invSize;

    public int currentLoad;

    public Animator playerAnimator;

    public delegate void EventHandler(GameObject e);
    public event EventHandler OnFoodPickup;

    private bool IsMoving;
    private bool IsFacingLeft;
    private bool IsRolling;
    private bool IsRollInitiated;   // prevents infinite roll    

    private void Start()
    {
        IsRolling = false;      // This needs to be false initially because it will only be assigned false outside of update
        IsRollInitiated = false;
        currentLoad = 0;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    void Roll(float xDir, float yDir)
    {
        // The roll animation is handled by the player animation controller
        // When rolling, the player will move a set distance and then stop. This distance is a property of player
        // Take a snapshot of the input axis when the roll is executed. If these are zero generate one based off of the direction the player is facing        

        if (!IsRollInitiated)   // Take snapshot of input here. This way it will only be done once per roll
        {
            rollCharges--;  // This allows for immediate feedback to the player that a charge has been expended
            IsRollInitiated = true;
            if (xDir == 0 && yDir == 0)
            {
                if (IsFacingLeft)
                    rollTo = new Vector3(transform.position.x + (-1 * rollDist), transform.position.y, 0);
                else
                    rollTo = new Vector3(transform.position.x + (1 * rollDist), transform.position.y, 0);                
            }
            else
                rollTo = new Vector3(transform.position.x + (xDir * rollDist), transform.position.y + (yDir * rollDist), 0);        // This is where the position is generated
        }

        float distance = Vector2.Distance(transform.position, rollTo);

        if (distance > 0.2f)
        {
            transform.position = Vector2.MoveTowards(transform.position, rollTo, 7f * Time.deltaTime);
            distance = Vector2.Distance(transform.position, rollTo);
        }
        else
        {
            // Roll complete!
            IsRollInitiated = false;
            IsRolling = false;
            playerAnimator.SetBool("IsRolling", false);            
        }
    }

    void GetInput()
    {
        // first determine the three conditions: IsMoving, IsRolling, IsFacingLeft

        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        bool rollInput = Input.GetKey(KeyCode.Space);

        if (IsRollInitiated)
        {
            Roll(inputX, inputY);
            return;     // take no mre input while rolling
        }

        // Check if rolling
        if (rollInput && !IsRollInitiated)
        {
            if (rollCharges > 0)
            {
                // This is also the case for calling the Roll() function
                IsRolling = true;
                playerAnimator.SetBool("IsRolling", true);
                Roll(inputX, inputY);
                return;
            }
        }

        // Check if facing left
        if (inputX < 0)
        {
            playerAnimator.SetBool("IsFacingLeft", true);
            IsFacingLeft = true;
        }
        else if (inputX > 0)        // this is necessary because it prevents IsFacingLeft from becoming false when the player simple stops moving altogether from a negative x input
        {
            playerAnimator.SetBool("IsFacingLeft", false);
            IsFacingLeft = false;
        }

        // Check if moving
        if (inputX != 0 || inputY != 0)
        {
            IsMoving = true;
            playerAnimator.SetBool("IsMoving", true);
            Vector3 movement = new Vector3(speed.x * inputX, speed.y * inputY, 0);

            movement *= Time.deltaTime;

            transform.Translate(movement);
        }
        else
        {
            IsMoving = false;
            playerAnimator.SetBool("IsMoving", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Food")
        {
            OnFoodPickup(collision.gameObject);
        }
    }
}
