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
    [SerializeField] private HandJointUpdater indexTop;

    [Space]

    // Middle
    [SerializeField] private HandJointUpdater middleBase;
    [SerializeField] private HandJointUpdater middleMiddle;
    [SerializeField] private HandJointUpdater middleTop;

    [Space]

    // Ring
    [SerializeField] private HandJointUpdater ringBase;
    [SerializeField] private HandJointUpdater ringMiddle;
    [SerializeField] private HandJointUpdater ringTop;

    [Space]

    // Pinky
    [SerializeField] private HandJointUpdater pinkyBase;
    [SerializeField] private HandJointUpdater pinkyMiddle;
    [SerializeField] private HandJointUpdater pinkyTop;

}
