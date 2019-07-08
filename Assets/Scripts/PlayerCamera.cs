using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject player;
    private PlayerScript pScript;

    public Transform playerPos;
    public float xMax;
    public float xMin;
    public float yMax;
    public float yMin;
    public Sprite invSlotSprite;
    public Sprite rollSlotSprite;

    private int invNum;
    private bool[] invSlots;
    private int rollChargeNum;

    // Start is called before the first frame update
    void Start()
    {
        pScript = player.GetComponent<PlayerScript>();
        invNum = pScript.invSize;
        invSlots = new bool[invNum];
        for (int i = 0; i < invSlots.Length; i++)   // initialize inventory slots
        {
            invSlots[i] = false;        // false indicates that the inv slot is free
        }
        rollChargeNum = pScript.rollCharges;
        DrawUI();
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
        if (rollChargeNum != pScript.rollCharges)
        {
            rollChargeNum = pScript.rollCharges;
            UpdateUI();
        }
    }

    void MoveCamera()
    {
        float xTransformation;
        float yTransformation;

        if (playerPos.position.x > xMin && playerPos.position.x < xMax)
            xTransformation = playerPos.position.x;
        else
            xTransformation = transform.position.x;

        if (playerPos.position.y > yMin && playerPos.position.y < yMax)
            yTransformation = playerPos.position.y;
        else
            yTransformation = transform.position.y;

        Vector3 moveTo = new Vector3(xTransformation, yTransformation, transform.position.z);
        transform.position = moveTo;
    }

    void DrawUI()
    {
        // Draw inventory
        for (int i = 0; i < invNum; i++)
        {
            // Create the inventory slot and add it to the camera heirarchy
            GameObject newInvSlot = new GameObject();
            newInvSlot.tag = "Inventory";
            newInvSlot.transform.parent = transform;
            Vector3 pos = transform.position;

            // Offset from camera:
            pos.x -= 7.0f;
            pos.y += 3.0f;
            pos.z = transform.position.z + 10;

            // offset from existing slots:
            pos.x += (0.595f * i);
            newInvSlot.transform.position = pos;
            newInvSlot.AddComponent(typeof(SpriteRenderer));
            newInvSlot.GetComponent<SpriteRenderer>().sprite = invSlotSprite;
            newInvSlot.GetComponent<SpriteRenderer>().sortingLayerName = "UI_Layer";
        }

        // draw roll charges
        for (int i = 0; i < rollChargeNum; i++)
        {
            // Create the inventory slot and add it to the camera heirarchy
            GameObject newRollSlot = new GameObject();
            newRollSlot.tag = "Roll";
            newRollSlot.transform.parent = transform;
            Vector3 pos = transform.position;

            // Offset from camera:
            pos.x -= 7.0f;
            pos.y += 2.4f;
            pos.z = transform.position.z + 10;

            // offset from existing slots:
            pos.x += (0.439f * i);
            newRollSlot.transform.position = pos;
            newRollSlot.AddComponent(typeof(SpriteRenderer));
            newRollSlot.GetComponent<SpriteRenderer>().sprite = rollSlotSprite;
            newRollSlot.GetComponent<SpriteRenderer>().sortingLayerName = "UI_Layer";
        }
    }

    void UpdateUI()     // This is used to update roll charges. It will not affect inventory slots
    {
        int cameraChildCount = transform.childCount;
        foreach (Transform child in transform)
        {
            if (child.tag == "Roll")
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        // draw roll charges
        for (int i = 0; i < rollChargeNum; i++)
        {
            // Create the inventory slot and add it to the camera heirarchy
            GameObject newRollSlot = new GameObject();
            newRollSlot.tag = "Roll";
            newRollSlot.transform.parent = transform;
            Vector3 pos = transform.position;

            // Offset from camera:
            pos.x -= 7.0f;
            pos.y += 2.4f;
            pos.z = transform.position.z + 10;

            // offset from existing slots:
            pos.x += (0.439f * i);
            newRollSlot.transform.position = pos;
            newRollSlot.AddComponent(typeof(SpriteRenderer));
            newRollSlot.GetComponent<SpriteRenderer>().sprite = rollSlotSprite;
            newRollSlot.GetComponent<SpriteRenderer>().sortingLayerName = "UI_Layer";
        }
    }

    public void AddFood(GameObject food)
    {
        // Find the next free inv slot
        int freeSlot = 0;
        foreach (bool slot in invSlots)     // find the next free inv slot
        {
            if (slot)
            {
                freeSlot++;
            }
            else
            {
                invSlots[freeSlot] = true;      // the inv slot is no longer free
                break;
            }
        }

        // the freeSlot int will be used to calculate the proper transform to assign the food object to


        food.transform.parent = transform;
        Vector3 pos = transform.position;

        // Offset from camera:
        pos.x -= 7.0f;
        pos.y += 3.0f;
        pos.z = transform.position.z + 5;  // food needs to appear on top of the inv slot

        // offset from existing slots:
        pos.x += (0.595f * freeSlot);
        food.transform.position = pos;
        food.GetComponent<SpriteRenderer>().sortingLayerName = "UI_Layer";
        Destroy(food.GetComponent<Rigidbody2D>());
        Destroy(food.GetComponent<BoxCollider2D>());
    }

    public GameObject RemoveFood()
    {
        // Find the last occupied inventory slot
        int lastFilledSlot = 0;
        foreach (bool slot in invSlots)     // find the last filled inv slot
        {
            if (invSlots[invSlots.Length - 1])     // This is the case where all inv slots are full
            {
                lastFilledSlot = invSlots.Length - 1;
                break;
            }
            if (!slot)      // This means a slot is free
            {
                lastFilledSlot--;
                break;
            }
            else
            {
                lastFilledSlot++;   // get ready to check the next slot
            }
        }

        // Using last filled slot, find and destroy the food that is occupying that slot
        foreach (Transform child in transform)
        {
            if (child.tag == "Food")        // find the gameObject with the position corresponding to the last filled inv slot
            {
                Vector3 pos = transform.position;

                pos.x -= 7.0f;
                pos.y += 3.0f;
                pos.z = transform.position.z + 5;
                pos.x += (0.595f * lastFilledSlot);

                if (child.transform.position == pos)
                {
                    child.parent = null;    // detach from player camera UI
                    invSlots[lastFilledSlot] = false;
                    return child.gameObject;
                }                
            }
        }

        return null;
    }
}
