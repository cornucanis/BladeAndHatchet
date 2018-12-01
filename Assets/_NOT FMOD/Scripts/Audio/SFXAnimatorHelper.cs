using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXAnimatorHelper : MonoBehaviour {

    SFXAnimationsChar playerSFX;

    
    void Start () {
        playerSFX = GetComponentInChildren<SFXAnimationsChar>();
    }

    public void AttackSFX_1()
    {
        playerSFX.AttackSFX_1();
    }

    public void AttackSFX_2()
    {
        playerSFX.AttackSFX_2();
    }

    public void AttackSFX_3()
    {
        playerSFX.AttackSFX_3();
    }

    public void JumpSFX()
    {
        playerSFX.JumpSFX();
    }

    public void FallSFX()
    {
        playerSFX.FallSFX();
    }

    public void FootstepsSFX()
    {
        playerSFX.FootstepsSFX();
    }



}
