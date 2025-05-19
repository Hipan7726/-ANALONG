using System.Collections;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public static ThirdPersonCamera Instance; //�̱��� �ν��Ͻ�

    Camera _camera;
    public Transform _camAxis;// ī�޶� ȸ���� ������ �Ǵ� ��
    public Transform target; //���� ĳ����
    public float _camSpeed = 5.0f; // ī�޶� ȸ�� �ӵ�

    private float _mouseX = 0f; 
    private float _mouseY = 0f; 
    Vector3 _velocity;

    private void Awake()
    {
        //�̱��� �ʱ�ȭ
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        _camera = Camera.main;
        // ī�޶� ȸ�� �� ���� (������ �� ������Ʈ)
        _camAxis = new GameObject("CamAxis").transform;
        // ī�޶� �ش� �࿡ �ڽ����� ����
        _camera.transform.parent = _camAxis;
        //�⺻ �þư� ����
        _camera.fieldOfView = 100f;
    }

    void Update()
    {
        // ���콺 �Է� �޾� ���� (ī�޶� ȸ��)
        _mouseX += Input.GetAxis(Define.MouseX) * _camSpeed;
        _mouseY -= Input.GetAxis(Define.MouseY) * _camSpeed;

        // ���Ʒ� ī�޶� ���� ����
        _mouseY = Mathf.Clamp(_mouseY, 0f, 40f);

        // ī�޶� ���� ĳ���� �Ӹ� ���� ��ġ��Ŵ
        _camAxis.position = target.position + new Vector3(0, 1f, 0);

        // ī�޶� ���� ȸ���� ���콺�� �Է¿� �°� ����
        _camAxis.rotation = Quaternion.Euler(_mouseY, _mouseX, 0f);

        // ī�޶�� �� �������� �ణ ���ʿ� ��ġ
        _camera.transform.localPosition = new Vector3(0, 0.1f, -2.5f); 

        // �� ȸ���� �����Ƿ� ��ü ȸ���� �ʱ�ȭ
        _camera.transform.localRotation = Quaternion.identity;
    }

    // ĳ���� ��ü �� Ÿ�� �缳��
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // �и� ���� �� ī�޶� ����Ʈ ȿ�� (�Ͻ������� ����)
    public IEnumerator CameraImpactEffect()
    {
        float startFOV = 100f;
        float zoomFOV = 20f;
        float time = 0f;

        // �� �� (0.3�� ����)
        while (time < 0.3f)
        {
            _camera.fieldOfView = Mathf.Lerp(startFOV, zoomFOV, time / 0.3f);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;

        // �� �ƿ� (������� ����)
        while (time < 0.3f)
        {
            _camera.fieldOfView = Mathf.Lerp(zoomFOV, startFOV, time / 0.3f);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
