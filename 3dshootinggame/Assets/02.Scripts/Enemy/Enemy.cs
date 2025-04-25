using System.Collections;
using System.Collections.Generic;
using TMPro;
using TreeEditor;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyState
{
    Idle = 0,
    Trace = 1,
    Return = 2,
    Attack = 3,
    Damaged = 4,
    Die = 5,
    Patrol = 6
}
public class Enemy : MonoBehaviour, IDamageable
{
    public EnemyState CurrentState = EnemyState.Idle;
    public float FindDistance = 7.0f;
    public float AttackDistance = 2.0f;
    public float MoveSpeed = 3.3f;
    public int Health = 30;
    public int MaxHealth = 30;
    public float AttackCooltime = 2.0f;
    public float DamagedTime = 0.1f;
    public EEnemyType EnemyType = EEnemyType.Patrol;
    public UI_EnemyHPBar HPBar;
    public float Damage = 10.0f;

    private float _damagedTimer = 0.0f;
    private float _attackTimer = 0.0f;
    private GameObject _player;
    private CharacterController _characterController;
    private Vector3 _startPosition;
    public List<Vector3> _patrolPoints;
    private float _patrolRadius = 10.0f;
    private Vector3 _patrolPosition;
    private bool _isPatrol = false;
    private bool _isPatrolTurn = false;
    private NavMeshAgent _agent;

    private void Start()
    {
        MaxHealth = Health;
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = MoveSpeed;

        _startPosition = transform.position;
        _player = GameObject.FindGameObjectWithTag("Player");
        _characterController = GetComponent<CharacterController>();
        _patrolPoints = new List<Vector3>();
        CreatePatrolPoint();
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
    private void TraceType()
    {
        if (CurrentState != EnemyState.Trace) return;
        _agent.SetDestination(_player.transform.position);
    }
    private void CreatePatrolPoint()
    {
        _patrolPoints.Clear();
        for (int i = 0; i < 3; i++)
        {
            Vector2 randomPosition = Random.insideUnitSphere * _patrolRadius;
            Vector3 targetPosition = new Vector3(randomPosition.x, 1.0f, randomPosition.y);
            _patrolPoints.Add(targetPosition);
        }
    }
    private void MoveToTarget()
    {
        if (_isPatrol == false)
        {
            int randomIndex = Random.Range(0, _patrolPoints.Count);
            _patrolPosition = _patrolPoints[randomIndex];
            _isPatrol = true;
        }
        else
        {

            Vector3 dir = (_patrolPosition - transform.position).normalized;
            _characterController.Move(dir * MoveSpeed * Time.deltaTime);

            // 돌아오는 상태인 경우
            if (_isPatrolTurn == true)
            {

                if (Vector3.Distance(transform.position, _startPosition) < 0.1f)
                {
                    _isPatrol = false;
                    _isPatrolTurn = false;
                }
            }
            // 순찰 위치로 이동 중인 경우
            else
            {
                Debug.Log(Vector3.Distance(transform.position, _patrolPosition));

                if (Vector3.Distance(transform.position, _patrolPosition) < 0.1f)
                {
                    _patrolPosition = _startPosition;
                    _isPatrolTurn = true;
                }
            }
        }
    }
    public void Patrol()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) < FindDistance ||
            EnemyType == EEnemyType.Trace)
        {
            Debug.Log("상태 전환 : Patrol -> Trace");
            CurrentState = EnemyState.Trace;
        }
        MoveToTarget();
    }
    public void TakeDamage(Damage damage)
    {
        if(CurrentState == EnemyState.Damaged || CurrentState == EnemyState.Die)
        {
            return;
        }

        Health -= damage.Value;
        
        HPBar.RefreshHPBar((float)Health / (float)MaxHealth);
        Debug.Log($"EnemyHealth : {(float)Health / (float)MaxHealth}");
        Knockback(10.0f, damage.From);
        if (Health <= 0)
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
    private void Knockback(float force, GameObject from)
    {
        Vector3 dir = (transform.position - from.transform.position).normalized;
        dir.y = 0.0f;
        _characterController.Move(dir * force);
    }
    private void Idle()
    {
        // 행동 : 가만히 있는다
        if(Vector3.Distance(transform.position, _player.transform.position) < FindDistance ||
            EnemyType == EEnemyType.Trace)
        {
            Debug.Log("상태 전환 : Idle -> Trace");
            CurrentState = EnemyState.Trace;
        }

        StartCoroutine(Idle_Coroutine());
    }
    private IEnumerator Idle_Coroutine()
    {
        yield return new WaitForSeconds(3f);
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
            if (CurrentState == EnemyState.Trace) return;
            CurrentState = EnemyState.Return;
        }
        if (Vector3.Distance(transform.position, _player.transform.position) < AttackDistance)
        {
            Debug.Log("상태 전환 : Trace -> Attack");
            CurrentState = EnemyState.Attack;
        }

        // 행동 : 플레이어를 추적한다.
        //Vector3 dir = (_player.transform.position - transform.position).normalized;
        //_characterController.Move(dir * MoveSpeed * Time.deltaTime);
        _agent.SetDestination(_player.transform.position);

        
    }
    private void Return()
    {
        if (Vector3.Distance(transform.position, _startPosition) < 0.1f)
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
        //Vector3 dir = (_startPosition - transform.position).normalized;
        //_characterController.Move(dir * MoveSpeed * Time.deltaTime);
        _agent.SetDestination(_startPosition);
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
        
        _attackTimer += Time.deltaTime;
        if(_attackTimer >= AttackCooltime)
        {
            Debug.Log("플레이어 공격");
            Player player = _player.GetComponent<Player>();
            Damage damage = default;
            damage.Value = (int)Damage;
            player.TakeDamage(damage);
            _attackTimer = 0.0f;
        }
    }
    private IEnumerator Damaged_Coroutine()
    {
        _agent.isStopped = false;
        _agent.ResetPath();
        yield return new WaitForSeconds(DamagedTime);
        Debug.Log("상태 전환 : Damaged -> Trace");
        CurrentState = EnemyState.Trace;
    }
    private IEnumerator Die_Coroutine()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("상태 전환 : Die");
        gameObject.SetActive(false);
    }


}
