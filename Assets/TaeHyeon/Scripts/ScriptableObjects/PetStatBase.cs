using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PetStatNames
{
    Fullness,
    Tiredness,
    Cleanliness,
    Exp,
    Level,
}

// 에디터에서 쉽게 사용할 수 있도록 메뉴를 만듬
[ CreateAssetMenu( fileName = "PetStat", menuName = "Scriptable Object Asset/PetStat" )]
public class PetStatBase : ScriptableObject
{
    [Header("Interact Part Stats")]
    [Range(0,100)]public int fullness; // 포만감
    [Range(0,100)]public int tiredness; // 피로도
    [Range(0,100)]public int cleanliness; // 청결도
    [Range(0,100)]public int exp; // 경험치
    [Range(1,10)]public int level; // 레벨
    
    [Header("Agility Part Stats")]
    [Range(0,100)] public int health; // 체력
    [Range(0,100)] public int speed; // 이동속도
    [Range(0,100)] public int reactionSpeed; // 반응속도
    [Range(0,100)] public int jumpPower; // 점프력
    [Range(0,100)] public int disobedienceRate; // 이해력
}
