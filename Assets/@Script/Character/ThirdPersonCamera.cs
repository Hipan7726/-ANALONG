using System.Collections;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public static ThirdPersonCamera Instance; //싱글톤 인스턴스

    Camera _camera;
    public Transform _camAxis;// 카메라 회전의 기준이 되는 축
    public Transform target; //따라갈 캐릭터
    public float _camSpeed = 5.0f; // 카메라 회전 속도

    private float _mouseX = 0f; 
    private float _mouseY = 0f; 
    Vector3 _velocity;

    private void Awake()
    {
        //싱글톤 초기화
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        _camera = Camera.main;
        // 카메라 회전 축 생성 (가상의 빈 오브젝트)
        _camAxis = new GameObject("CamAxis").transform;
        // 카메라를 해당 축에 자식으로 설정
        _camera.transform.parent = _camAxis;
        //기본 시아각 설정
        _camera.fieldOfView = 100f;
    }

    void Update()
    {
        // 마우스 입력 받아 누적 (카메라 회전)
        _mouseX += Input.GetAxis(Define.MouseX) * _camSpeed;
        _mouseY -= Input.GetAxis(Define.MouseY) * _camSpeed;

        // 위아래 카메라 각도 제한
        _mouseY = Mathf.Clamp(_mouseY, 0f, 40f);

        // 카메라 축을 캐릭터 머리 위로 위치시킴
        _camAxis.position = target.position + new Vector3(0, 1f, 0);

        // 카메라 축의 회전을 마우스에 입력에 맞게 조절
        _camAxis.rotation = Quaternion.Euler(_mouseY, _mouseX, 0f);

        // 카메라는 축 기준으로 약간 뒤쪽에 위치
        _camera.transform.localPosition = new Vector3(0, 0.1f, -2.5f); 

        // 축 회전을 따르므로 자체 회전은 초기화
        _camera.transform.localRotation = Quaternion.identity;
    }

    // 캐릭터 교체 시 타겟 재설정
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // 패링 성공 시 카메라 임팩트 효과 (일시적으로 줌인)
    public IEnumerator CameraImpactEffect()
    {
        float startFOV = 100f;
        float zoomFOV = 20f;
        float time = 0f;

        // 줌 인 (0.3초 동안)
        while (time < 0.3f)
        {
            _camera.fieldOfView = Mathf.Lerp(startFOV, zoomFOV, time / 0.3f);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;

        // 줌 아웃 (원래대로 복귀)
        while (time < 0.3f)
        {
            _camera.fieldOfView = Mathf.Lerp(zoomFOV, startFOV, time / 0.3f);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
