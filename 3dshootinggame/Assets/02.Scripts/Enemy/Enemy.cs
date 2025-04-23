using System.Collections;
using System.Collections.Generic;
using TMPro;
using TreeEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        Idle    = 0,
        Trace   = 1,
        Return  = 2,
        Attack  = 3,
        Damaged = 4,
        Die     = 5,
        Patrol  = 6
    }
    public EnemyState CurrentState = EnemyState.Idle;
    public float FindDistance = 7.0f;
    public float AttackDistance = 2.0f;
    public float MoveSpeed = 3.3f;
    public int Health = 100;
    public float AttackCooltime = 2.0f;
    public float DamagedTime = 0.5f;

    private float _damagedTimer = 0.0f;
    private float _attackTimer = 0.0f;
    private GameObject _player;
    private CharacterController _characterController;
    private Vector3 _startPosition;
    private Vector3 _tempPoint1;
    private Vector3 _tempPoint2;
    private Vector3 _tempPoint3;
    private void Start()
    {
        _startPosition = transform.position;
        _player = GameObject.FindGameObjectWithTag("Player");
        _characterController = GetComponent<CharacterController>();
        _tempPoint1 = transform.position;
        _tempPoint2 = transform.position;
        _tempPoint3 = transform.position;
        _tempPoint1.x += 20.0f;
        _tempPoint2.z += 20.0f;
        _tempPoint3.z -= 20.0f;
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case EnemyState.Idle:
            {
                Idle();
                break;
            }
            case EnemyState.Trace:
            {
                Trace();
                break;
            }
            case EnemyState.Return:
            {
                Return();
                break;
            }
            case EnemyState.Attack:
            {
                Attack();
                break;
            }
            case EnemyState.Patrol:
            {
                Patrol();
                break;
            }
        }
    }
    public void Patrol()
    {
        Vector3 targetPosition = _tempPoint1;
        _characterController.Move(_tempPoint1 * MoveSpeed * Time.deltaTime);
    }
    public void TakeDamage(Damage damage)
    {
        if(CurrentState == EnemyState.Damaged || CurrentState == EnemyState.Die)
        {
            return;
        }

        Health -= damage.Value;
        if(Health <= 0)
        {
            Debug.Log($"상태 전환 : {CurrentState} -> Die");
            CurrentState = EnemyState.Die;
            StartCoroutine(Die_Coroutine());
            return;
        }
        Debug.Log($"상태 전환 : {CurrentState} -> Damaged");

        _damagedTimer = 0.0f;
        CurrentState = EnemyState.Damaged;
        StartCoroutine(Damaged_Coroutine());
    }
    private void Idle()
    {
        // 행동 : 가만히 있는다
        if(Vector3.Distance(transform.position, _player.transform.position) < FindDistance)
        {
            Debug.Log("상태 전환 : Idle -> Trace");
            CurrentState = EnemyState.Trace;
        }

        StartCoroutine(Idle_Coroutine());
    }
    private IEnumerator Idle_Coroutine()
    {
        yield return new WaitForSeconds(5f);
        if(CurrentState == EnemyState.Idle)
        {
            Debug.Log("상태 전환 : Idle -> Patrol");
            CurrentState = EnemyState.Patrol;
        }
    }
    private void Trace()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) >= FindDistance)
        {
            Debug.Log("상태 전환 : Trace -> Return");
            CurrentState = EnemyState.Return;
        }
        if (Vector3.Distance(transform.position, _player.transform.position) < AttackDistance)
        {
            Debug.Log("상태 전환 : Trace -> Attack");
            CurrentState = EnemyState.Attack;
        }

        // 행동 : 플레이어를 추적한다.
        Vector3 dir = (_player.transform.position - transform.position).normalized;
        _characterController.Move(dir * MoveSpeed * Time.deltaTime);

        
    }
    private void Return()
    {
        if (Vector3.Distance(transform.position, _startPosition) < _characterController.minMoveDistance)
        {
            Debug.Log("상태 전환 : Return -> Idle");
            transform.position = _startPosition;
            CurrentState = EnemyState.Idle;
        }
        if (Vector3.Distance(transform.position, _player.transform.position) < FindDistance)
        {
            Debug.Log("상태 전환 : Return -> Trace");
            CurrentState = EnemyState.Trace;
        }
        Vector3 dir = (_startPosition - transform.position).normalized;
        _characterController.Move(dir * MoveSpeed * Time.deltaTime);
    }
    private void Attack()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) >= AttackDistance)
        {
            Debug.Log("상태 전환 : Attack -> Trace");
            CurrentState = EnemyState.Trace;
            _attackTimer = 0.0f;
            return;
        }
        
        // 행동 : 플레이어를 추적한다.
        _attackTimer += Time.deltaTime;
        if(_attackTimer >= AttackCooltime)
        {
            Debug.Log("플레이어 공격");
            _attackTimer = 0.0f;
            
        }

    }
    private IEnumerator Damaged_Coroutine()
    {
        yield return new WaitForSeconds(DamagedTime);
        Debug.Log("상태 전환 : Damaged -> Trace");
        CurrentState = EnemyState.Trace;
    }
    private IEnumerator Die_Coroutine()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }


}
