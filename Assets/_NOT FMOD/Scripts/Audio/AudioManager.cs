using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance = null;


    private FMOD.Studio.EventInstance ambienceOutside;
  //  private FMOD.Studio.EventInstance music;

    private void Awake()

    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);


        DontDestroyOnLoad(this.gameObject);
    }

    void Start()

    {
        ambienceOutside = FMODUnity.RuntimeManager.CreateInstance(FMODPaths.AMBIENCE_OUTSIDE);
      //  music = FMODUnity.RuntimeManager.CreateInstance(FMODPaths.MUSIC);
        AmbienceOutsideStart();
    }

    public void AmbienceOutsideStart()
    {
        ambienceOutside.start();
    }
    /*
    void Update()
      {

          if (Input.GetKeyDown(KeyCode.Keypad1))
          {
              MusicStart();
          }

          if (Input.GetKeyDown(KeyCode.Keypad0))
          {
              MusicStop();
          }

          if (Input.GetKeyDown(KeyCode.Keypad2))
          {
              MusicTransitionToBeat();
          }

          if (Input.GetKeyDown(KeyCode.Keypad3))
          {
              MusicTransitionToBeatAndMelody();
          }

      }

     

  

    public void AmbienceOutsideStop()
    {
        ambienceOutside.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void MusicStart()
    {
        music.start();
    }

    public void MusicStop()
    {
        music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void MusicTransitionToBeat()
    {
        music.setParameterValue(FMODPaths.MUSIC_PARAM, 1f);
    }

    public void MusicTransitionToBeatAndMelody()
    {
        music.setParameterValue(FMODPaths.MUSIC_PARAM, 2f);
    }

     */

}