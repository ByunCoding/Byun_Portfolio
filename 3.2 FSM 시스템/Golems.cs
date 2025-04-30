using NUnit.Framework.Constraints;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum State
{
    Idle,
    Chase,
    Attack,
    Patrol,
    Gethit
}

public class Golems : MonoBehaviour
{
    [SerializeField] private GameObject _healthBarPrefab;
    private Slider _hpBar;


    [SerializeField] private Collider _leftAttackCollider;
    [SerializeField] private Collider _rightAttackCollider;

    public WeaponController _lastKilledWeapon;


    public float _maxHealth;
    public float _currentHealth;
    public float _attackDamage;
    public float _defense;
    public State _currentState;

    // Golems의 FSM 상태 객체 저장용 변수
    GolemsIdleState _idleState;
    GolemsChaseState _chaseState;
    GolemsAttackState _attackState;
    GolemsPatrolState _patrolState;
    GolemsGethitState _gethitState;

    StateMachine<Golems> _fsm;

    public float _detectingRange = 25.0f; // 감지 반경
    public float _attackRange = 5.0f;
    private float _chaseMaxRange = 17.0f;
    private float _chaseMinRange = 3.0f;
    protected bool _isPlayerInRange;

    public Transform _playerTr = null; // 감지된 Player 저장용
    public NavMeshAgent _navAgent;
    public Animator _animator;
    public string currentAnimState;

    public float _moveSpeed = 2.0f;

    private Vector3 _targetPos;
    private Vector3 _startPos;
    private float _radius = 40.0f;
    private bool _isMoving = false;

    private float _patrolDestLength = 15.0f; // 패트롤 가능 거리.
    private float _patrolRadius = 20.0f;    // 패트롤 위치계산용 반지름.
    private Vector3 _patrolCenterPos;

    private bool _isAttacking = false;
    public bool _hasBeenAttacked = false;
    public bool _isAnimPlaying = false;
    private float _nextGethitTime = 0f;

