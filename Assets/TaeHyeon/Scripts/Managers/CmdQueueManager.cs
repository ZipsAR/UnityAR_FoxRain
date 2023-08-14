using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using Logger = ZipsAR.Logger;

public static class CmdQueueManager
{
    public static void ExecuteCmd(PetBase pet, CmdDetail cmdDetail)
        {
            switch (cmdDetail.cmdIdx)
            {
                case (int)Cmd.Move:
                    // Go to current player position
                    if (cmdDetail.targetDir == default)
                    {
                        pet.CmdMoveTo(Utils.GetPointBeforeDistance(
                            pet.gameObject.transform.position, 
                            GameManager.Instance.player.transform.position, 
                            GameData.playerFrontDistance));
                    }
                    else
                    {
                        if (cmdDetail.targetObj == default)
                        {
                            pet.CmdMoveTo(cmdDetail.targetDir);
                        }
                        else
                        {
                            // purpose true means pet move to snack or toy
                            pet.CmdMoveTo(cmdDetail.targetDir, true);   
                        }
                    }
                    break;
                
                case (int)Cmd.Look:
                    if(cmdDetail.targetDir == default)
                        pet.CmdLook(GameManager.Instance.player.gameObject.transform.position);
                    else
                        pet.CmdLook(cmdDetail.targetDir);
                    break;
                case (int)Cmd.Sit:
                    pet.CmdSit();
                    break;
                case (int)Cmd.Eat:
                    pet.CmdEat(cmdDetail.targetObj);
                    break;
                case (int)Cmd.Brush:
                    pet.CmdBrush();
                    break;
                case (int)Cmd.Bite:
                    Logger.Log("bite obj name : " + cmdDetail.targetObj);
                    pet.CmdBite(cmdDetail.targetObj);
                    break;
                case (int)Cmd.Spit:
                    pet.CmdSpit();
                    break;
                default:
                    throw new Exception("Unimplemented command");
            }
        }
    
    public static void EnqueueCmd(Queue<CmdDetail> targetQueue, Cmd cmd, Vector3 pos = default, GameObject targetObj = default)
    {
        if (cmd == Cmd.Eat && targetObj == default) throw new Exception("eat cmd must include targetObj");
        if (cmd == Cmd.Bite && targetObj == default) throw new Exception("bite cmd must include targetObj");
            
        // if (cmd == Cmd.Look && pos == default) means look current player position
        // if (cmd == Cmd.Move && pos == default) means goto current player position
            
        targetQueue.Enqueue(new CmdDetail((int)cmd, pos, targetObj));
    }
    
    /// <summary>
    /// Get top of cmd
    /// </summary>
    /// <param name="result">dequeue command value</param>
    /// <returns>Whether deqeue is successful</returns>
    public static bool DequeCmd(Queue<CmdDetail> targetQueue, out CmdDetail result)
    {
        // if cmdQueue is empty
        if (targetQueue.Count == 0)
        {
            // Meaningless data
            result = new CmdDetail((int)Cmd.Move, Vector3.zero, null);
            return false;
        }
    
        result = targetQueue.Dequeue();
        return true;
    }
    
    public static void ClearCmdQueue(Queue<CmdDetail> targetQueue)
    {
        targetQueue.Clear();
        Logger.Log("targetQueue clear");
    }

    public static string ShowCurQueue(Queue<CmdDetail> targetQueue)
    {
        string str = "";
        foreach (CmdDetail val in targetQueue)
        {
            str += $"{val}\n";
        }

        str += "\n";
        Logger.Log(str);
        return str;
    }
}
