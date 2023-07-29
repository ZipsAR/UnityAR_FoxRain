using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class Cat : PetBase
{
    public override void InteractHead()
    {
        animator.Play("B_picks");
        Logger.Log("Play B_picks");
    }

    public override void InteractJaw()
    {
        animator.Play("B_wash");
        Logger.Log("Play B_wash");
    }

    public override void InteractBody()
    {
        animator.Play("B_cry");
        Logger.Log("Play B_cry");
    }

    public override void InteractHandDetection()
    {
        animator.Play("C_massage");
        Logger.Log("Play C_massage");
    }
}
