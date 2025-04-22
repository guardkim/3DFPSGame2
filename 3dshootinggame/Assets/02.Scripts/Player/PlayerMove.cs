using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.Android.Gradle.Manifest;
public class PlayerMove : MonoBehaviour
{
    // 목표 : wasd를 누르면 캐릭터를 이동시키고 싶다.
    // 이동속도
    public PlayerStatSO PlayerStat;

    private bool _isJumping = false;
    private bool _isJumping2 = false;
    private bool _isRunning = false;
    private bool _isRolling = false;
    private bool _isCliming = false;
    private bool _isFall = false;
    private bool _isStaminaRegen = false;

    private const float GRAVITY = -9.8f;    // 중력
    private float _yVelocity = 0f;          // 중력가속도
    private Vector3 _dir;

    private Animator _ani;
    private CharacterController _characterController;
    public Slider StaminaSlider;

    private void Awake()
    {
        _ani = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        _isCliming = (_characterController.collisionFlags & CollisionFlags.Sides) != 0;

        Rolling();
        if (_isCliming && PlayerStat.Stamina > 0.0f)
        {
            Climing();
        }
        if (_isRolling == false && _isCliming == false)
        {
            Move();
            Jump();
            Run();
            
            // 3. 방향에 따라 플레이어를 이동한다.
            _characterController.Move(_dir * PlayerStat.MoveSpeed * Time.deltaTime);
        }
    }
    private void Climing()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        _dir = new Vector3(h, v, _dir.z);
        _dir = _dir.normalized;
        // 2-1.메인 카메라를 기준으로 방향을 변환한다.
        //_dir = Camera.main.transform.TransformDirection(_dir);

        _characterController.Move(_dir * PlayerStat.MoveSpeed * Time.deltaTime);

        PlayerStat.Stamina -= PlayerStat.StaminaChangeRate * Time.deltaTime; ;
        StaminaSlider.value = PlayerStat.Stamina;
        if(PlayerStat.Stamina <= 0.0f)
        {
            _isFall = true;
            _isCliming = false;
        }
        if(_isFall)
        {
            // 4. 중력 적용
            _yVelocity += GRAVITY * Time.deltaTime;
            _dir.y = _yVelocity;
        }
        if (_characterController.isGrounded)
        {
            _isFall = false;
        }
    }
    private void Rolling()
    {
       if (Input.GetKeyDown(KeyCode.E) && !_isRolling && PlayerStat.Stamina >= 0.3f)
       {
            _isRolling = true;
            PlayerStat.Stamina -= 0.3f;
            PlayerStat.Stamina = Mathf.Clamp(PlayerStat.Stamina, 0f, 1f);
            StaminaSlider.value = PlayerStat.Stamina;
            StartCoroutine(Roll());
       }
    }
    private IEnumerator Roll()
    {
        float elapsed = 0f;
        _dir = Camera.main.transform.forward;

        while (elapsed<PlayerStat.RollDuration)
        {
            if (_characterController.isGrounded && _yVelocity < 0f)
            {
                _yVelocity = -2f;
            }

            _yVelocity += GRAVITY* Time.deltaTime;

            Vector3 move = _dir.normalized * PlayerStat.RollSpeed + Vector3.up * _yVelocity;
            _characterController.Move(move* Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;  
        }
        _isRolling = false;
    }
    private void Move()
    {
        // 1. 키보드 입력을 받는다.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 2. 입력으로부터 방향을 설정한다.
        _dir = new Vector3(h, 0, v);
        _dir = _dir.normalized;

        // 2-1.메인 카메라를 기준으로 방향을 변환한다.
        _dir = Camera.main.transform.TransformDirection(_dir);
        _ani.SetTrigger("WALK");

        // 4. 중력 적용
        _yVelocity += GRAVITY * Time.deltaTime;
        _dir.y = _yVelocity;
    }
    private void Run()
    {
        // 5. 달리기 적용
        if (Input.GetKey(KeyCode.LeftShift) && _isRunning == false && _isStaminaRegen == false)
        {
            PlayerStat.MoveSpeed = 15f;
            _isRunning = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && _isRunning == true)
        {
            PlayerStat.MoveSpeed = 10f;
            _isRunning = false;
        }

        // 6. 스태미나 감소
        StaminaChange();

    }
    private void Jump()
    {
        // 캐릭터가 땅 위에 있다면
        if (_characterController.isGrounded)
        {
            _isJumping = false;
            _isJumping2 = false;
        }

        // 3. 점프 적용
        if (Input.GetKeyDown(KeyCode.Space) && _isJumping == false)
        {
            Debug.Log("Jump");
            _yVelocity = PlayerStat.JumpPower;
            _isJumping = true;
        }
        if( Input.GetKeyDown(KeyCode.Space) && _isJumping == true && _isJumping2 == false)
        {
            Debug.Log("DoubleJump");
            _yVelocity = PlayerStat.JumpPower;
            _isJumping2 = true;
        }

    }
    private void StaminaChange()
    {
        if (_isRunning == true )
        {
            PlayerStat.Stamina -= PlayerStat.StaminaChangeRate * Time.deltaTime;
        }
        else
        {
            PlayerStat.Stamina += PlayerStat.StaminaChangeRate * 2 * Time.deltaTime;
        }
        PlayerStat.Stamina = Mathf.Clamp(PlayerStat.Stamina, 0, 1);
        StaminaSlider.value = PlayerStat.Stamina;
        if (PlayerStat.Stamina <= 0.0f)
        {
            _isRunning = false;
            PlayerStat.MoveSpeed = 10.0f;
            _isStaminaRegen = true;
        }
        else if( PlayerStat.Stamina >= 1.0f)
        {
            _isStaminaRegen = false;
        }

    }
}

