using Mirror;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;


// Include the namespace required to use Unity UI
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour {

   // Create public variables for player speed, and for the Text UI game objects
   public float speed;
   public Text countText;
   public Text winText;

   // Create private references to the rigidbody component on the player, and the count of pick up objects picked up so far
   private Rigidbody rb;
   int count;
   [Command(requiresAuthority = false)] void setCount(int value) { count = value; }
   public int Count { get { return count; }  set { setCount(value); } }
   public float caveHeight;
   public bool Walled { get { return Physics.Raycast(transform.position, Vector3.back, maxDistance: (Size + 3) * 1.5f, layerMask: LayerMask.GetMask("wall")); } }
   public bool Caved { get { return Physics.Raycast(rb.position, Vector3.up, maxDistance: (Size + 3 + caveHeight) * 1.5f, layerMask: LayerMask.GetMask("wall")); } }

   [SerializeField] CameraController cam;

   public float Size { get { return Count >= 1 ? Mathf.Log(Count + 1) * 2 : 1f; } }

   MusicManager mm;
   IngameGUI igg;
   GameManager gm;
   public static PlayerController Me { get
      {
         var pcs = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
         if (pcs.Length == 0) return null;
         if (pcs.Length > 1)
            foreach (var p in pcs) { if (p.isLocalPlayer) return p; }
         return pcs[0];
      }
   }
   

   // At the start of the game..
   void Start()
   {
      // Assign the Rigidbody component to our private rb variable
      rb = GetComponent<Rigidbody>();

      // Set the count to zero 
      Count = 1;
      mm = MusicManager.Singleton;
      gm = GameManager.Singleton;
      igg = IngameGUI.Singleton;
      igg.CoinText = Count.ToString();   
   }

   // Each physics step..
   void FixedUpdate()
   {
      // Set some local float variables equal to the value of our Horizontal and Vertical Inputs
      float moveHorizontal = Input.GetAxis("Horizontal");
      float moveVertical = Input.GetAxis("Vertical");

      // Create a Vector3 variable, and assign X and Z to feature our horizontal and vertical float variables above
      Vector3 movement = new(moveHorizontal, 0.0f, moveVertical);

      // Add a physical force to our Player rigidbody using our 'movement' Vector3 above, 
      // multiplying it by 'speed' - our public player speed that appears in the inspector
      rb.AddForce(movement * speed);
      transform.localScale = Vector3.Slerp(transform.localScale, Vector3.one * Size, .1f);
      mm.SetCave(Caved);

   }

   // When this game object intersects a collider with 'is trigger' checked, 
   // store a reference to that collider in a variable named 'other'..
   void OnTriggerEnter(Collider other)
   {
      // ..and if the game object we intersect has the tag 'Pick Up' assigned to it..
      if (other.gameObject.CompareTag("Pick Up"))
      {
         // Make the other game object (the pick up) inactive, to make it disappear
         other.GetComponent<Pickup>().PickedUp();
         GameManager.Singleton.SpawnNewCoin();

         // Add one to the score variable 'count'
         ++Count;
         igg.CoinText = Count.ToString();
      }
   }
   
   void OnCollisionEnter(Collision other)
   {
      if (other.collider.CompareTag("Player")) { gm.PlayerInteract(this, other.gameObject.GetComponent<PlayerController>()); }
   }

}