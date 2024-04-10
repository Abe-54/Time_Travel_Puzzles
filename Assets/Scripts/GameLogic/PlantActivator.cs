using UnityEngine;

public abstract class PlantActivator : MonoBehaviour
{
    public Seed correctSeed;
    protected TimeSwapManager timeSwapManager;

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
        if (timeSwapManager.currentTimePeriod == TimeSwapManager.TimePeriod.Past && other.CompareTag("Seed"))
        {
            Seed seed = other.GetComponent<Seed>();

            Debug.Log("Seed planted: " + seed.seedData.seedName);

            if (IsCorrectSeed(seed))
            {
                Debug.Log("Correct seed planted");
                arePlantsGrown = true;
            }

            Destroy(other.gameObject);
        }
    }
}
