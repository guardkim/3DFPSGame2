using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    // 필요 속성
    // - 폭탄 발사 위치
    public GameObject FirePosition;
    // - 폭탄 프리팹
    public GameObject BombPrefab;

    public float ThrowPower = 15f;
    public float FireCoolTime = 0.1f;
    public int FireMaxCount = 50;

    private float _buttonDownTimer = 0.0f;
    private float _fireTimer = 0.1f;
    private int _currentBoomCount = 3;
    private int _fireCurrentCount = 50;

    private bool _isReloading = false;

    public ParticleSystem BulletEffect;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void FireBoom()
    {
        UI_Manager.Instance.RemoveBoom();
        _currentBoomCount = UI_Manager.Instance.GetBoomCount();
        BombPool.Instance.Create(FirePosition.transform.position, ThrowPower * _buttonDownTimer);
    }
    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            if (_currentBoomCount <= 0) return;
            _buttonDownTimer += Time.deltaTime;
            if (_buttonDownTimer >= 3.0f) _buttonDownTimer = 3.0f;
            
        }
        if(Input.GetMouseButtonUp(1))
        {
            if (_currentBoomCount <= 0) return;
            FireBoom();
            _buttonDownTimer = 0.0f;
        }
        // 총알 발사(레이저 방식)
        // 1. 왼쪽 버튼 입력 받기
        if (Input.GetMouseButton(0))
        {
            if(UI_Manager.Instance.IsReloading == true) return;
            _fireTimer -= Time.deltaTime;
            if(_fireTimer <= 0.0f && _fireCurrentCount > 0)
            {
                _fireCurrentCount--;
                // 2. 레이를 생성하고 발사 위치와 진행 방향을 설정
                Ray ray = new Ray(FirePosition.transform.position, Camera.main.transform.forward);
                // 3. 레이와 부딛힌 물체의 정보를 저장할 변수를 생성
                RaycastHit hitInfo = new RaycastHit();
                // 4. 레이를 발사한다음 
                bool isHit = Physics.Raycast(ray, out hitInfo);
                if (isHit) //  변수에 데이터가 있다면(부딪혔다면)
                {
                    // 피격 이펙트 생성(표시)
                    BulletEffect.transform.position = hitInfo.point;
                    BulletEffect.transform.forward = hitInfo.normal;
                    BulletEffect.Play();
                }
                UI_Manager.Instance.SetBulletCount(_fireCurrentCount);
                _fireTimer = FireCoolTime;
            }
        }
            // Ray : 레이저 (시작위치, 방향)
            // RayCast : 레이저를 발사
            // RayCastHit : 레이저가 물체와 부딛혔다면, 그 정보를 저장하는 구조체

        if(Input.GetKeyDown(KeyCode.R))
        {
            if (_fireCurrentCount < FireMaxCount)
            {
                UI_Manager.Instance.Reload();
                _fireCurrentCount = FireMaxCount;
            }
        }
    }
}
