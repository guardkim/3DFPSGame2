using UnityEngine;

public enum CameraType
{
    FPS,
    TPS,
    ISO
}
public class CameraManager : Singleton<CameraManager>
{
    public CameraType CameraType = CameraType.FPS;
    public bool IsTypeChanged = false;
    
    // ISO 모드 카메라 설정
    [Header("ISO 카메라 설정")]
    public float ISOCameraHeight = 15f;     // ISO 카메라 높이
    public float ISOCameraDistance = -15f;  // ISO 카메라 거리 (- 값은 플레이어 뒤)
    public float ISOCameraAngle = 45f;      // ISO 카메라 각도 (X 축 회전)
    public GameObject GunPrefab;
    public GameObject SwordPrefab;
    public Transform FPSGunPosition;
    public Transform TPSISOGunPosition;
    
    private CameraType _prevCameraType;
    private CameraFollow _cameraFollow;
    private CameraRotate _cameraRotate;
    private Transform _playerTransform;
    private PlayerRotate _playerRotate;
    private GameObject _isoCamera; // ISO 카메라 타겟 (월드 공간에 고정)

    void Start()
    {
        _prevCameraType = CameraType;
        
        // 메인 카메라 컴포넌트 참조 가져오기
        _cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (_cameraFollow == null)
        {
            _cameraFollow = Camera.main.gameObject.AddComponent<CameraFollow>();
        }
        
        _cameraRotate = Camera.main.GetComponent<CameraRotate>();
        if (_cameraRotate == null)
        {
            _cameraRotate = Camera.main.gameObject.AddComponent<CameraRotate>();
        }
        
        // 플레이어 찾기
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (_playerTransform != null)
        {
            // 플레이어 참조 설정
            _cameraFollow.PlayerTransform = _playerTransform;
            _cameraRotate.PlayerTransform = _playerTransform;
            
            // PlayerRotate 컴포넌트 찾기
            _playerRotate = _playerTransform.GetComponent<PlayerRotate>();
            
            // 플레이어에 카메라 타겟 오브젝트가 없으면 생성
            if (_cameraFollow.FPSTarget == null)
            {
                CreateCameraTarget("FPSCameraTarget", Vector3.up * 1.8f, new Vector3(0, 0, 0));
            }
            
            if (_cameraFollow.TPSTarget == null)
            {
                CreateCameraTarget("TPSCameraTarget", new Vector3(0, 2.0f, -4.0f), new Vector3(15, 0, 0));
            }
            
            // ISO 카메라는 별도로 생성 - 월드 공간에 고정
            if (_cameraFollow.ISOTarget == null)
            {
                CreateISOCamera();
            }
        }
        else
        {
            Debug.LogError("플레이어를 찾을 수 없습니다. 'Player' 태그를 가진 오브젝트가 존재하는지 확인하세요.");
        }
        
        // 마우스 커서 상태 업데이트
        UpdateCursorState(CameraType);
        
        // 초기화
        IsTypeChanged = true;
        _cameraFollow.Init();
    }

