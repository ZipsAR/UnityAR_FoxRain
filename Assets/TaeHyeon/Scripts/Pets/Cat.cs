using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class Cat : PetBase
{
    protected override void PetStatInitialize()
    {
        stat.fullness = 50;
        stat.tiredness = 30;
        stat.cleanliness = 60;
        stat.exp = 0;
        stat.level = 1;
        
        stat.speed = 10;
    }

    public override void InteractHead()
    {
        animator.Play("InteractHead");
        Logger.Log("Play InteractHead");
    }

    public override void InteractJaw()
    {
        animator.Play("InteractJaw");
        Logger.Log("Play InteractJaw");
    }

    public override void InteractBody()
    {
        animator.Play("InteractBody");
        Logger.Log("Play InteractBody");
    }

    public override void InteractHandDetection()
    {
        animator.Play("InteractHandDetection");
        Logger.Log("Play InteractHandDetection");
    }
}
