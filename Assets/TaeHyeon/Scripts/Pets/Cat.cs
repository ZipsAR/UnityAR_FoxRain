using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class Cat : PetBase
{
    public override void InitializeStatByDefault()
    {
        stat.fullness = 70;
        stat.tiredness = 30;
        stat.cleanliness = 60;
        stat.exp = 0;
        stat.level = 1;
        
        stat.speed = 15;
    }
}
