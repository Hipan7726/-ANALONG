using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;  // 싱글톤 패턴

    public Transform _character;
    public Animator _animator;
    CharacterController _controller;

    public Camera _camera;
    public Transform _camAxis;
    float _camSpeed = 8f;
    float _mouseX = 0f;
    float _mouseY = 0f;
    float _wheel = -2;
    float _move = 3f;
    Vector3 _velocity;


    public float Speed
    {
        get { return _animator.GetFloat(Define.Speed); }
        set { _animator.SetFloat(Define.Speed, value); }
    }

    public bool Run
    {
        get { return _animator.GetBool(Define.Run); }
        set { _animator.SetBool(Define.Run, value); }
    }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _character = transform.GetChild(0);
        _animator = _character.GetComponent<Animator>();
        _animator.applyRootMotion = false;

        _controller = gameObject.AddComponent<CharacterController>();
        _controller.center = new Vector3(0, 0.8f, 0);
        _controller.radius = 0.3f;
        _controller.height = 1.5f;
        _controller.stepOffset = 0.5f;       // 계단 높이 설정
        _controller.slopeLimit = 45f;        // 오를 수 있는 경사

        _camera = Camera.main;
        _camAxis = new GameObject("CamAxis").transform;
        _camera.transform.SetParent(_camAxis);
        _camera.transform.localPosition = new Vector3(0, 0, _wheel);

        GameManager.Instance.LoadGame();  // 초기화 완료 후 LoadGame 호출
    }

    void Update()
    {
        if ((DialogueManager.Instance != null && DialogueManager.Instance.IsInDialogue) || !GameManager.Instance.SetPlayer)
        {
            _animator.SetFloat(Define.Speed, 0);
            return;
        }
        Zoom();
        CameraMove();
        Move();
        CurrentQuest();
        CameraCollision();
        Shop();
        CharacterChoice();

    }

    void CharacterChoice()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SoundManager.Instance.PlayButton();

            GameManager.Instance.SaveGame();
            StartCoroutine(Character());

        }
    }

    IEnumerator Character()
    {
        yield return FadeManager.Instance.FadeOut();
        SceneManager.LoadScene("CharacterScene");
    }

    void Shop()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SoundManager.Instance.PlayButton();

            GameManager.Instance.SaveGame();

            StartCoroutine(InShop());
        }

    }
    IEnumerator InShop()
    {
        yield return FadeManager.Instance.FadeOut();
        SceneManager.LoadScene("DrawScrene");
    }


    void CurrentQuest()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            // 퀘스트 위치 가져오기
            Quest currentQuest = QuestManager.Instance.GetCurrentQuest();

            if (currentQuest != null)
            {
                Vector3 targetPos = currentQuest.targetPosition.ToVector3();
                NavigationManager.Instance.CreatePath(transform.position, targetPos);
            }
        }
    }

    void Zoom()
    {
        _wheel += Input.GetAxis(Define.MouseScroll) * 10;
        _wheel = Mathf.Clamp(_wheel, -3, -1);
        _camera.transform.localPosition = new Vector3(0, 0, _wheel);
    }

    void Move()
    {
        float h = Input.GetAxis(Define.Horizontal);
        float v = Input.GetAxis(Define.Vertical);
        Run = false;


        Vector3 move = new Vector3(h, 0, v);

        if (move.magnitude > 0.1f)
        {
            Quaternion camRot = Quaternion.Euler(0, _mouseX * _camSpeed, 0);
            Vector3 dir = camRot * move;

            _controller.Move(dir.normalized * _move * Time.deltaTime);

            _character.transform.localRotation = Quaternion.Slerp(
                _character.transform.localRotation,
                Quaternion.LookRotation(dir),
                5 * Time.deltaTime
            );
        }
        if (Input.GetMouseButton(1) && move.magnitude > 0.1f)
        {
            Run = true;
            _move = 5f;
        }
        else
        {
            Run = false;
            Speed = move.sqrMagnitude;
            //_move = 3f;
        }
        if (move.magnitude < 0.1f)
        {
            _move = 3f;
        }

        // 중력 적용
        if (_controller.isGrounded)
        {
            _velocity.y = -1f;
        }
        else
        {
            _velocity.y += Physics.gravity.y * Time.deltaTime;
        }
        _controller.Move(_velocity * Time.deltaTime);

        _camAxis.position = transform.position + new Vector3(0, 1, 0);
        _character.eulerAngles = new Vector3(0, _character.eulerAngles.y, 0);
    }

    void CameraMove()
    {
        _mouseX += Input.GetAxis(Define.MouseX);
        _mouseY -= Input.GetAxis(Define.MouseY);
        _mouseY = Mathf.Clamp(_mouseY, 0, 10);

        _camAxis.rotation = Quaternion.Euler(_mouseY * _camSpeed, _mouseX * _camSpeed, 0);
    }

    void CameraCollision()
    {
        Vector3 desiredCameraPos = _camAxis.position + _camAxis.forward * _wheel; // 원하는 위치
        Vector3 direction = desiredCameraPos - _camAxis.position;
        float distance = Mathf.Abs(_wheel);

        RaycastHit hit;
        if (Physics.SphereCast(_camAxis.position, 0.2f, direction.normalized, out hit, distance, LayerMask.GetMask("Default")))
        {
            // 벽에 부딪히면 부딪히기 직전까지 카메라 위치 조정
            float hitDistance = hit.distance - 0.1f;
            hitDistance = Mathf.Clamp(hitDistance, 0.5f, distance); // 너무 가까이 붙지 않게 제한
            _camera.transform.localPosition = new Vector3(0, 0, -hitDistance);
        }
        else
        {
            // 아무것도 안 막으면 원래 거리로
            _camera.transform.localPosition = new Vector3(0, 0, _wheel);
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public Vector3 GetPlayerRotation()
    {
        return _character.eulerAngles;
    }

    public Vector3 GetCameraPosition()
    {
        return _camera.transform.localPosition;
    }

    public Vector3 GetCameraRotation()
    {
        return _camAxis.rotation.eulerAngles;
    }
}