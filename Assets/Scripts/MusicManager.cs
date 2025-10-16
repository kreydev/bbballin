using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
   public static MusicManager Singleton { get { return FindFirstObjectByType<MusicManager>(); } }
   [SerializeField] AudioSource[] music;
   public int cur;
   [SerializeField] AudioMixer am;
   AudioMixerSnapshot[] snaps = { null, null};
   float[] weights = { 0, 1 };
   readonly int musicDenom = 2;
   PlayerController me;


   void Start()
   {
      me = PlayerController.Me;
      snaps[0] = am.FindSnapshot("default");
      snaps[1] = am.FindSnapshot("cave");
   }

   void FixedUpdate()
   {
      for (uint i = 0; i < music.Length; ++i)
      {
         music[i].mute = cur != i;
      }

      if (me != null) cur = Mathf.Clamp(me.Count / musicDenom / music.Length, 0, music.Length - 1);
   }

   public void SetCave(bool val)
   {
      weights[0] = val ? 0 : 1;
      weights[1] = val ? 1 : 0;
      am.TransitionToSnapshots(snaps, weights, .3f);
   }
}
