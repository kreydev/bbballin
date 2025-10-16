using UnityEngine;
using System.Collections;
using Mirror;
public enum PType { Coin, Vector, Speedster, Maniac,  }

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class Pickup : NetworkBehaviour
{
	public AudioClip pickupSound;
	Vector3 rotVel;


	void Awake()
	{
		rotVel = new Vector3(Random.Range(13, 17), Random.Range(28, 32), Random.Range(43, 47)) * 2;
	}

	// Before rendering each frame..
	void Update()
	{
		// Rotate the game object that this script is attached to by 15 in the X axis,
		// 30 in the Y axis and 45 in the Z axis, multiplied by deltaTime in order to make it per second
		// rather than per frame.
		transform.Rotate(rotVel * Time.deltaTime);
	}

	[Command(requiresAuthority = false)]
	public void PickedUp()
	{
		PlaySounds();
		StartCoroutine(nameof(PickedUpC));
	}

	[ClientRpc]
	void PlaySounds()
	{
		GetComponent<Renderer>().enabled = false;
		AudioSource ac = GetComponent<AudioSource>();
		ac.pitch = Random.Range(.9f, 1.3f);
		ac.PlayOneShot(pickupSound);
	}


	IEnumerator PickedUpC()
	{
		yield return new WaitForSecondsRealtime(2);
		NetworkServer.UnSpawn(gameObject);
		Destroy(gameObject);
	}
}	