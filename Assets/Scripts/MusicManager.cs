using UnityEngine;

public class MusicManager : MonoBehaviour
{
   [SerializeField] AudioSource[] music;
   [SerializeField] uint key;

   // Update is called once per frame
   void FixedUpdate()
   {
      for (uint i = 0; i < music.Length; ++i)
      {
         music[i].mute = key != i;
      }
   }
}
