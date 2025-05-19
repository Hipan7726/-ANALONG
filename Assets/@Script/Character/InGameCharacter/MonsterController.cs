using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public static MonsterController Instance;

    [Header("���� ������")]
    public int AttackPower = 200;
    public float DetectionRange = 10f;
    public float AttackRange = 1.5f;
    public float MoveSpeed = 2f;
    public int MaxHP = 10000;
    private int _currentHP;

    [Header("������Ʈ ��ġ")]
    private Transform _player;
    private Animator _animator;

    [Header("bool �۵� ����")]
    private bool _isAttacking = false;
    public bool _isDead = false;
    public bool _isPadding = false;

    public Collider AttackTriggerCollider; // ���� ���� �ݶ��̴�

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
            Debug.LogError("Player �±׸� ���� ������Ʈ�� ã�� �� �����ϴ�!");
        }

        _animator = GetComponent<Animator>();
        _currentHP = MaxHP;

        // �ڽ����� ���� ��ġ�� ü�¹� ã��
        _healthBarUI = GetComponentInChildren<MonsterHealthBar>();
        if (_healthBarUI != null)
        {
            _healthBarUI.SetHealth(_currentHP, MaxHP);
        }

        _playerParry1 = FindAnyObjectByType<ZZZCharacterControllerANBI>();
        _playerParry2 = FindAnyObjectByType<ZZZCharacterControllerLONGINUS>();

        // ���� �ݶ��̴��� ���� �� ��Ȱ��ȭ
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
            // ���� �ִϸ��̼� �߿��� �̵����� ����
            _animator.SetBool("isMoving", false);
            return; // ���� �߿��� �ƹ� ���۵� ���� ����
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
            _animator.SetBool("isMoving", false); // ���
        }
    }

    void MoveToPlayer()
    {
        if (_isAttacking) return; // ���� ���� ���� �̵����� ����

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

        yield return new WaitForSeconds(1.5f); // ���� �ִϸ��̼� �ð�

        // ���� �� ��Ÿ�� 2�� ���
        yield return new WaitForSeconds(5f);


        _isAttacking = false;
    }

    // �ִϸ��̼� �̺�Ʈ���� ȣ���: ������ ���۵Ǹ� ���� ������ Ȱ��ȭ�ǵ��� ����
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

    public void StartParryWindow() // �ִϸ��̼� �̺�Ʈ�� ȣ���
    {
        _isPadding = true;
        ParryEffect.Instance?.ShowParryEffect(_player.position); // �÷��̾� ��ġ ���� ����Ʈ
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

        ShowDamageText(damage); // ������ �ؽ�Ʈ ����

        if (_currentHP <= 0)
        {
            Die();
        }
    }

    void ShowDamageText(int damage)
    {
        Vector3 spawnPos = transform.position + Vector3.up * 2f; // �Ӹ� ��
        spawnPos += new Vector3(Random.Range(-0.3f, 0.3f), 0, 0); // �ణ ����

        GameObject obj = DamageTextPool.Instance.GetTextObject();
        obj.transform.position = spawnPos;

        DamageText dt = obj.GetComponent<DamageText>();
        dt.Show(damage, Color.yellow);
    }


    // ������ �ؽ�Ʈ �ִϸ��̼�
    IEnumerator AnimateDamageText(GameObject damageText)
    {
        // �ؽ�Ʈ�� ���� �ö󰡰� �ϴ� �ִϸ��̼�
        float elapsedTime = 0f;
        Vector3 startPosition = damageText.transform.localPosition;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            damageText.transform.localPosition = startPosition + Vector3.up * Mathf.Sin(elapsedTime * 3f) * 0.5f;
            yield return null;
        }

        // ���� �ð��� ���� �� �ؽ�Ʈ �����
        damageText.SetActive(false);
    }

    void Die()
    {
        _isDead = true;
        _animator.SetTrigger("die");
        Destroy(gameObject, 2f); // �ִϸ��̼� �� ����
    }

    public void InterruptAttack()
    {
        if (_isAttacking)
        {
            _isAttacking = false;
            _animator.SetTrigger("Hit"); // �ߴܿ� �ִϸ��̼� Ʈ���ų� bool �Ķ���� �ʿ�
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