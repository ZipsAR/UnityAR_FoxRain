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
        throw new NotImplementedException();
    }

    public override void InteractBody()
    {
        throw new NotImplementedException();
    }
}
