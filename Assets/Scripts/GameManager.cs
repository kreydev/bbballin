using System.Collections;
using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public GameObject coinPrefab;
    public GameObject[] powerupPrefabs;
    public Transform pickupRoot;

    public static GameManager Singleton { get { return FindFirstObjectByType<GameManager>(); } }

    const float boxSize = 45f;
    const float pullSpeed = 1.75f;
    NetworkManager nm;


    private IEnumerator WaitForNetwork()
    {
        yield return new WaitUntil(() => { return NetworkServer.active; });
        yield return new WaitUntil(() => { return nm.numPlayers >= 1; });
        for (uint i = 0; i < 100; ++i) { SpawnNewPickupInternal(coinPrefab); }
        foreach (var p in powerupPrefabs) { SpawnNewPickupInternal(p); }
    }

    private void Awake()
    {
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        nm = NetworkManager.singleton;
        nm.networkAddress = "localhost";
        nm.StartHost();
        StartCoroutine(WaitForNetwork());
    }

    [Command(requiresAuthority = false)]
    public void SpawnNewPickup(PType type)
    {
        if (type == 0) SpawnNewPickupInternal(coinPrefab);
        else
        {
            SpawnNewPickupInternal(powerupPrefabs[(int)type - 1]);
        }
    }
    
    private void SpawnNewPickupInternal(GameObject prefab)
    {
        GameObject p = Instantiate(prefab);
        NetworkServer.Spawn(p);
        
        p.transform.SetParent(pickupRoot);
        p.transform.position = new Vector3(Random.Range(-boxSize, boxSize), Mathf.Floor(Random.Range(0, 2)) * 100 + 1f, Random.Range(-boxSize, boxSize));
        Physics.Raycast(p.transform.position, Vector3.down, out RaycastHit hit, maxDistance: 100000, LayerMask.GetMask("wall"));
        p.transform.position += Vector3.down * (hit.distance - .5f);
    }
    


    [Command(requiresAuthority = false)]
    public void PlayerInteract(PlayerController p1, PlayerController p2)
    {
        // print("P1: " + p1.Count + " P2: " + p2.Count);
        if (p1.Count > (p2.Count * 1.2))
        {
            p1.Count += p2.Count;
            p2.Count = 0;
            p1.RefreshScore();
            p2.RefreshScore();
        }
    }

    [Command(requiresAuthority = false)]
    public void PullObject(NetworkTransformReliable player, NetworkTransformReliable other)
    {
        if (other == null || player == null) return;
        var d = Vector3.Distance(player.transform.position, other.transform.position);
        other.transform.Translate((player.transform.position - other.transform.position).normalized * pullSpeed * (1/(d*3)), Space.World);
    }
}
