using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CmdDetail
{
    public int cmdIdx; // Cmd enum index
    public Vector3 targetDir; // Position the pet will move to
    public GameObject targetObj; // Snack or Toy
    
    public CmdDetail(int cmdIdx, Vector3 targetDir, GameObject targetObj)
    {
         this.cmdIdx = cmdIdx;
         this.targetDir = targetDir;
         this.targetObj = targetObj;
    }
}
