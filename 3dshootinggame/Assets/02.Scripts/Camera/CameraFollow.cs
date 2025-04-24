using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // 카메라 포지션 오브젝트들은 Player의 자식 오브젝트로 존재
    // FPS, TPS는 Player의 자식, ISO는 월드 공간에 고정
    public Transform FPSTarget;
    public Transform TPSTarget;
    public Transform ISOTarget;
    
    // 플레이어 참조
    public Transform PlayerTransform;
    
    // ISO 카메라 설정
    [Header("ISO 카메라 설정")]
    public Vector3 ISOOffset; // 플레이어로부터 카메라 오프셋 (높이, 거리)
    
    private Transform _currentTarget;
    private bool _isISOMode = false;

    private void Awake()
    {
        ISOOffset = ISOTarget.transform.position;
    }
    private void Start()
    {
        // 플레이어가 설정되지 않았다면 찾기
        if (PlayerTransform == null)
        {
            PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        // 각 카메라 타겟이 없다면 경고
        if (FPSTarget == null || TPSTarget == null)
        {
            Debug.LogWarning("FPS 또는 TPS 카메라 타겟이 설정되지 않았습니다. 타겟을 설정해주세요.");
        }
        
        Init(); // 초기화
    }

    private void LateUpdate()
    {
        if (CameraManager.Instance.IsTypeChanged)
        {
            // 카메라 타입이 변경되었을 때만 Init 호출
            Init();
        }
        
        // ISO 모드일 때 간단한 카메라 위치 및 시선 설정
        if (_isISOMode && PlayerTransform != null)
        {
            // 플레이어 위치를 기준으로 고정된 오프셋만큼 이동
            transform.position = PlayerTransform.position + ISOOffset;
            
            // 항상 플레이어를 바라보도록 설정
            transform.LookAt(PlayerTransform);
        }
        // FPS/TPS 모드일 때는 타겟을 따라가는 방식
        else if (_currentTarget != null)
        {
            transform.position = _currentTarget.position;
            transform.rotation = _currentTarget.rotation;
        }
    }
    
    public void Init()
    {
        // 카메라 타입에 따라 현재 타겟 설정
        switch (CameraManager.Instance.CameraType)
        {
            case CameraType.FPS:
                _currentTarget = FPSTarget;
                _isISOMode = false;
                break;
            case CameraType.TPS:
                _currentTarget = TPSTarget;
                _isISOMode = false;
                break;
            case CameraType.ISO:
                _currentTarget = null; // ISO 모드에서는 타겟을 사용하지 않음
                _isISOMode = true;
                break;
            default:
                break;
        }
        
        // 즉시 카메라 위치 및 회전 업데이트
        if (_isISOMode && PlayerTransform != null)
        {
            // ISO 모드 초기화
            transform.position = PlayerTransform.position + ISOOffset;
            transform.LookAt(PlayerTransform);
        }
        else if (_currentTarget != null)
        {
            transform.position = _currentTarget.position;
            transform.rotation = _currentTarget.rotation;
        }
        
        CameraManager.Instance.IsTypeChanged = false;
    }
}
