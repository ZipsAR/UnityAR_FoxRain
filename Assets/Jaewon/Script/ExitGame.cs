using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ExitGame : MonoBehaviour
{
    public float time = 0;
    public Text text;
    public GameObject arcamera;
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        text.text = "���� �ð� = " + time;
        if(time == 30.0f)
        {
            Application.Quit();
        }
    }
    public void buttondown()
    {
        // ���̸� �߻��� ������ ���� ����
        Vector3 rayOrigin = arcamera.transform.position; // ���� ��ü�� ��ġ�� �������� ����
        Vector3 rayDirection = arcamera.transform.forward; // ���� ��ü�� ���� �������� ���� (��, ��ü�� �ٶ󺸴� ����)

        // Raycast�� �̿��Ͽ� �浹 ����
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit))
        {
            // �浹 ������ ��ǥ�� �����ͼ� ó��
            Vector3 hitPoint = hit.point;
            Debug.Log("�浹 ���� ��ǥ: " + hitPoint);
        }
    }
}
