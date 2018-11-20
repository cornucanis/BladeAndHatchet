using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXAnimationsChar : MonoBehaviour {

    private float fallImpact;
    FMOD.Studio.EventInstance fallSFX;

    // Use this for initialization
    void Start () {
        
    }

    void Update()
    {
        
    }
	
    public void AttackSFX_1()
    {
        FMODUnity.RuntimeManager.PlayOneShot(FMODPaths.ATTACK_1, GetComponent<Transform>().position);
    }

    public void AttackSFX_2()
    {
        FMODUnity.RuntimeManager.PlayOneShot(FMODPaths.ATTACK_2, GetComponent<Transform>().position);
    }

    public void AttackSFX_3()
    {
        FMODUnity.RuntimeManager.PlayOneShot(FMODPaths.ATTACK_3, GetComponent<Transform>().position);
    }

    public void JumpSFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot(FMODPaths.JUMP, GetComponent<Transform>().position);
    }

    public void FallSFX()
    {
        if (fallImpact > 30f)
        {
            FMODUnity.RuntimeManager.PlayOneShot(FMODPaths.FALL_STONE, GetComponent<Transform>().position);
            //Debug.Log("Hard fall impact");
        }
        else 
        {
            //FMODUnity.RuntimeManager.PlayOneShot(FMODPaths.FALL_STONE_LIGHT, GetComponent<Transform>().position);

            fallSFX = FMODUnity.RuntimeManager.CreateInstance(FMODPaths.FALL_STONE_LIGHT);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(fallSFX, GetComponent<Transform>(), GetComponent<Rigidbody>());
            fallSFX.setParameterValue(FMODPaths.FALL_IMPACT, fallImpact);
            fallSFX.start();
            //Debug.Log("Light fall impact");
        }
      
    }

    public void FootstepsSFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot(FMODPaths.FALL_STONE_LIGHT, GetComponent<Transform>().position);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        fallImpact = col.relativeVelocity.magnitude;
    }

}
