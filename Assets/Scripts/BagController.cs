using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagController : MonoBehaviour
{
    public GameObject itemToDispense;

    public GameObject SpawnItem(GameObject location)
    {
        if (itemToDispense != null)
        {
            GameObject item = Instantiate(itemToDispense, location.transform.position, Quaternion.identity);

            return item;
        }

        return null;
    }
}
