using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
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
    private int rollChargeNum;

    // Start is called before the first frame update
    void Start()
    {
        pScript = player.GetComponent<PlayerScript>();
        invNum = pScript.invSize;
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

    void UpdateUI()
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
        }
    }
}
