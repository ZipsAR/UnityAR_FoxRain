using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 에디터에서 쉽게 사용할 수 있도록 메뉴를 만듬
[ CreateAssetMenu( fileName = "InteractData", menuName = "Scriptable Object Asset/InteractData" )]
public class InteractData : ScriptableObject
{
    [Range(0,10)] public float playerPetMaxDistance; // 유저와 펫 사이의 최대 거리
    public float playerIdleTimeThreshold; // 이 시간이상 플레이어가 이동하지 않는 경우 펫이 않음
    
    // Brushing
    public bool isBrushing;
    public float brushingTime;
    public float brushingTimeThreshold;

    // Hand interaction Info
    public bool isColliding;
    public float collisionTimer;
    public float collisionTimeLimit;
    public PetParts curPetCollisionPart;
    public Coroutine checkPetCollisionTimeCoroutine;

    public bool isInteractionIgnored;
    public float interactionCoolTime;
}
