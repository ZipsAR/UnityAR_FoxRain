using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogCon : MonoBehaviour
{
    GameObject current;
    public  void GetPanel()
    {
        for (int i = 0; i < 3; i++)
        {
            if (StoreManager.Instance.panelManager.Panels[i].activeSelf)
            {
                current = StoreManager.Instance.panelManager.Panels[i];
            }
        }
        switch (current.tag)
        {
            case "furniture":
                InteractEventManager.NotifyClearDialog();
                InteractEventManager.NotifyDialogShow("펫을 위한 가구를 확인해보세요! \n 상호작용 : Pinch");
                break;
            case "toy":
                InteractEventManager.NotifyClearDialog();
                InteractEventManager.NotifyDialogShow("펫과 놀아줄 색다른 장난감을 둘러보세요! \n 상호작용 : Pinch");
                break;
            case "food":
                InteractEventManager.NotifyClearDialog();
                InteractEventManager.NotifyDialogShow("펫의 허기를 달래줄 먹이를 구매해보세요! \n 상호작용 : Pinch");
                break;
        }

    }
}

