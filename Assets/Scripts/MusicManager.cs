using UnityEngine;
using UnityEngine.Audio;
using Mirror;
using System;
using System.Linq;

public class MusicManager : MonoBehaviour
{
   public static MusicManager Singleton { get { return FindFirstObjectByType<MusicManager>(); } }
   [SerializeField] private AudioSource[] music;
   [SerializeField] private AudioSource[] powerupMusic;
   public int cur;
   [SerializeField] private AudioMixer am;
   private AudioMixerSnapshot[] snaps = { null, null };

   private float[] weights = { 0, 1 };
   private readonly int musicDenom = 2;
   private PlayerController me;

   // public float sens;
   // public int freq;

   // float[] specdata = new float[64];


   public float Vol { get; private set; }

   private void Start()
   {
      me = PlayerController.Me;
      snaps[0] = am.FindSnapshot("default");
      snaps[1] = am.FindSnapshot("cave");
   }

   // private void Update()
   // {
   //    music[0].GetSpectrumData(specdata, 0, FFTWindow.BlackmanHarris);
   //    string outstr = "";
   //    foreach (float item in specdata)
   //    {
   //       outstr += (item > sens ? item : 0).ToString() + ", ";
   //    }
  
   //    // print(outstr);
   //    // Vol = specdata[freq] > sens ? 10 : 0;
   // }

   private void FixedUpdate()
   {
      for (uint i = 0; i < music.Length; ++i)
      {
         music[i].mute = cur != i;
      }

      if (me != null) cur = Mathf.Clamp(me.Count / musicDenom / music.Length, 0, music.Length - 1);
   }

   [Client]
   public void SetCave(bool val)
   {
      weights[0] = val ? 0 : 1;
      weights[1] = val ? 1 : 0;
      am.TransitionToSnapshots(snaps, weights, .3f);
   }

   [Client]
   public void SetPowered(bool p, PType t = 0)
   {
      am.GetFloat("PowerupVol", out float vol);
      am.SetFloat("PowerupVol", Mathf.Lerp(vol, p ? 0 : -100, .2f ));
      am.GetFloat("BaseVol", out vol);
      am.SetFloat("BaseVol", Mathf.Lerp(vol, p ? -100 : 0, .2f));

      if (p)
      {
         for (int i = 0; i < powerupMusic.Length; ++i)
         {
            powerupMusic[i].volume = Mathf.Lerp(powerupMusic[i].volume, i == (int)t - 1 ? 1 : 0, .05f);
         }
      } else {
         for (int i = 0; i < powerupMusic.Length; ++i) {powerupMusic[i].volume = Mathf.Lerp(powerupMusic[i].volume, 0, .05f);}
      }
   }
}
