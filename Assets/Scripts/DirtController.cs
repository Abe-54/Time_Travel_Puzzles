using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DirtController : MonoBehaviour
{
    public bool hasSeed = false;

    public Seed seed;

    public GameObject seedBag;

    public Tile tile;
    public Tilemap pastMap;
    public Transform locationToPlant;

    public TreeController tree;
    public GameObject Axe;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (hasSeed)
        {
            if (seed.seedData.seedName == "Tree")
            {
                pastMap.SetTile(Vector3Int.FloorToInt(locationToPlant.position), tile);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Seed"))
        {
            Debug.Log("Seed has been planted");
            seed = other.gameObject.GetComponent<Seed>();
            hasSeed = true;
            tree.gameObject.SetActive(true);
            tree.isGrown = true;
            seedBag.SetActive(false);
            Axe.SetActive(true);
            Destroy(other.gameObject);
        }
    }
}
