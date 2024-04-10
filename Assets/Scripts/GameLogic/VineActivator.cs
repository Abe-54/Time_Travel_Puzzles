using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VineActivator : PlantActivator
{
    public Tile vine;
    public Tilemap vineMap;

    public Transform[] vineLocations;

    protected override void HandleTimePeriodChanged()
    {
        if (timeSwapManager.currentTimePeriod == TimeSwapManager.TimePeriod.Present && arePlantsGrown)
        {
            GrowPlants();
            Debug.Log("Vines are grown");
        }
    }

    protected override void GrowPlants()
    {
        // Specific vine painting logic
        foreach (Transform vineTransform in vineLocations)
        {
            vineMap.SetTile(Vector3Int.FloorToInt(vineTransform.position), vine);
        }
    }
}
