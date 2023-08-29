using QCHT.Interactions.Hands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingersData : MonoBehaviour
{
    [Space]

    // Thumb
    [SerializeField] public HandJointUpdater thumbBase;
    [SerializeField] public HandJointUpdater thumbMiddle;
    [SerializeField] public HandJointUpdater thumbTop;

    [Space]

    // Index
    [SerializeField] public HandJointUpdater indexBase;
    [SerializeField] public HandJointUpdater indexMiddle;
    [SerializeField] public HandJointUpdater indexTop;

    [Space]

    // Middle
    [SerializeField] public HandJointUpdater middleBase;
    [SerializeField] public HandJointUpdater middleMiddle;
    [SerializeField] public HandJointUpdater middleTop;

    [Space]

    // Ring
    [SerializeField] public HandJointUpdater ringBase;
    [SerializeField] public HandJointUpdater ringMiddle;
    [SerializeField] public HandJointUpdater ringTop;

    [Space]

    // Pinky
    [SerializeField] public HandJointUpdater pinkyBase;
    [SerializeField] public HandJointUpdater pinkyMiddle;
    [SerializeField] public HandJointUpdater pinkyTop;

}
