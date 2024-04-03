using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VineActivator : MonoBehaviour
{
    public Tile vine;
    public Tilemap vineMap;

    public Transform[] vineLocations;

    public bool areVinesPainted;

    // private GameManager gameManager;

    private TimeSwapManager timeSwapManager;

    // Start is called before the first frame update
    void Start()
    {
        // gameManager = FindObjectOfType<GameManager>();
        timeSwapManager = FindObjectOfType<TimeSwapManager>();

        areVinesPainted = false;

        timeSwapManager.OnTimePeriodChanged += HandleTimePeriodChanged;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void HandleTimePeriodChanged()
    {
        if (timeSwapManager.currentTimePeriod == TimeSwapManager.TimePeriod.Present && areVinesPainted)
        {
            PaintVines();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (timeSwapManager.currentTimePeriod == TimeSwapManager.TimePeriod.Past)
        {
            if (other.CompareTag("Seed"))
            {
                Debug.Log("SEED ENTERED");
                Seed seed = other.GetComponent<Seed>();

                if (seed.seedData.name.CompareTo("Vine") == 0)
                {
                    areVinesPainted = true;
                }

                Destroy(other.gameObject);
            }
        }
    }

    void PaintVines()
    {
        foreach (Transform vineTransform in vineLocations)
        {
            vineMap.SetTile(Vector3Int.FloorToInt(vineTransform.position), vine);
        }
    }
}
