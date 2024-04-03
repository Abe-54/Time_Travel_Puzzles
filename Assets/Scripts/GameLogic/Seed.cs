using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    public SeedSO seedData;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = seedData.seedIcon;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
