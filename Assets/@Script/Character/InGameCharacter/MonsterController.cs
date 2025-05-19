using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public static MonsterController Instance;

    [Header("몬스터 데이터")]
    public int AttackPower = 200;
    public float DetectionRange = 10f;
    public float AttackRange = 1.5f;
    public float MoveSpeed = 2f;
    public int MaxHP = 10000;
    private int _currentHP;

    [Header("오브젝트 위치")]
    private Transform _player;
    private Animator _animator;

    [Header("bool 작동 여부")]
    private bool _isAttacking = false;
    public bool _isDead = false;
    public bool _isPadding = false;

    public Collider AttackTriggerCollider; // 공격 범위 콜라이더

    private ZZZCharacterControllerANBI _playerParry1;
    private ZZZCharacterControllerLONGINUS _playerParry2;
    private MonsterHealthBar _healthBarUI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }

        _animator = GetComponent<Animator>();
        _currentHP = MaxHP;

        // 자식으로 직접 배치한 체력바 찾기
        _healthBarUI = GetComponentInChildren<MonsterHealthBar>();
        if (_healthBarUI != null)
        {
            _healthBarUI.SetHealth(_currentHP, MaxHP);
        }

        _playerParry1 = FindAnyObjectByType<ZZZCharacterControllerANBI>();
        _playerParry2 = FindAnyObjectByType<ZZZCharacterControllerLONGINUS>();

        // 공격 콜라이더는 시작 시 비활성화
        if (AttackTriggerCollider != null)
        {
            AttackTriggerCollider.enabled = false;
        }

    }

    void Update()
    {
        if (_isDead || _player == null) return;

        float distance = Vector3.Distance(transform.position, _player.position);

        if (_isAttacking)
        {
            // 공격 애니메이션 중에는 이동하지 않음
            _animator.SetBool("isMoving", false);
            return; // 공격 중에는 아무 동작도 하지 않음
        }

        if (distance <= AttackRange)
        {
            StartCoroutine(Attack());
        }
        else if (distance <= DetectionRange)
        {
            MoveToPlayer();
        }
        else
        {
            _animator.SetBool("isMoving", false); // 대기
        }
    }

    void MoveToPlayer()
    {
        if (_isAttacking) return; // 공격 중일 때는 이동하지 않음

        _animator.SetBool("isMoving", true);
        Vector3 dir = (_player.position - transform.position).normalized;
        transform.position += dir * MoveSpeed * Time.deltaTime;
        transform.LookAt(_player);
    }

    IEnumerator Attack()
    {
        _isAttacking = true;
        _animator.SetTrigger("attack");
        _animator.SetBool("isMoving", false);

        yield return new WaitForSeconds(1.5f); // 공격 애니메이션 시간

        // 공격 후 쿨타임 2초 대기
        yield return new WaitForSeconds(5f);


        _isAttacking = false;
    }

    // 애니메이션 이벤트에서 호출됨: 공격이 시작되면 공격 범위가 활성화되도록 설정
    public void StartAttackCollider()
    {
        if (AttackTriggerCollider != null)
        {
            AttackTriggerCollider.enabled = true;
        }
    }

    public void EndAttackCollider()
    {
        if (AttackTriggerCollider != null)
        {
            AttackTriggerCollider.enabled = false;
        }
    }

    public void StartParryWindow() // 애니메이션 이벤트로 호출됨
    {
        _isPadding = true;
        ParryEffect.Instance?.ShowParryEffect(_player.position); // 플레이어 위치 기준 이펙트
    }

    public void EndParryWindow()
    {
        _isPadding = false;
        ParryEffect.Instance?.DestroyEffect();
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHP -= damage;
        if (_healthBarUI != null) _healthBarUI.SetHealth(_currentHP, MaxHP);

        ShowDamageText(damage); // 데미지 텍스트 띄우기

        if (_currentHP <= 0)
        {
            Die();
        }
    }

    void ShowDamageText(int damage)
    {
        Vector3 spawnPos = transform.position + Vector3.up * 2f; // 머리 위
        spawnPos += new Vector3(Random.Range(-0.3f, 0.3f), 0, 0); // 약간 랜덤

        GameObject obj = DamageTextPool.Instance.GetTextObject();
        obj.transform.position = spawnPos;

        DamageText dt = obj.GetComponent<DamageText>();
        dt.Show(damage, Color.yellow);
    }


    // 데미지 텍스트 애니메이션
    IEnumerator AnimateDamageText(GameObject damageText)
    {
        // 텍스트가 위로 올라가게 하는 애니메이션
        float elapsedTime = 0f;
        Vector3 startPosition = damageText.transform.localPosition;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            damageText.transform.localPosition = startPosition + Vector3.up * Mathf.Sin(elapsedTime * 3f) * 0.5f;
            yield return null;
        }

        // 일정 시간이 지난 후 텍스트 숨기기
        damageText.SetActive(false);
    }

    void Die()
    {
        _isDead = true;
        _animator.SetTrigger("die");
        Destroy(gameObject, 2f); // 애니메이션 후 제거
    }

    public void InterruptAttack()
    {
        if (_isAttacking)
        {
            _isAttacking = false;
            _animator.SetTrigger("Hit"); // 중단용 애니메이션 트리거나 bool 파라미터 필요
            StartCoroutine(ResetAttack());
        }
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(2f);
    }

    public void UpdatePlayerTarget(Transform newPlayer)
    {
        _player = newPlayer;
    }
}