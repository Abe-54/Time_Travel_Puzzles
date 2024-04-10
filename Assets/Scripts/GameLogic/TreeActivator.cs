using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeActivator : PlantActivator
{
    public Transform Tree;

    protected override void GrowPlants()
    {
        Tree.gameObject.SetActive(true);
    }

    protected override void HandleTimePeriodChanged()
    {
        if (timeSwapManager.currentTimePeriod == TimeSwapManager.TimePeriod.Present && arePlantsGrown)
        {
            GrowPlants();
            Debug.Log("Tree is grown");
        }
    }
}
