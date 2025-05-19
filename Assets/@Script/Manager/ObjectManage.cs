using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectManage : MonoBehaviour
{
    [SerializeField]
    private GameObject Object; // 강조할 대상 객체
    private bool check = false; // 강조 상태를 추적하는 변수

    // 강조 효과를 토글하는 함수
    public void SetEmphasis()
    {
        if (!check)
        {
            // 강조 시작
            StartCoroutine("Emphasis", Object);
            check = true;
        }
        else
        {
            // 강조 중지
            StopCoroutine("Emphasis");
            check = false;
        }
    }

    // 강조 효과를 주는 코루틴
    private IEnumerator Emphasis(GameObject gameObject)
    {
        float increase = 0.1f; // 크기 변경의 증가값

        // 무한 루프 (크기 변경을 반복)
        while (true)
        {
            // 객체가 최소 크기 0.5f 이상이 될 때까지 크기 축소
            while (gameObject.GetComponent<Transform>().localScale.x > 0.5f)
            {
                // 크기를 축소
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x - increase
                                                                , gameObject.transform.localScale.y - increase
                                                                , gameObject.transform.localScale.z - increase);
                yield return new WaitForSeconds(0.05f); // 크기가 바뀌는 속도
            }
            yield return new WaitForSeconds(0.05f); // 중간에 멈추는 텀

            // 객체가 원래 크기(1f)로 돌아올 때까지 크기 증가
            while (gameObject.GetComponent<Transform>().localScale.x < 1f)
            {
                // 크기를 증가
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x + increase
                                                                , gameObject.transform.localScale.y + increase
                                                                , gameObject.transform.localScale.z + increase);
                yield return new WaitForSeconds(0.05f); // 크기가 바뀌는 속도
            }
            yield return new WaitForSeconds(0.05f); // 중간에 멈추는 텀
        }
    }
}
