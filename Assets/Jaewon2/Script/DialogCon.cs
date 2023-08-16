using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogCon : MonoBehaviour
{
    GameObject current;
    private int furniture, toy, food = 0;
    private void Start()
    {
        GameManager.Instance.storeGet++;
        if (GameManager.Instance.storeGet == 1)
        {
            InteractEventManager.NotifyClearDialog();
            InteractEventManager.NotifyDialogShow("���� ���� ������ Ȯ���غ�����! \n ��ȣ�ۿ� : Pinch");
            furniture++;
        }
    }
    public  void GetPanel()
    {
        InteractEventManager.NotifyClearDialog();
        if (GameManager.Instance.storeGet == 1)
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
                    if (furniture == 0)
                    {
                        InteractEventManager.NotifyClearDialog();
                        InteractEventManager.NotifyDialogShow("���� ���� ������ Ȯ���غ�����! \n ��ȣ�ۿ� : Pinch");
                        furniture++;
                    }
                    break;
                case "toy":
                    if (toy == 0)
                    {
                        InteractEventManager.NotifyClearDialog();
                        InteractEventManager.NotifyDialogShow("��� ����� ���ٸ� �峭���� �ѷ�������! \n ��ȣ�ۿ� : Pinch");
                        toy++;
                    }
                    break;
                case "food":
                    if (food == 0)
                    {
                        InteractEventManager.NotifyClearDialog();
                        InteractEventManager.NotifyDialogShow("���� ��⸦ �޷��� ���̸� �����غ�����! \n ��ȣ�ۿ� : Pinch");
                        food++;
                    }
                    break;
            }

        }
    }
}

