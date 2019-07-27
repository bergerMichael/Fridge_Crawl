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

    public PlayerCamera PlayerUI;

    private bool IsMoving;
    private bool IsFacingLeft;
    private bool IsRolling;
    private bool IsRollInitiated;   // prevents infinite roll    
    private bool IsStunned;
    private bool IsWallCollisionActive;

    private void Start()
    {
        IsRolling = false;      // This needs to be false initially because it will only be assigned false outside of update
        IsRollInitiated = false;
        currentLoad = 0;
        IsStunned = false;
        IsWallCollisionActive = false;
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

        if (IsWallCollisionActive)  // If we hit a wall end the roll
        {
            rollTo = transform.position;
        }

        Debug.DrawLine(transform.position, rollTo, Color.yellow);   // used for testing purposes

        // check if there is a collision that is preventing the player from reaching rollTo. If so, cancel the roll

        float distance = Vector2.Distance(transform.position, rollTo);

        if (distance > 0.2f)
        {
            transform.position = Vector2.MoveTowards(GetComponent<Rigidbody2D>().position, rollTo, 7f * Time.deltaTime);
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
        bool rollInput = Input.GetKeyUp(KeyCode.Space);

        if (IsRollInitiated)
        {
            Roll(inputX, inputY);
            return;     // take no mre input while rolling
        }

        // Check if rolling
        if (rollInput)
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

        // Check if eating food
        if (Input.GetKeyUp(KeyCode.Q))
        {
            EatFood();
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
        if (collision.transform.tag == "Food" && !IsStunned)      // if the player collides with a food object
        {
            if (collision.gameObject.GetComponent<FoodBehavior>().active)       // make sure the food didn't just spawn
            {
                // Check if there's room in the player inventory
                if (currentLoad < invSize)
                {
                    PlayerCamera pcScript = PlayerUI.GetComponent<PlayerCamera>();
                    pcScript.AddFood(collision.gameObject);
                    currentLoad++;
                }
            }
        }

        else if (collision.transform.tag == "Guard")    // if the player collides with a guard
        {
            //IsStunned = true;
            // Launch food in a directions
            PlayerCamera pcScript = PlayerUI.GetComponent<PlayerCamera>();
            for (int i = currentLoad; i > 0; i--)
            {
                // generate a random vector and speed to launch the food 
                Vector2 randDir = new Vector2();
                randDir.x = Random.Range(-2.0f, 2f);
                randDir.y = Random.Range(-2.0f, 2f);
                float rSpeed = Random.Range(8f, 16f);

                LaunchFood(pcScript.RemoveFood(), randDir, rSpeed);
                currentLoad--;
            }
            // Stun the player momentarily                        
        }

        if (collision.transform.tag == "Wall")  // if the player is colliding with a wall, we need to flag it to prevent a roll from getting caught
        {
            IsWallCollisionActive = true;
        }

        if (collision.transform.tag == "Chest")
        {
            // transfer food to chest
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Wall")
        {
            IsWallCollisionActive = false;
        }
    }

    private void EatFood()
    {
        if (currentLoad != 0)
        {
            PlayerCamera pcScript = PlayerUI.GetComponent<PlayerCamera>();
            Destroy(pcScript.RemoveFood());
            currentLoad--;
            rollCharges++;
        }
    }

    private void LaunchFood(GameObject food, Vector2 direction, float sp)
    {
        food.transform.position = transform.position;       // food must originate from the player
        FoodBehavior fScript = food.GetComponent<FoodBehavior>();
        fScript.TakeMovementParams(direction, sp);
    }
}
