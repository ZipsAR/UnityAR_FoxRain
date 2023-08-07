using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftMovement : MonoBehaviour
{
    private GameObject gift;
    private int coinEarnedValue;
    
    private float rotationSpeed; // Rotation angle per second
    private float risingDistance;
    [SerializeField] private float itemApproachSpeed;
    [SerializeField] private float approachThreshold;

    private void Awake()
    {
        rotationSpeed = 540f;
        risingDistance = 0.08f;
        itemApproachSpeed = 1f;
        approachThreshold = 0.02f;
    }
    
    public void setGift(GameObject g) => gift = g;

    private IEnumerator RotationCoroutine()
    {
        while (true)
        {
            gift.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
            yield return null;
        }
    }

    private IEnumerator RisingCoroutine(float risingTime)
    {
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = initialPosition + Vector3.up * risingDistance;
        
        while (elapsedTime < risingTime)
        {
            float t = elapsedTime / risingTime;
            transform.position = Vector3.Lerp(initialPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        
        StartCoroutine(MoveToPlayerChest());
    }

    private IEnumerator MoveToPlayerChest()
    {
        while (Vector3.Distance(transform.position, GameManager.Instance.player.GetChestPosition()) > approachThreshold)
        {
            Vector3 chestPos = GameManager.Instance.player.GetChestPosition();
            
            Vector3 direction = (chestPos - transform.position).normalized;
            transform.position += direction * (itemApproachSpeed * Time.deltaTime);

            yield return null;
        }

        AddToInventory();
    }

    private void AddToInventory()
    {
        // add to inventory
        FileIOSystem.Instance.Load(FileIOSystem.Instance.invendatabase, FileIOSystem.InvenFilename);
        int existMoney = FileIOSystem.Instance.invendatabase.money;
        int moneyAfterEarned = existMoney + coinEarnedValue;

        FileIOSystem.Instance.invendatabase.money = moneyAfterEarned;
        
        FileIOSystem.Instance.Save(FileIOSystem.Instance.invendatabase, FileIOSystem.InvenFilename);
        
        Destroy(gameObject);
    }

    public void StartRotating()
    {
        StartCoroutine(RotationCoroutine());
    }

    public void StartRising(float f)
    {
        StartCoroutine(RisingCoroutine(f));
    }

    public void SetCoinEarnedValue(int value) => coinEarnedValue = value;
}
