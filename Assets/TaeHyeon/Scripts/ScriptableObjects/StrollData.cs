using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 에디터에서 쉽게 사용할 수 있도록 메뉴를 만듬
[ CreateAssetMenu( fileName = "StrollData", menuName = "Scriptable Object Asset/StrollData" )]
public class StrollData : ScriptableObject
{
    [Range(0,10)] public float playerPetMaxDistance; // 유저와 펫 사이의 최대 거리
    public float strollTime; // 산책 경과 시간
    public int collectedItemCount; // 산책 중 획득한 아이템 개수 
    public int wildPetEncounterCount; // 산책 중 야생 펫을 만난 횟수
    public int obstacleClearCount; // 산책 중 극복한 장애물 개수
    public float playerIdleTimeThreshold; // 이 시간이상 플레이어가 이동하지 않는 경우 펫이 않음
    
    public void Init()
    {
        // Set all attribute to zero except "playerPetMaxDistance"
        strollTime = 0f;
        collectedItemCount = 0;
        wildPetEncounterCount = 0;
        obstacleClearCount = 0;
    }
}
