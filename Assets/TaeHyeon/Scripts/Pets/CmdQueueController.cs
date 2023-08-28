using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using Logger = ZipsAR.Logger;

public static class CmdQueueController
{
    public static void ExecuteCmd(PetBase pet, CmdDetail cmdDetail)
    {
        switch (cmdDetail.cmdIdx)
        {
            case (int)Cmd.Move:
                Vector3 targetDir = cmdDetail.targetDir;

                // Go to current player position
                if (targetDir == default)
                {
                    targetDir = Utils.GetPointBeforeDistance(
                        pet.gameObject.transform.position, 
                        GameManager.Instance.player.transform.position,
                                GameData.playerFrontDistance);
                }
                
                // If targetObj is not default,
                // Exclamation marks are displayed above the pet's head when moving
                pet.CmdMoveTo(targetDir, cmdDetail.targetObj != default);
                break;
            
            case (int)Cmd.Look:
                // If targetDir is default, look at the player
                pet.CmdLook(cmdDetail.targetDir == default
                    ? GameManager.Instance.player.gameObject.transform.position
                    : cmdDetail.targetDir);
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

        targetQueue.Enqueue(new CmdDetail((int)cmd, pos, targetObj));
    }

    /// <summary>
    /// Get top of cmd
    /// </summary>
    /// <param name="targetQueue">Queue to modify</param>
    /// <param name="result">dequeue command value</param>
    /// <returns>Whether deqeue is successful</returns>
    public static bool DequeCmd(Queue<CmdDetail> targetQueue, out CmdDetail result)
    {
        // if cmdQueue is empty
        if (targetQueue.Count == 0)
        {
            // Meaningless data
            result = default;
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
