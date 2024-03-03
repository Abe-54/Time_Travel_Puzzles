using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VineActivator : MonoBehaviour
{
    public Tile vine;
    public Tilemap vineMap;

    public Transform[] vineLocations;

    public Transform tpLocation;

    public bool areVinesPainted;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        areVinesPainted = false;

        gameManager.OnTimePeriodChanged += HandleTimePeriodChanged;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void HandleTimePeriodChanged()
    {
        if (gameManager.currentTimePeriod == GameManager.TimePeriod.Present && areVinesPainted)
        {
            PaintVines();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (gameManager.currentTimePeriod)
        {
            case GameManager.TimePeriod.Past:

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

                break;

            case GameManager.TimePeriod.Present:
                if (other.CompareTag("Player") && areVinesPainted)
                {
                    other.transform.position = tpLocation.position;
                }

                break;
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
