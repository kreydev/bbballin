using UnityEngine;

public class CameraController : MonoBehaviour
{

   public PlayerController player;
   public Vector3 offset;
   public Vector3 initialOffset;
   Vector2 mouse;
   readonly float sens = 1/2f;

   void Start()
   {
      // Create an offset by subtracting the Camera's position from the player's position
      offset = transform.position - player.transform.position;
      offset *= Mathf.Log(2) * .5f;
      initialOffset = transform.position - player.transform.position;
   }
   void Update()
   {
      mouse = Input.mousePosition;
      if (!player.Walled)
         transform.LookAt(player.transform);
      else
         transform.localEulerAngles = new Vector3(Mathf.Clamp(mouse.y * sens, -90, 90), mouse.x * sens, 0);

         //TODO: Clamp rotation
   }

   void FixedUpdate()
   {
      if (player.Walled)
      {
         offset = new Vector3(0, 0, -0.4f);
         transform.localPosition = player.transform.position + offset;
      }
      else
      {
         offset = Vector3.Lerp(offset, initialOffset * player.Size / 5, .2f);
         transform.localPosition = Vector3.Lerp(transform.localPosition, player.transform.position + offset, .5f);
      }
   }
}