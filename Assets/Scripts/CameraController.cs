using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{

   MusicManager mm;
   public PlayerController player;
   public Vector3 offset;
   public Vector3 initialOffset;
   // Vector2 mouse;
   private float fovset = 0;
   // readonly float sens = 1 / 2f;
   public float FOV { get { return GetComponent<Camera>().fieldOfView; } set { foreach (var c in GetComponentsInChildren<Camera>()) { c.fieldOfView = value + fovset; } } }


   private IEnumerator PulseLerp()
   {
      while (fovset > -1.88f) {
         fovset = Mathf.Lerp(fovset, -2f, .4f);
         yield return new WaitForFixedUpdate();
      }
      while (fovset < -0.02f)
      {
         fovset = Mathf.Lerp(fovset, 0, .4f);
         yield return new WaitForFixedUpdate();
      }
      fovset = 0;
   }

   public void PulseEvent()
   {
      StopAllCoroutines();
      StartCoroutine(PulseLerp());
   }


   void Start()
   {
      mm = MusicManager.Singleton;
      if (player == null) return;
      // Create an offset by subtracting the Camera's position from the player's position
      offset = transform.localPosition - player.transform.localPosition;
      offset *= Mathf.Log(2) * .5f;
      initialOffset = transform.localPosition - player.transform.localPosition;
   }

   void Update()
   {
      if (player == null) return;
      // mouse = Input.mousePosition;
      // if (!player.Walled)
         transform.LookAt(player.transform);
      // else
      //    transform.localEulerAngles = new Vector3(Mathf.Clamp(mouse.y * sens, -90, 90), mouse.x * sens, 0);

         //TODO: Clamp rotation
   }

   void FixedUpdate()
   {
      fovset = mm.Vol;
      if (player == null)
      {
         FOV = 60; // this is necessary i swear
         return;
      }
      FOV = Mathf.Lerp(FOV, player.Caved ? 35 : 60, .2f);
      // if (player.Walled)
      // {
      //    offset = new Vector3(0, 0, -0.4f);
      //    transform.localPosition = player.transform.position + offset;
      // }
      // else
      // {
         offset = Vector3.Lerp(offset, initialOffset * player.Size / 5, .2f);
         transform.localPosition = Vector3.Lerp(transform.localPosition, player.transform.localPosition + offset, .5f);
      // }
   }
}