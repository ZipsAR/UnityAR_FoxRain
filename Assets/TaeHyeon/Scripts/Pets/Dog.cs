using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class Dog : PetBase
{
    public override void InitializeStatByDefault()
    {
        stat.fullness = 65;
        stat.tiredness = 40;
        stat.cleanliness = 70;
        stat.exp = 0;
        stat.level = 1;

        stat.speed = 15;
    }
}
