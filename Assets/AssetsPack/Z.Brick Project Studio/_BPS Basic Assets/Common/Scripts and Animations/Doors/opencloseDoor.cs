using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SojaExiles
{
    public class opencloseDoor : MonoBehaviour
    {
        public Animator openandclose;
        public bool open;
        public Transform Player;
        private float interactionDistance = 2f; // 문과 플레이어 간 상호작용 거리
        private Collider doorCollider; // 문 콜라이더

        void Start()
        {
            open = false;
            doorCollider = GetComponent<Collider>(); // 문 오브젝트의 Collider 가져오기
        }

        void Update()
        {
            if (Player)
            {
                float dist = Vector3.Distance(Player.position, transform.position);
                if (dist < interactionDistance && Input.GetKeyDown(KeyCode.E)) // E 키 입력 확인
                {
                    if (open)
                        StartCoroutine(closing());
                    else
                        StartCoroutine(opening());
                }
            }
        }

        IEnumerator opening()
        {
            print("You are opening the door");
            openandclose.Play("Opening");
            open = true;

            if (doorCollider != null)
                doorCollider.isTrigger = true; // 문을 열면 isTrigger 활성화 (충돌 제거)

            yield return new WaitForSeconds(.5f);
        }

        IEnumerator closing()
        {
            print("You are closing the door");
            openandclose.Play("Closing");
            open = false;

            yield return new WaitForSeconds(.5f);

            if (doorCollider != null)
                doorCollider.isTrigger = false; // 문을 닫으면 isTrigger 비활성화 (충돌 활성화)
        }
    }
}
