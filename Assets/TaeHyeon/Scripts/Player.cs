using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class Player : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, StrollManager.Instance.strollData.playerPetMaxDistance);
    }
}
