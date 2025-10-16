using System.Collections;
using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public GameObject coinPrefab;
    public Transform pickupRoot;

    public static GameManager Singleton { get { return FindFirstObjectByType<GameManager>(); } }

    const float boxSize = 45f;
    NetworkManager nm;

    IEnumerator WaitForNetwork()
    {
        var wfs = new WaitForSeconds(0.25f);
        while (nm.numPlayers < 1)
        {
            yield return wfs;
        }
        for (uint i = 0; i < 100; ++i) { SpawnNewCoin(); }
    }

    void Start()
    {
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        nm = NetworkManager.singleton;
        StartCoroutine(WaitForNetwork());
    }

    void SpawnNewPickup(GameObject prefab)
    {
        GameObject p = Instantiate(prefab);
        NetworkServer.Spawn(p);
        p.transform.SetParent(pickupRoot);
        p.transform.position = new Vector3(Random.Range(-boxSize, boxSize), Mathf.Floor(Random.Range(0, 2)) * 100 + 1f, Random.Range(-boxSize, boxSize));
        Physics.Raycast(p.transform.position, Vector3.down, out RaycastHit hit, maxDistance: 100000, LayerMask.GetMask("wall"));
		p.transform.position += Vector3.down * (hit.distance - .5f);
		// Debug.DrawRay(p.transform.position, Vector3.down * 5);
		// print(hit.distance);
    }

    [Command(requiresAuthority=false)]
    public void SpawnNewCoin()
    {
        SpawnNewPickup(coinPrefab);
    }

    [Command(requiresAuthority=false)]
    public void PlayerInteract(PlayerController p1, PlayerController p2)
    {
        // print("P1: " + p1.Count + " P2: " + p2.Count);
        if (p1.Count > (p2.Count * 1.2)) {
            p1.Count += p2.Count;
            p2.Count = 0;
            p1.RefreshScore();
            p2.RefreshScore();
            // NetworkServer.UnSpawn(p2.gameObject);
            // NetworkServer.Spawn(p2.gameObject);
        }
    }
}
