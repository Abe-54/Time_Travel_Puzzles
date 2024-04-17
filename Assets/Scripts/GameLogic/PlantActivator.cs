using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class PlantActivator : MonoBehaviour
{
    public Seed correctSeed;
    protected TimeSwapManager timeSwapManager;

    public Tile saplingTile;
    public Tilemap pastTilemap;
    public Transform saplingLocation;

    protected bool arePlantsGrown { get; private set; } = false;

    protected virtual void Start()
    {
        timeSwapManager = FindObjectOfType<TimeSwapManager>();
        timeSwapManager.OnTimePeriodChanged += HandleTimePeriodChanged;
        arePlantsGrown = false;
    }

    protected abstract void HandleTimePeriodChanged();
    protected abstract void GrowPlants();

    private bool IsCorrectSeed(Seed seed)
    {
        return seed != null && seed.seedData.seedName.CompareTo(correctSeed.seedData.seedName) == 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Seed seed = other.GetComponent<Seed>();

        if (timeSwapManager.currentTimePeriod == TimeSwapManager.TimePeriod.Past && other.CompareTag("Pick-Up") && seed != null)
        {
            Debug.Log("Seed planted: " + seed.seedData.seedName);

            if (IsCorrectSeed(seed))
            {
                Debug.Log("Correct seed planted");
                arePlantsGrown = true;
                pastTilemap.SetTile(pastTilemap.WorldToCell(saplingLocation.position), saplingTile);
            }

            Destroy(other.gameObject);
        }
    }
}
