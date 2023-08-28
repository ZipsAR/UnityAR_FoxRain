using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = ZipsAR.Logger;
using PlayMode = EnumTypes.PlayMode;
using Random = UnityEngine.Random;

public class IntroPetController : MonoBehaviour
{
    // It is normal that index 0 is a null value
    [SerializeField] private List<PetObject> introPets;

    private int petTypeCount;
    
    private void Awake()
    {
        petTypeCount = Enum.GetValues(typeof(PetType)).Length;
        
        // It is normal that index 0 is a null value
        if (introPets.Count != petTypeCount)
        {
            throw new Exception("number of pet objects must be equal with number of PetType");
        }

        // Spawn pets start with index 1
        for (int i = 1; i < petTypeCount; i++)
        {
            introPets[i].instanceObj = Instantiate(introPets[i].prefabObj, introPets[i].spawnPos);
            introPets[i].instanceObj.GetComponent<PetBase>().SetPetAnimationMode(PlayMode.InteractMode);
            introPets[i].instanceObj.GetComponent<PetBase>().InitializeStatByDefault();
        }
    }

    private void Update()
    {
        CalcPetAlgorithm(1);
        CalcPetAlgorithm(2);
        CalcPetAlgorithm(3);
        CalcPetAlgorithm(4);
        CalcPetAlgorithm(5);
        CalcPetAlgorithm(6);
    }

    private void CalcPetAlgorithm(int i)
    {
        if(introPets[i].instanceObj.GetComponent<PetBase>().inProcess) return;
        
        CmdDetail nextCmd;
        if(CmdQueueController.DequeCmd(introPets[i].cmdQueue, out nextCmd))
        {
            CmdQueueController.ExecuteCmd(introPets[i].instanceObj.GetComponent<PetBase>(), nextCmd);
        }
        else
        {
            SetNextCmd(introPets[i].cmdQueue);
        }
    }

    private void SetNextCmd(Queue<CmdDetail> cmdQueue)
    {
        int randomValue = Random.Range(0, 2);
        
        switch (randomValue)
        {
            case 0:
                Vector2 randomPointOnCircle = Random.insideUnitCircle.normalized * 2f;
                Vector3 targetPos = new Vector3(randomPointOnCircle.x, GameData.floorHeight, randomPointOnCircle.y);
                CmdQueueController.EnqueueCmd(cmdQueue, Cmd.Move, targetPos);
                break;
            case 1:
                CmdQueueController.EnqueueCmd(cmdQueue, Cmd.Brush);
                break;
            default:
                break;
        }
    }
}
