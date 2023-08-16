using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class PetObject
{
    public PetType type;
    public GameObject prefabObj;
    public GameObject instanceObj;
    public Transform spawnPos;
    public Queue<CmdDetail> cmdQueue = new Queue<CmdDetail>();
}
