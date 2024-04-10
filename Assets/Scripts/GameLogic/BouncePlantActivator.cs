using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePlantActivator : PlantActivator
{
    public Transform bouncePlant;

    protected override void HandleTimePeriodChanged()
    {
        if (timeSwapManager.currentTimePeriod == TimeSwapManager.TimePeriod.Present && arePlantsGrown)
        {
            GrowPlants();
            Debug.Log("Bounce plants are grown");
        }
    }

    protected override void GrowPlants()
    {
        bouncePlant.gameObject.SetActive(true);
    }
}
