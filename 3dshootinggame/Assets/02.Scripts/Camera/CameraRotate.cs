using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    // 카메라 회전 스크립트
    // 각 카메라 위치는 Player의 자식 오브젝트로 존재
    
    public float RotationSpeed = 90f;
    public Transform PlayerTransform; // 플레이어 Transform
    public GameObject PlayerWeapon; // 플레이어 무기
    
    // 카메라 각도는 0도에서부터 시작한다고 기준을 세운다.
    private float _rotationX = 0;
    private float _rotationY = 0;
    
    // ISO 카메라 모드용 변수
    private Vector3 _targetPosition;
    private Vector3 _shootDirection; // 발사 방향 벡터
    private Ray _ray;
    private RaycastHit _hit;
    private LayerMask _groundLayer;
    
    private CameraFollow _cameraFollow;
    private PlayerFire _playerFire;
    private CameraType _lastCameraType;

    private void Start()
    {
        _groundLayer = LayerMask.GetMask("Ground"); // "Ground" 레이어 설정 필요
        
        // CameraFollow 참조 가져오기
        _cameraFollow = GetComponent<CameraFollow>();
        if (_cameraFollow == null)
        {
            _cameraFollow = gameObject.AddComponent<CameraFollow>();
        }
        
        // 플레이어 참조 가져오기
        if (PlayerTransform == null && _cameraFollow != null)
        {
            PlayerTransform = _cameraFollow.PlayerTransform;
        }
        
        if (PlayerTransform == null)
        {
            PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        // 무기 찾기
        if (PlayerWeapon == null && PlayerTransform != null)
        {
            // 플레이어의 자식 중 무기 찾기 (태그나 이름에 따라 조정 필요)
            Transform weaponTransform = PlayerTransform.Find("Weapon");
            if (weaponTransform != null)
            {
                PlayerWeapon = weaponTransform.gameObject;
            }
        }
        
        // PlayerFire 컴포넌트 찾기
        if (PlayerTransform != null)
        {
            _playerFire = PlayerTransform.GetComponent<PlayerFire>();
        }
        
        // FPS 모드 초기 회전값 설정
        if (PlayerTransform != null && _cameraFollow != null && _cameraFollow.FPSTarget != null)
        {
            Vector3 angles = _cameraFollow.FPSTarget.eulerAngles;
            _rotationX = angles.y;
            _rotationY = angles.x;
        }
        
        _lastCameraType = CameraManager.Instance.CameraType;
    }

    void Update()
    {
        if (PlayerTransform == null)
            return;
        
        // 카메라 모드가 변경되었는지 확인
        if (_lastCameraType != CameraManager.Instance.CameraType)
        {
            _lastCameraType = CameraManager.Instance.CameraType;
        }
            
        switch (CameraManager.Instance.CameraType)
        {
            case CameraType.FPS:
                FPSCameraRotate();
                break;
            case CameraType.TPS:
                TPSCameraRotate();
                break;
            case CameraType.ISO:
                ISOCameraRotate();
                break;
            default:
                break;
        }
    }
    
    void TPSCameraRotate()
    {
        // FPS와 유사하지만 플레이어로부터 일정 거리 유지
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _rotationX += mouseX * RotationSpeed * Time.deltaTime;
        _rotationY += -mouseY * RotationSpeed * Time.deltaTime;
        _rotationY = Mathf.Clamp(_rotationY, -45f, 60f);

        // 플레이어 캐릭터는 Y축만 회전 (수평 회전만)
        PlayerTransform.rotation = Quaternion.Euler(0, _rotationX, 0);
        
        // TPS 카메라 타겟 회전 설정 (플레이어 자식)
        if (_cameraFollow != null && _cameraFollow.TPSTarget != null)
        {
            _cameraFollow.TPSTarget.localRotation = Quaternion.Euler(_rotationY, 0, 0);
        }
    }
    
    void ISOCameraRotate()
    {
        // ISO 모드에서는 카메라는 고정된 위치와 방향 유지, 플레이어만 마우스 방향 바라봄
        // 마우스 위치로부터 레이 생성
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(_ray, out _hit, 100f, _groundLayer))
        {

            _targetPosition = _hit.point;
            
            // 플레이어가 마우스 포인터를 향해 회전 (XZ 평면에서만)
            Vector3 targetDirection = _targetPosition - PlayerTransform.position;
            targetDirection.y = 0; // Y축은 무시하고 XZ 평면에서만 회전
            
            if (targetDirection.magnitude > 0.1f) // 방향 벡터가 충분히 유효할 때만
            {
                // 플레이어 회전 - 타겟 방향으로 즉시 회전
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                
                // 부드러운 회전 적용 (즉시 회전이 아닌 보간)
                PlayerTransform.rotation = Quaternion.Slerp(
                    PlayerTransform.rotation, 
                    targetRotation, 
                    RotationSpeed * 0.5f * Time.deltaTime
                );
            }
            
            // 발사 방향 계산 - 플레이어에서 타겟까지의 방향
            _shootDirection = _targetPosition - PlayerTransform.position;
            
            // 발사 방향의 높이 조정 (너무 높거나 낮게 쏘지 않도록)
            _shootDirection.Normalize();
            
            // y값을 일정 범위로 제한 (-0.2~0.2 정도가 적당)
            float yLimit = Mathf.Clamp(_shootDirection.y, -0.2f, 0.2f);
            _shootDirection.y = yLimit;
            
            // 다시 정규화하여 단위 벡터로 만듦
            _shootDirection.Normalize();
            
            // 무기도 같은 방향을 바라보게 함
            if (PlayerWeapon != null)
            {
                // 무기는 정확한 발사 방향을 바라보도록 설정
                PlayerWeapon.transform.rotation = Quaternion.LookRotation(_shootDirection);
            }
        }
        
        // 발사 방향 시각화 (디버그용)
        if (_shootDirection != Vector3.zero && PlayerTransform != null)
        {
            Debug.DrawRay(PlayerTransform.position, _shootDirection * 10f, Color.red);
        }
        
        Debug.Log($"PlayerTransform : {PlayerTransform.gameObject.name}, PlayerFire : {_playerFire}");
        // PlayerFire에 발사 방향 전달
        if (_playerFire != null)
        {
            Debug.Log($"ShootDirection : {_shootDirection}");
            _playerFire.SetISOShootDirection(_shootDirection);
        }
    }
    
    void FPSCameraRotate()
    {
        // 1. 마우스 입력을 받는다. (마우스의 커서의 움직임 방향)
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 2. 마우스 입력으로부터 회전시킬 방향을 만든다.
        // 회전한 양만큼 누적시켜나간다
        _rotationX += mouseX * RotationSpeed * Time.deltaTime;
        _rotationY += -mouseY * RotationSpeed * Time.deltaTime;
        _rotationY = Mathf.Clamp(_rotationY, -45f, 60f);

        // 플레이어 캐릭터 회전 (Y축만)
        PlayerTransform.rotation = Quaternion.Euler(0, _rotationX, 0);
        
        // FPS 카메라 타겟 회전 설정 (플레이어 자식)
        if (_cameraFollow != null && _cameraFollow.FPSTarget != null)
        {
            _cameraFollow.FPSTarget.localRotation = Quaternion.Euler(_rotationY, 0, 0);
        }
    }
}
