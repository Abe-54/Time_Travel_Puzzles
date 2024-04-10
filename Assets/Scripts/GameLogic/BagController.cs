using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagController : MonoBehaviour, IInteractable
{
    public GameObject itemToDispense;
    private GameObject spawnedItem = null;

    public void Interact(PlayerMovement player)
    {
        if (itemToDispense != null)
        {
            spawnedItem = Instantiate(itemToDispense, player.itemHoldPosition.position, Quaternion.identity);
            player.PickupItem(spawnedItem);
        }
    }
}
