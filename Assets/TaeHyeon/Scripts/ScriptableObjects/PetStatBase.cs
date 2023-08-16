using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PetStatBase
{
    public int statMin = 0;
    public int statMax = 100;

    public int expMax = 100;
    public int levelMax = 10;
    
    [Header("Interact Part Stats")]
    [Range(0,100)]public int fullness = 0; // 포만감
    [Range(0,100)]public int tiredness = 0; // 피로도
    [Range(0,100)]public int cleanliness = 0; // 청결도
    [Range(0,100)]public int exp = 0; // 경험치
    [Range(1,10)]public int level = 1; // 레벨
    
    [Header("Agility Part Stats")]
    [Range(0,100)] public int health; // 체력
    [Range(0,100)] public int speed; // 이동속도
    [Range(0,100)] public int reactionSpeed; // 반응속도
    [Range(0,100)] public int jumpPower; // 점프력
    [Range(0,100)] public int disobedienceRate; // 이해력
}
