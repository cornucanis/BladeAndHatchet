using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveReverbSFX : MonoBehaviour {

    private FMOD.Studio.EventInstance caveReverb;


    void OnTriggerEnter2D (Collider2D collider)
    {
     
        if (collider.gameObject.tag == "Player")
        {

            caveReverb = FMODUnity.RuntimeManager.CreateInstance(FMODPaths.CAVE_REVERB);
            caveReverb.start();
        }
    }

    void OnTriggerExit2D (Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {

            caveReverb.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            caveReverb.release();
        }
    }



}