    public Vector3 nextPatrolPos;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _attackRange);

        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(transform.position, _chaseMinRange);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, _chaseMaxRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _detectingRange);
    }

    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _navAgent.enabled = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _navAgent.enabled = true;
        _navAgent.speed = _moveSpeed;
        _animator = GetComponent<Animator>();
        _startPos = transform.position;
        _patrolCenterPos = this.transform.position;

        _attackDamage = Random.Range(2f, 5.5f);
        _maxHealth = Random.Range(100f, 150f);
        _currentHealth = _maxHealth;
        _defense = Random.Range(5.0f, 10.0f);

        GameObject hpBarInstance = Instantiate(_healthBarPrefab, transform.position + Vector3.up * 4.5f, Quaternion.identity);
        hpBarInstance.transform.SetParent(this.transform, true);

        _hpBar = hpBarInstance.GetComponentInChildren<Slider>();

        if (Camera.main != null && _hpBar != null)
        {
            Vector3 targetPos = Camera.main.transform.position;
            _hpBar.transform.LookAt(targetPos);
        }

        if (_hpBar != null)
        {
            _hpBar.maxValue = _maxHealth;
            _hpBar.value = _currentHealth;
        }
        
        //_idleState = new GolemsIdleState(this);
        _chaseState = new GolemsChaseState(this);
        _attackState = new GolemsAttackState(this);
        _patrolState = new GolemsPatrolState(this);
        _gethitState = new GolemsGethitState(this);

        _fsm = new StateMachine<Golems>(this, _patrolState); // 초기 상태를 idle 상태로 설정한다.
    }

    void Update()
    {
        _fsm.DoOperateUpdate();

        if (_hpBar != null)
        {
            _hpBar.value = _currentHealth;
        }
    }

    public void ChangeIdleState() { _currentState = State.Idle; _fsm.SetState(_idleState); }
    public void ChangeChaseState() { _currentState = State.Chase; _fsm.SetState(_chaseState); }
    public void ChangeAttackState() { _currentState = State.Attack; _fsm.SetState(_attackState); }
    public void ChangePatrolState() { _currentState = State.Patrol; _fsm.SetState(_patrolState); }
    public void ChangeGethitState() { _currentState = State.Gethit; _fsm.SetState(_gethitState); }
    public void SetEndAttack() { _isAttacking = false; }
    public bool IsAttack() { return _isAttacking; }
    public void SetStartAttack() { _isAttacking = true; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _isAttacking)
        {
            if (_leftAttackCollider.enabled || _rightAttackCollider.enabled)
            {
                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.GetDamaged(_attackDamage);
                }
            }
        }
    }

    public void EnableAttackColliders()
    {
        _rightAttackCollider.enabled = true;
        _leftAttackCollider.enabled = true;
    }

    public void DisableAttackColliders()
    {
        _rightAttackCollider.enabled = false;
        _leftAttackCollider.enabled = false;
    }

    public void TakeDamaged(float damage, WeaponController weapon = null)
    {
        float takeDamage = Mathf.Max(1, damage - _defense);
        _currentHealth -= takeDamage;

        //AudioManager.Instance.PlaySwordHitSound();

        Stage currentStage = GameMain.Instance._currentStage;
        if (currentStage is Stage_6 stage_6)
        {
            stage_6.GetDamaged();
        }

        _hasBeenAttacked = true;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0.0f;
            Die();
            return;
        }

        if(Time.time >= _nextGethitTime)
        {
            _nextGethitTime = Time.time + 4.0f;
            ChangeGethitState();
        }

        if(_hpBar != null)
        {
            _hpBar.value = _currentHealth;
        }
    }

    private void Die(WeaponController selectedWeapon = null, GameObject selectedMonster = null)
    {
        _navAgent.enabled = false;
        _animator.Play("Die");

        Stage currentStage = GameMain.Instance._currentStage;
        if(currentStage is Stage_1 stage_1)
        {
            stage_1.IncreaseKilledCount();
        }
        else if (currentStage is Stage_2 stage_2)
        {
            if (selectedWeapon != null)
            {
                stage_2.EnemyOneKilled(selectedWeapon);
            }
        }
        else if (currentStage is Stage_4 stage_4)
        {
            stage_4.AllKilledCount();
        }
        else if (currentStage is Stage_6 stage_6)
        {
            stage_6.KillTime();
        }




        Destroy(this.gameObject, 3.0f);
    }

    public void GetHitAnimationEnd()
    {
        Debug.Log("GetHitAnimationEnd");
        _isAnimPlaying = false;
        _isAttacking = false;
    }

    public bool checkAttackRange()
    {
        if (_playerTr != null && Vector3.Distance(this.transform.position, _playerTr.position) <= _attackRange)
        {
            return true;
        }

        return false;
    }

    public bool checkChaseRange()
    {
        if (_playerTr != null && Vector3.Distance(this.transform.position, _playerTr.position) <= _chaseMaxRange)
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Detect 감지 반경 내에 들어온 타겟을 감지
    /// </summary>
    /// <returns></returns>
    public bool DetectingTarget()
    {
        // 감지 반경 안에 들어온 오브젝트들의 Collider를 가져오기
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, _detectingRange);

        foreach (var col in colliders)
        {
            // 감지 반경안에 들어온 오브젝트 중에 Player tag를 가지고 있는 오브젝트가 있는지 체크
            if (col.CompareTag("Player"))
            {
                _playerTr = col.transform; // 감지된 Player를 _playerTr에 저장
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// 추적 상태 호출: NavMeshAgent를 통해 플레이어를 추적하고,
    /// 플레이어와의 거리가 공격 범위 내에 들어오면 공격상태로 전환.
    /// </summary>
    public void MoveChase()
    {
        if (_playerTr != null && _navAgent.isOnNavMesh)
        {
            if (Vector3.Distance(transform.position, _playerTr.position) > _chaseMinRange)
            {
                _navAgent.SetDestination(_playerTr.position);
            }
        }
        if (_hasBeenAttacked)
        {
            _animator.SetBool("Walk", false);
            _animator.SetBool("Run", true);
            _navAgent.speed = _moveSpeed * 2.0f;
        }
        else
        {
            _animator.SetBool("Walk", true);
            _animator.SetBool("Run", false);
            _navAgent.speed = _moveSpeed;
        }
    }

    public Vector3 GetPatrolPositionOnNavMesh()
    {
        bool isOk = false;
        Vector3 retPos = _patrolCenterPos;
        int findCount = 0;

        Bounds landBoudns = MapManager.Instance.LandBounds;

        while (!isOk)
        {
            float randomX = Random.Range(landBoudns.min.x, landBoudns.max.x);
            float randomZ = Random.Range(landBoudns.min.z, landBoudns.max.z);


            Vector3 randomDirect = new Vector3(randomX, _patrolCenterPos.y, randomZ);

            if ((randomDirect - this.transform.position).magnitude <= _patrolDestLength)
            {
                continue;
            }

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirect, out hit, _patrolRadius, NavMesh.AllAreas))
            {
                retPos = hit.position;
                break;
            }

            if (findCount++ > 10)
            {
                isOk = false;
            }
        }

        return retPos;
    }

    public void PatrolMove(Vector3 movePosition)
    {
        nextPatrolPos = GetPatrolPositionOnNavMesh();
        //_navAgent.SetDestination(movePosition);
        _navAgent.SetDestination(nextPatrolPos);
        _animator.SetBool("Walk", true);
    }

    public bool IsArrived()
    {
        if(!_navAgent.isOnNavMesh)
        {
            return true;
        }

        if (!_navAgent.pathPending && _navAgent.remainingDistance <= _navAgent.stoppingDistance + 0.1f &&
            _navAgent.pathStatus != NavMeshPathStatus.PathPartial &&
            _navAgent.pathStatus != NavMeshPathStatus.PathInvalid)
        {
            return true;
        }

        return false;
    }

    public void Attack1()
    {
        _animator.SetBool("Walk", false);
        _animator.SetTrigger("Attack1");
    }

    public void Attack1End()
    {
        SetEndAttack();

        if (checkAttackRange())
        {
            _animator.SetTrigger("Attack2");
            SetStartAttack();
        }
        else
        {
            ChangeChaseState();
        }
    }

    public void Attack2End()
    {
        SetEndAttack();

        if (checkAttackRange())
        {
            _animator.SetTrigger("Attack1");
            SetStartAttack();
        }
        else
        {
            ChangeChaseState();
        }
    }
}