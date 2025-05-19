using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZZZCharacterControllerLONGINUS : MonoBehaviour
{
    Transform _character;
    Transform _camTransform;
    CharacterController _controller;
    Animator _animator;

    Vector3 _velocity;

    public InCharacter InCharacter;
    public RuntimeCharacterData RuntimeData;
    private InGameCharacterManager _characterManager;

    public bool IsInvincible = false;
    public bool IsDead = false;
    public float MoveSpeed = 6f;
    public bool IsParryWindowActive = false;
    private bool _isPadding = false;
    bool canParry = true;

    public int CurrentHP => RuntimeData.CurrentHP;
    public int MaxHP => InCharacter.maxHP;
    public Sprite Portrait => InCharacter.characterImage;

    public int Attack => InCharacter.attackPower;

    void Start()
    {
        _characterManager = FindAnyObjectByType<InGameCharacterManager>();

        _character = transform.GetChild(0);
        _animator = _character.GetComponent<Animator>();
        if (_animator == null)
            _animator.applyRootMotion = false;

        _controller = gameObject.AddComponent<CharacterController>();
        _controller.center = new Vector3(0, 0.8f, 0);
        _controller.radius = 0.3f;
        _controller.height = 1.5f;
        _controller.stepOffset = 0.5f;
        _controller.slopeLimit = 45f;

        _camTransform = Camera.main.transform;
    }

    void Update()
    {
        if (IsDead) return;

        HandleAttack();

        if (!IsAttacking)
        {
            HandleMove();
        }
        else
        {
            // ���� �߿� Speed 0���� ���� (�ȱ� ����)
            Speed = 0f;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeHit(500);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("�ߴ���");
            if (MonsterController.Instance._isPadding == true)
            {
                ParrySuccess();
            }
        }
    }

    public void Initialize(RuntimeCharacterData data)
    {
        RuntimeData = data;
    }


    #region �̵�

    void HandleMove()
    {
        // ���� �߿��� �̵����� ���ϰ� �Ѵ�.
        if (IsAttacking)
        {
            Speed = 0f;  // ���� �߿��� �̵� �ӵ��� 0���� ����
            return;  // �̵� ó�� �ߴ�
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        Vector3 camForward = _camTransform.forward;
        Vector3 camRight = _camTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * inputDir.z + camRight * inputDir.x;

        if (moveDir.magnitude > 0.1f)
        {
            _controller.Move(moveDir * MoveSpeed * Time.deltaTime);
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            _character.rotation = Quaternion.Slerp(_character.rotation, targetRot, 10f * Time.deltaTime);
        }

        Speed = moveDir.sqrMagnitude;

        if (_controller.isGrounded)
            _velocity.y = -1f;
        else
            _velocity.y += Physics.gravity.y * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);

        ThirdPersonCamera.Instance._camAxis.position = transform.position + new Vector3(0, 1, 0);
        //_character.eulerAngles = new Vector3(0, _character.eulerAngles.y, 0);
    }

    #endregion

    #region ���� �� ü��

    void HandleAttack()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (!IsAttacking)
            {
                IsAttacking = true;
                ComboCount = 1;
            }
            else
            {
                IsNextCombo = true;
            }
        }
    }


    void EndInvincible()
    {
        IsInvincible = false;
        Padding = false;
    }

    public void TakeHit(int damage)
    {
        if (IsInvincible || IsDead) return;

        RuntimeData.CurrentHP -= damage;
        RuntimeData.CurrentHP = Mathf.Max(0, RuntimeData.CurrentHP);
        Debug.Log($"New HP: {RuntimeData.CurrentHP} / {RuntimeData.BaseData.maxHP}");

        CharacterSwitchUI.Instance.UpdateHealthBarOnly();

        if (RuntimeData.CurrentHP <= 0)
        {
            Die();
        }
        else
        {
            Hit = true;
        }
    }

    public void Die()
    {
        if (IsDead) return;

        IsDead = true;
        Dead = true;
        _animator.SetTrigger(Define.Dead);
    }

    public void PerformAttackHitCheck()
    {
        float attackRadius = 2.2f;
        Vector3 attackCenter = transform.position + _character.forward * 1.6f + Vector3.up * 0.8f;

        Collider[] hits = Physics.OverlapSphere(attackCenter, attackRadius);
        foreach (Collider hit in hits)
        {
            MonsterController monster = hit.GetComponent<MonsterController>();
            if (monster != null)
            {
                monster.TakeDamage(Attack);
            }
        }
    }

    public void AttackSound()
    {
        SoundManager.Instance.PlayAttack();
    }


    #endregion

    #region �и�

    // �и� ���� �� ���ͷ�Ʈ ó��
    void ParrySuccess()
    {
        Debug.Log("�и� ����! ĳ���� ��ü!");

        SoundManager.Instance.PlayParing();

        IsInvincible = true; //����
        ThirdPersonCamera.Instance.StartCoroutine(ThirdPersonCamera.Instance.CameraImpactEffect());

        Padding = true;
        // 1�� �Ŀ� ������ ó�� ����
        Invoke(nameof(FinishParrySuccess), 0.7f);
    }
    void FinishParrySuccess()
    {

        if (InGameCharacterManager.Instance != null)
        {
            CharacterSwitchUI.Instance.SwitchCharacter();
            CharacterSwitchUI.Instance.ScalePortraitEffect();
            InGameCharacterManager.Instance.SwapCharacter();
        }

        MonsterController.Instance.InterruptAttack();
        IsInvincible = false; //����

    }
    #endregion

    #region Animator �Ķ����

    public float Speed
    {
        get => _animator.GetFloat(Define.Speed);
        set => _animator.SetFloat(Define.Speed, value);
    }

    public int ComboCount
    {
        get => _animator.GetInteger(Define.ComboCount);
        set => _animator.SetInteger(Define.ComboCount, value);
    }

    public bool IsNextCombo
    {
        get => _animator.GetBool(Define.isNextCombo);
        set => _animator.SetBool(Define.isNextCombo, value);
    }

    public bool IsAttacking
    {
        get => _animator.GetBool(Define.isAttacking);
        set => _animator.SetBool(Define.isAttacking, value);
    }

    public bool Padding
    {
        get => _isPadding;
        set
        {
            _isPadding = value;
            if (value) _animator.SetTrigger(Define.Padding);
        }
    }

    public bool Hit
    {
        set => _animator.SetTrigger(Define.Hit);
    }

    public bool Dead
    {
        get => _animator.GetBool(Define.Dead);
        set => _animator.SetBool(Define.Dead, value);
    }

    #endregion
}
