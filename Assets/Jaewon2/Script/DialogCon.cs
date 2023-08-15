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
                InteractEventManager.NotifyDialogShow("���� ���� ������ Ȯ���غ�����! \n ��ȣ�ۿ� : Pinch");
                break;
            case "toy":
                InteractEventManager.NotifyClearDialog();
                InteractEventManager.NotifyDialogShow("��� ����� ���ٸ� �峭���� �ѷ�������! \n ��ȣ�ۿ� : Pinch");
                break;
            case "food":
                InteractEventManager.NotifyClearDialog();
                InteractEventManager.NotifyDialogShow("���� ��⸦ �޷��� ���̸� �����غ�����! \n ��ȣ�ۿ� : Pinch");
                break;
        }

    }
}

