using UnityEngine;

public class ItemSensor : MonoBehaviour
{
    public BoxCollider2D itemCheck;
    public LayerMask itemMask;
    public GameObject item;

    public bool itemDetected { get; private set; }

    void FixedUpdate()
    {
        CheckItem();
    }

    void CheckItem()
    {
        itemDetected = Physics2D.OverlapAreaAll(itemCheck.bounds.min, itemCheck.bounds.max, itemMask).Length > 0;

        if (itemDetected)
        {
            Collider2D[] items = Physics2D.OverlapAreaAll(itemCheck.bounds.min, itemCheck.bounds.max, itemMask);

            foreach (Collider2D i in items)
            {
                if (i.CompareTag("Seed") || i.CompareTag("Container"))
                {
                    Debug.Log("Item detected: " + i.gameObject.name);
                    item = i.gameObject;
                    break;
                }
            }

        }
        else
        {
            item = null;
        }
    }
}