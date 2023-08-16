using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class BuySellEffectController : MonoBehaviour
{
    
    [SerializeField] private GameObject buySellEffect;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Button sellBtn;

    
    private void Awake()
    {
        buyBtn.onClick.AddListener(SpawnEffectOnBuyBtn);
        sellBtn.onClick.AddListener(SpawnEffectOnSellBtn);
    }

    private void SpawnEffectOnBuyBtn()
    {
        var transform1 = buyBtn.transform;
        GameObject effectObj = Instantiate(buySellEffect, transform1.position, transform1.rotation);
        StartCoroutine(DestroyAfter(effectObj, 2f));
    }
    
    private void SpawnEffectOnSellBtn()
    {
        var transform1 = sellBtn.transform;
        GameObject effectObj = Instantiate(buySellEffect, transform1.position, transform1.rotation);
        StartCoroutine(DestroyAfter(effectObj, 2f));
    }

    private IEnumerator DestroyAfter(Object target, float sec)
    {
        yield return new WaitForSeconds(sec);
        Destroy(target);
    }
}
