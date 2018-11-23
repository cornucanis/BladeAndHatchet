using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance = null;


    private FMOD.Studio.EventInstance ambienceOutside;

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
        AmbienceOutsideStart();
    }

    public void AmbienceOutsideStart()
    {
        ambienceOutside.start();
    }

    public void AmbienceOutsideStop()
    {
        ambienceOutside.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    
    

}