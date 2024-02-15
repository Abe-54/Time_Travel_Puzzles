using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image inventoryImage;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateInventory(GameObject item)
    {
        Debug.Log("Picked up " + item.name);
        inventoryImage.sprite = item.GetComponent<SpriteRenderer>().sprite;
        inventoryImage.color = item.GetComponent<SpriteRenderer>().color;
        inventoryImage.rectTransform.localScale = new Vector3(item.transform.localScale.x, item.transform.localScale.y, item.transform.localScale.z);
    }
}
