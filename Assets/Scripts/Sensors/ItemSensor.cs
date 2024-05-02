using UnityEngine;

public class ItemSensor : MonoBehaviour
{
    public BoxCollider2D itemCheck;
    public LayerMask itemMask;
    public GameObject item;

    public bool itemDetected { get; private set; }

    private UIManager uiManager;

    private void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    void FixedUpdate()
    {
        CheckItem();
    }

    void CheckItem()
    {
        itemDetected = Physics2D.OverlapAreaAll(itemCheck.bounds.min, itemCheck.bounds.max, itemMask).Length > 0;

        if (itemDetected)
        {
            uiManager.ShowPickUpPrompt();

            Collider2D[] items = Physics2D.OverlapAreaAll(itemCheck.bounds.min, itemCheck.bounds.max, itemMask);

            foreach (Collider2D i in items)
            {
                if (i.CompareTag("Container") || i.CompareTag("Pick-Up"))
                {
                    Debug.Log("Item detected: " + i.gameObject.name);
                    item = i.gameObject;
                    break;
                }
            }

        }
        else
        {
            uiManager.HidePickUpPrompt();
            item = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(itemCheck.bounds.center, itemCheck.bounds.size);
    }
}