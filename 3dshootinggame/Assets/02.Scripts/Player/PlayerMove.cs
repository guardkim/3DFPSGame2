using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.Android.Gradle.Manifest;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private PlayerStatSO PlayerStat;
    [SerializeField] private Slider StaminaSlider;
    [SerializeField] private float climbingSmoothTime = 0.1f; // 클라이밍 이동 스무딩 시간

    // 상태 변수
    private bool _isJumping = false;
    private bool _isJumping2 = false;
    private bool _isRunning = false;
    private bool _isRolling = false;
    private bool _isCliming = false;
    private bool _isFall = false;
    private bool _isStaminaRegen = false;

    // 이동 관련 변수
    private const float GRAVITY = -9.8f;
    private float _yVelocity = 0f;
    private Vector3 _dir;
    private Vector3 _climbVelocity; // 클라이밍 이동 속도
    private Vector3 _currentClimbVelocity; // 스무딩을 위한 현재 클라이밍 속도
    private Vector3 _moveDir; // 이동 방향 보존

    // 입력 저장 변수
    private float _horizontalInput;
    private float _verticalInput;
    private bool _jumpKeyPressed;
    private bool _runKeyPressed;
    private bool _rollKeyPressed;

    // 컴포넌트 캐싱
    private Animator _ani;
    private CharacterController _characterController;
    private Player _player;
    private void Awake()
    {
        _ani = GetComponentInChildren<Animator>();
        _characterController = GetComponent<CharacterController>();
        _player = GetComponent<Player>();
    }

    void Update()
    {
        // 키 입력 처리
        GetInputs();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        // 스태미나 소진 체크 - 가장 먼저 수행
        if (_isCliming && PlayerStat.Stamina <= 0.0f)
        {
            StopClimbing();
        }

        // 상태 업데이트
        UpdatePlayerState();

        // 움직임 처리
        if (_isRolling)
            return;

        if (_isCliming)
        {
            ProcessClimbing();
        }
        else
        {
            ProcessMovement();
        }

        // 스태미나 관리
        UpdateStamina();
    }

    #region 입력 처리
    private void GetInputs()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        
        // 점프 입력 감지
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpKeyPressed = true;
        }
        
        // 달리기 입력 감지
        _runKeyPressed = Input.GetKey(KeyCode.LeftShift);
        
        // 구르기 입력 감지
        if (Input.GetKeyDown(KeyCode.E) && !_isRolling && PlayerStat.Stamina >= 0.3f)
        {
            _rollKeyPressed = true;
        }

    }

    private void UpdateAnimations()
    {
        _ani.SetBool("IsMove", _horizontalInput != 0 || _verticalInput != 0);
        _ani.SetBool("IsRun", _runKeyPressed);
    }
    #endregion

    #region 상태 관리
    private void UpdatePlayerState()
    {
        // 벽에 닿아있는지 확인 (측면 충돌)
        bool isTouchingWall = (_characterController.collisionFlags & CollisionFlags.Sides) != 0;
        
        // 클라이밍 상태 시작 (벽에 닿았을 때)
        if (isTouchingWall && !_isCliming && !_characterController.isGrounded)
        {
            StartClimbing();
        }
        // 클라이밍 상태 종료 (벽에서 떨어졌을 때)
        else if (!isTouchingWall && _isCliming)
        {
            StopClimbing();
        }

        // 구르기 처리
        if (_rollKeyPressed)
        {
            StartRolling();
            _rollKeyPressed = false;
        }

        // 달리기 상태 처리
        UpdateRunningState();

        // 바닥 체크
        if (_characterController.isGrounded)
        {
            _isJumping = false;
            _isJumping2 = false;
            _isFall = false;
            if (_isCliming)
            {
                StopClimbing();
            }
        }
    }

    private void StartClimbing()
    {
        _isCliming = true;
        // 클라이밍 시작 시 중력 영향 제거
        _yVelocity = 0f;
        _currentClimbVelocity = Vector3.zero;
        _climbVelocity = Vector3.zero;
    }

    private void StopClimbing()
    {
        if (!_isCliming) return; // 이미 클라이밍 중이 아니면 무시

        _isCliming = false;
        _isFall = true;
        // 중력 다시 적용
        _yVelocity = 0; // 초기 낙하 속도 설정
    }

    private void UpdateRunningState()
    {
        if (_runKeyPressed && !_isRunning && !_isStaminaRegen)
        {
            PlayerStat.MoveSpeed = 15f;
            _isRunning = true;
        }
        else if (!_runKeyPressed && _isRunning)
        {
            PlayerStat.MoveSpeed = 10f;
            _isRunning = false;
        }

        // 스태미나 소진시 달리기 중단
        if (PlayerStat.Stamina <= 0.0f && _isRunning)
        {
            _isRunning = false;
            PlayerStat.MoveSpeed = 10.0f;
            _isStaminaRegen = true;
        }
        else if (PlayerStat.Stamina >= 1.0f)
        {
            _isStaminaRegen = false;
        }
    }
    #endregion

    #region 이동 처리
    private void ProcessMovement()
    {
        CalculateMovementDirection();
        ProcessJump();
        
        // 캐릭터 이동
        _characterController.Move(_dir * PlayerStat.MoveSpeed * Time.deltaTime);
    }

    private void CalculateMovementDirection()
    {
        _dir = new Vector3(_horizontalInput, 0, _verticalInput);
        _ani.SetFloat("MoveAmount", _dir.magnitude);
        _ani.SetLayerWeight(2, 1 - (_player.PlayerHP / _player._maxHP));

        if (_dir.magnitude > 0.1f)
        {
            _dir = _dir.normalized;

            // 카메라 타입에 따라 다른 이동 방향 계산
            if (CameraManager.Instance != null && CameraManager.Instance.CameraType == CameraType.ISO)
            {
                Transform camT = Camera.main.transform;
                Vector3 camForward = camT.forward;
                camForward.y = 0f;
                camForward.Normalize();
                Vector3 camRight = camT.right;
                camRight.y = 0f;
                camRight.Normalize();
                _moveDir = camForward * _verticalInput + camRight * _horizontalInput;
            }
            else
            {
                // FPS/TPS 모드에서는 카메라 기준으로 이동
                _moveDir = Camera.main.transform.TransformDirection(_dir);
                _moveDir.y = 0;
                _moveDir.Normalize();
            }
        }
        else
        {
            // 입력이 없는 경우 이전 방향 유지
            _moveDir = Vector3.zero;
        }
        
        // 최종 이동 방향 설정
        _dir = _moveDir;
        
        // 중력 적용
        _yVelocity += GRAVITY * Time.deltaTime;
        
        // 그라운드 체크로 중력 최적화
        if (_characterController.isGrounded && _yVelocity < 0f)
        {
            _yVelocity = -2f; // 지면에 붙어있게 하는 작은 중력값
        }
        
        _dir.y = _yVelocity;
    }

    private void ProcessJump()
    {
        if (!_jumpKeyPressed)
            return;

        if (!_isJumping)
        {
            _yVelocity = PlayerStat.JumpPower;
            _isJumping = true;
        }
        else if (_isJumping && !_isJumping2)
        {
            _yVelocity = PlayerStat.JumpPower;
            _isJumping2 = true;
        }
        
        _jumpKeyPressed = false;
    }

    private void ProcessClimbing()
    {
        // 스태미나를 먼저 확인
        if (PlayerStat.Stamina <= 0f)
        {
            StopClimbing();
            return;
        }
        
        // 클라이밍 중에는 중력 영향 없음
        _yVelocity = 0;
        
        // 벽에서의 이동 방향 계산
        Vector3 targetVelocity = Vector3.zero;
        
        // 카메라 기준으로 상하좌우 이동 가능
        Vector3 right = Camera.main.transform.right;
        
        // 수평면으로 투영 (y값을 0으로)
        right.y = 0;
        right.Normalize();
        
        // 수직 이동은 월드 Up 벡터 사용
        targetVelocity = (right * _horizontalInput + Vector3.up * _verticalInput).normalized;
        
        // 스무딩 적용 - 부드러운 이동을 위해 (떨림 방지)
        _climbVelocity = Vector3.SmoothDamp(_climbVelocity, targetVelocity, ref _currentClimbVelocity, climbingSmoothTime);
        
        // 실제 이동 적용 (속도는 그대로 유지)
        _characterController.Move(_climbVelocity * PlayerStat.MoveSpeed * Time.deltaTime);
        
        PlayerStat.Stamina -= PlayerStat.StaminaChangeRate * Time.deltaTime;
        PlayerStat.Stamina = Mathf.Max(0, PlayerStat.Stamina); // 0 미만으로 내려가지 않도록
        StaminaSlider.value = PlayerStat.Stamina;
    }

    private void StartRolling()
    {
        _isRolling = true;
        PlayerStat.Stamina -= 0.3f;
        PlayerStat.Stamina = Mathf.Clamp(PlayerStat.Stamina, 0f, 1f);
        StaminaSlider.value = PlayerStat.Stamina;
        StartCoroutine(Roll());
    }

    private IEnumerator Roll()
    {
        float elapsed = 0f;
        
        // 카메라 타입에 따라 구르기 방향 설정
        if (CameraManager.Instance != null && CameraManager.Instance.CameraType == CameraType.ISO)
        {
            // ISO 모드에서는 입력 방향으로 구르기
            if (_moveDir.magnitude > 0.1f)
            {
                _dir = _moveDir;
            }
            else
            {
                // 입력이 없으면 현재 플레이어가 바라보는 방향으로
                _dir = transform.forward;
            }
        }
        else
        {
            // FPS/TPS 모드에서는 카메라 방향으로 구르기
            _dir = Camera.main.transform.forward;
        }

        while (elapsed < PlayerStat.RollDuration)
        {
            if (_characterController.isGrounded && _yVelocity < 0f)
            {
                _yVelocity = -2f;
            }

            _yVelocity += GRAVITY * Time.deltaTime;

            Vector3 move = _dir.normalized * PlayerStat.RollSpeed + Vector3.up * _yVelocity;
            _characterController.Move(move * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;  
        }
        _isRolling = false;
    }
    #endregion

    #region 스태미나 관리
    private void UpdateStamina()
    {
        // 스태미나 소모 또는 회복
        if (_isRunning)
        {
            // 달리기 스태미나 소모
            PlayerStat.Stamina -= PlayerStat.StaminaChangeRate * Time.deltaTime;
        }
        else if (!_isCliming) // 클라이밍이 아닐 때만 회복 (클라이밍 스태미나 소모는 ProcessClimbing에서 처리)
        {
            PlayerStat.Stamina += PlayerStat.StaminaChangeRate * 2 * Time.deltaTime;
        }

        // 스태미나 클램프 및 UI 업데이트
        PlayerStat.Stamina = Mathf.Clamp(PlayerStat.Stamina, 0, 1);
        StaminaSlider.value = PlayerStat.Stamina;
    }
    #endregion
}

