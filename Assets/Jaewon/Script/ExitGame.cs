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
        text.text = "현재 시간 = " + time;
        if(time == 30.0f)
        {
            Application.Quit();
        }
    }
    public void buttondown()
    {
        // 레이를 발사할 원점과 방향 설정
        Vector3 rayOrigin = arcamera.transform.position; // 현재 객체의 위치를 원점으로 설정
        Vector3 rayDirection = arcamera.transform.forward; // 현재 객체의 전방 방향으로 설정 (즉, 객체가 바라보는 방향)

        // Raycast를 이용하여 충돌 감지
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit))
        {
            // 충돌 지점의 좌표를 가져와서 처리
            Vector3 hitPoint = hit.point;
            Debug.Log("충돌 지점 좌표: " + hitPoint);
        }
    }
}