    void Update()
    {
        // 카메라 타입 변경 감지
        bool typeChanged = false;
        CameraType newType = CameraType;
        
        if(Input.GetKeyDown(KeyCode.Alpha8))
        { 
            newType = CameraType.FPS;
            typeChanged = true;
            GunPrefab.transform.parent = FPSGunPosition;
            GunPrefab.transform.localPosition = new Vector3(0, 0, 0);
            GunPrefab.transform.localRotation = Quaternion.identity;

            SwordPrefab.transform.parent = FPSGunPosition;
            SwordPrefab.transform.localPosition = new Vector3(0, 0, 0);
            SwordPrefab.transform.localRotation = Quaternion.identity;
        }
        if(Input.GetKeyDown(KeyCode.Alpha9))
        { 
            newType = CameraType.TPS;
            typeChanged = true;
            GunPrefab.transform.parent = TPSISOGunPosition;
            GunPrefab.transform.localPosition = new Vector3(0, 0, 0);
            GunPrefab.transform.localRotation = Quaternion.identity;

            SwordPrefab.transform.parent = TPSISOGunPosition;
            SwordPrefab.transform.localPosition = new Vector3(0, 0, 0);
            SwordPrefab.transform.localRotation = Quaternion.identity;

        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        { 
            newType = CameraType.ISO;
            typeChanged = true;
            GunPrefab.transform.parent = TPSISOGunPosition;
            GunPrefab.transform.localPosition = new Vector3(0, 0, 0);
            GunPrefab.transform.localRotation = Quaternion.identity;
            SwordPrefab.transform.parent = TPSISOGunPosition;
            SwordPrefab.transform.localPosition = new Vector3(0, 0, 0);
            SwordPrefab.transform.localRotation = Quaternion.identity;
        }

        // 카메라 타입이 변경되었다면
        if (typeChanged && _prevCameraType != newType)
        {
            CameraType = newType;
            IsTypeChanged = true;
            _prevCameraType = CameraType;
            
            // 마우스 커서 상태 업데이트
            UpdateCursorState(CameraType);
            
            // 타입 변경 시 초기화 함수 호출
            if (_cameraFollow != null)
            {
                // ISO 모드로 변경 시 ISO 카메라 위치 업데이트
                if (CameraType == CameraType.ISO)
                {
                    UpdateISOCameraPosition();
                }
                
                _cameraFollow.Init();
            }
        }
        
        // ISO 모드에서는 플레이어를 계속 추적
        if (CameraType == CameraType.ISO && _isoCamera != null && _playerTransform != null)
        {
            UpdateISOCameraLookAt();
        }
    }
    
    // ISO 카메라가 플레이어를 바라보도록 업데이트
    private void UpdateISOCameraLookAt()
    {
        // 플레이어 방향을 바라보도록 회전 설정
        Vector3 lookDirection = _playerTransform.position - _isoCamera.transform.position;
        
        // X축 회전만 유지하고 Y축만 플레이어 방향으로 회전
        float currentXRotation = _isoCamera.transform.eulerAngles.x;
        
        // 바라보는 방향 계산 (Y축만 플레이어 방향으로)
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        Quaternion newRotation = Quaternion.Euler(currentXRotation, targetRotation.eulerAngles.y, 0);
        
        // 회전 적용
        _isoCamera.transform.rotation = newRotation;
    }
    
    // ISO 카메라 위치 업데이트 (플레이어가 이동했을 때)
    private void UpdateISOCameraPosition()
    {
        if (_isoCamera != null && _playerTransform != null)
        {
            // 플레이어 위치를 기준으로 카메라 위치 설정
            Vector3 playerPos = _playerTransform.position;
            
            // 플레이어의 앞 방향으로 고정된 거리에 위치
            Vector3 forwardDirection = _playerTransform.forward;
            forwardDirection.y = 0; // y축은 무시
            forwardDirection.Normalize();
            
            // 카메라 위치 설정 (플레이어 뒤쪽 + 높이)
            _isoCamera.transform.position = new Vector3(
                playerPos.x - forwardDirection.x * Mathf.Abs(ISOCameraDistance),
                playerPos.y + ISOCameraHeight,
                playerPos.z - forwardDirection.z * Mathf.Abs(ISOCameraDistance)
            );
            
            // ISO 카메라 회전 설정 (고정된 X축 각도)
            _isoCamera.transform.rotation = Quaternion.Euler(ISOCameraAngle, _playerTransform.eulerAngles.y, 0);
        }
    }
    
    // 마우스 커서 상태 업데이트
    private void UpdateCursorState(CameraType type)
    {
        switch (type)
        {
            case CameraType.FPS:
            case CameraType.TPS:
                // FPS/TPS 모드에서는 마우스 잠금
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case CameraType.ISO:
                // ISO 모드에서는 마우스 잠금 해제
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
    }
    
    // ISO 카메라 생성 (월드 공간에 고정)
    private void CreateISOCamera()
    {
        if (_playerTransform == null) return;
        
        _isoCamera = new GameObject("ISOCameraTarget");
        
        // 초기 위치 설정 (플레이어 기준)
        Vector3 playerPos = _playerTransform.position;
        Vector3 forwardDirection = _playerTransform.forward;
        forwardDirection.y = 0;
        forwardDirection.Normalize();
        
        _isoCamera.transform.position = new Vector3(
            playerPos.x - forwardDirection.x * Mathf.Abs(ISOCameraDistance),
            playerPos.y + ISOCameraHeight,
            playerPos.z - forwardDirection.z * Mathf.Abs(ISOCameraDistance)
        );
        
        // 회전 설정
        _isoCamera.transform.rotation = Quaternion.Euler(ISOCameraAngle, _playerTransform.eulerAngles.y, 0);
        
        // CameraFollow 컴포넌트에 참조 설정
        _cameraFollow.ISOTarget = _isoCamera.transform;
    }
    
    // 카메라 타겟 오브젝트 생성 (플레이어의 자식으로)
    private void CreateCameraTarget(string name, Vector3 localPosition, Vector3 localRotation)
    {
        GameObject target = new GameObject(name);
        target.transform.parent = _playerTransform;
        target.transform.localPosition = localPosition;
        target.transform.localEulerAngles = localRotation;
        
        switch (name)
        {
            case "FPSCameraTarget":
                _cameraFollow.FPSTarget = target.transform;
                break;
            case "TPSCameraTarget":
                _cameraFollow.TPSTarget = target.transform;
                break;
        }
    }
}
