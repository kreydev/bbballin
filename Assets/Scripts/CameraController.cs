using UnityEngine;
using System.Collections;
using System.Diagnostics;
using Unity.VisualScripting;

public class CameraController : MonoBehaviour
{

   public GameObject player;
   public Vector3 offset;
   public Vector3 initialOffset;

   void Start()
   {
      // Create an offset by subtracting the Camera's position from the player's position
      offset = transform.position - player.transform.position;
      offset *= Mathf.Log(2) * .5f;
      initialOffset = transform.position - player.transform.position;
   }

   void FixedUpdate()
   {
      transform.LookAt(player.transform);
      // Set the position of the Camera (the game object this script is attached to)
      // to the player's position, plus the offset amount
      transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, .1f);
   }
}