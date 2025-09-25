using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject coinPrefab;
    public Transform pickupRoot;

    public static GameManager Singleton { get { return FindFirstObjectByType<GameManager>(); } }

    const float boxSize = 45f;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        for (uint i = 0; i < 90; ++i)
        {
            SpawnNewCoin();
        }
    }

    public void SpawnNewPickup(GameObject prefab, bool lockToFloor = true)
    {
        GameObject p = Instantiate(prefab);
        p.transform.SetParent(pickupRoot);
        p.transform.position = new Vector3(Random.Range(-boxSize, boxSize), lockToFloor ? .5f : Random.Range(0f, boxSize) , Random.Range(-boxSize, boxSize));
    }

    public void SpawnNewCoin(bool lockToFloor = true)
    {
        SpawnNewPickup(coinPrefab, lockToFloor);
    }
}
