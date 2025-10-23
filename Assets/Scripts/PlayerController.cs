using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Playables;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour {

   // Create public variables for player speed, and for the Text UI game objects
   readonly float jumpVel = 5f;
   readonly float baseSpeed = 10f;
   readonly float fastSpeed = 23f;
   float speed;

   public bool HasEffect(PType t) { return effectTypes.IndexOf(t) != -1; }

   private Rigidbody rb;
   [SyncVar] int count;
   [Command(requiresAuthority = false)] void SetCount(int value) { count = value; }
   public int Count { get { return count; }  set { SetCount(value); } }
   public float caveHeight;
   [SyncVar] [SerializeField] List<PType> effectTypes = new();
   [SyncVar] [SerializeField] List<float> effectTimes = new();
   [Command(requiresAuthority = false)] void AddEffect(PType e) { effectTypes.Add((e)); effectTimes.Add(45); }

   public bool Walled { get { return Physics.Raycast(transform.position, Vector3.back, maxDistance: (Size + 3) * 1.5f, layerMask: LayerMask.GetMask("wall")); } }
   public bool Caved { get { return Physics.Raycast(rb.position, Vector3.up, maxDistance: (Size + 3 + caveHeight) * 1.5f, layerMask: LayerMask.GetMask("wall")); } }
   public bool CanJump { get {
      float step = 2 * Mathf.Deg2Rad;
      for (float i = 0; i < 2 * Mathf.PI; i += step)
      {
         if (Physics.Raycast(rb.position + new Vector3((Size - .3f) * Mathf.Cos(i), 0, (Size -.3f) * Mathf.Sin(i)), Vector3.down, maxDistance: Size + 1, layerMask: LayerMask.GetMask("wall"))) return true;
      }
      return false;
   } }


   [SerializeField] readonly CameraController cam;

   public float Size { get { return Count >= 1 ? Mathf.Log(Count + 1) * 2 : 1f; } }

   MusicManager mm;
   IngameGUI igg;
   GameManager gm;
   public static PlayerController Me
   {
      get
      {
         var pcs = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
         if (pcs.Length > 1)
         {
            foreach (var p in pcs) { if (p.isLocalPlayer) return p; }
            return pcs[0];
         }
         else return null;
      }
   }
   
   [ClientRpc] void UpdateClientEffects()
   {
      for (int i = 0; i < effectTypes.Count; ++i)
      {
         igg.SetIconState(effectTypes[i], true, effectTimes[i]);
      }

      if (effectTimes.Count > 0)
         mm.SetPowered(true, effectTypes.Last());
      else
         mm.SetPowered(false);
   }
   
   
   [Command(requiresAuthority = false)] void EffectsTick() {
      for (int i = 0; i < effectTimes.Count; ++i)
      {
         if (effectTimes[i] <= 0)
         {
            igg.SetIconState(effectTypes[i], false, effectTimes[i]);
            effectTimes.RemoveAt(i);
            effectTypes.RemoveAt(i);
         }
         else
         {
            effectTimes[i] -= 0.0167f;
         }
      }
      UpdateClientEffects();
   }

   // At the start of the game..
   void Start()
   {
      speed = (float)baseSpeed; // cast to a float to copy instead of referencing
         
      rb = GetComponent<Rigidbody>();
      mm = MusicManager.Singleton;
      gm = GameManager.Singleton;
      igg = IngameGUI.Singleton;
      if (isLocalPlayer) {
         SetCount(1);
         igg.CoinText = Count.ToString();
         igg.ShowIcons = true;
         var output = mm.GetComponent<PlayableDirector>().playableAsset.outputs.Where(o => o.streamName == "strack").ElementAt(0);
         // output.sourceObject = this;
      } else
      {
         cam.gameObject.SetActive(false);
      }
   }

   [Client] void Update()
   {
      if (!NetworkClient.isConnected) return;
      if (!isLocalPlayer) return;

      if (CanJump && HasEffect(PType.Jumper) && Input.GetKeyDown(KeyCode.Space)) rb.AddForce(jumpVel * Size * Vector3.up, ForceMode.Impulse);
   }

   [Client] void FixedUpdate()
   {
      if (!NetworkClient.isConnected) return;
      if (!isLocalPlayer) return;

      float moveHorizontal = Input.GetAxis("Horizontal");
      float moveVertical = Input.GetAxis("Vertical");
      Vector3 movement = new(moveHorizontal, 0f, moveVertical);

      speed = HasEffect(PType.Speedster) ? fastSpeed : baseSpeed;
      if (HasEffect(PType.Vector))
      {
         movement.y = (Input.GetKey(KeyCode.E) ? .8f : 0f) + (Input.GetKey(KeyCode.Q) ? -.8f : 0f);
         rb.linearVelocity = movement * speed * 1.2f;
      }
      else
      {
         movement.y = 0;
         rb.AddForce(movement * speed);
      }

      if (HasEffect(PType.Maniac)) {
         for (int i = 0; i < gm.pickupRoot.childCount; ++i)
         {
            var c = gm.pickupRoot.GetChild(i);
            if (Vector3.Distance(transform.position, c.position) < (Size + 8) && NetworkClient.ready)
            {
               gm.PullObject(GetComponent<NetworkTransformReliable>(), c.GetComponent<NetworkTransformReliable>());
            }
         }
      }


      transform.localScale = Vector3.Slerp(transform.localScale, Vector3.one * Size, .1f);
      mm.SetCave(Caved);
      if (Caved)
      {
         rb.AddForce(Vector3.up * 10);
      }

      EffectsTick();
   }

   void OnTriggerEnter(Collider other)
   {
      if (!isLocalPlayer) return;
      if (other.gameObject.CompareTag("Pick Up"))
      {
         Pickup opick = other.GetComponent<Pickup>();

         if (opick.type == PType.Coin)
         {
            SetCount(Count + 1);
            igg.CoinText = Count.ToString();
            gm.SpawnNewPickup(0);
         } else
         {
            effectTypes.Add(opick.type);
            effectTimes.Add(20f);
            gm.SpawnNewPickup(opick.type);
         }
         opick.PickedUp();
      }
   }

   void OnCollisionEnter(Collision other)
   {
      if (!isLocalPlayer) return;
      if (other.collider.CompareTag("Player")) {
         if (!other.collider.GetComponent<PlayerController>().HasEffect(PType.Maniac))
            gm.PlayerInteract(this, other.gameObject.GetComponent<PlayerController>());
      }
   }

   [ClientRpc] public void RefreshScore()
   {
      if (isLocalPlayer) igg.CoinText = Count.ToString();
   }

}