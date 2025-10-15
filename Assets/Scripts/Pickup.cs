using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class Pickup : MonoBehaviour
{
	public AudioClip pickupSound;

	// Before rendering each frame..
	void Update()
	{
		// Rotate the game object that this script is attached to by 15 in the X axis,
		// 30 in the Y axis and 45 in the Z axis, multiplied by deltaTime in order to make it per second
		// rather than per frame.
		transform.Rotate(new Vector3(15, 30, 45) * (Time.deltaTime * 2));
	}

	public void PickedUp() { StartCoroutine(nameof(PickedUpC)); }
	
	IEnumerator PickedUpC()
	{
		GetComponent<Renderer>().enabled = false;
		GetComponent<AudioSource>().PlayOneShot(pickupSound);
		yield return new WaitForSecondsRealtime(2);
		Destroy(gameObject);
	}
}	