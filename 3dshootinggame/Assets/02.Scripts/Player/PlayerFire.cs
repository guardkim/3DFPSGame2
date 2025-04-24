using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public GameObject FirePosition;
    public GameObject BombPrefab;
    public GameObject BulletFirePosition;
    public ParticleSystem BulletEffect;

    public float ThrowPower = 15f;
    public float FireCoolTime = 0.1f;
    public int FireMaxCount = 50;

    private float _buttonDownTimer = 0.0f;
    private float _fireTimer = 0.1f;
    private int _currentBoomCount = 3;
    private int _fireCurrentCount = 50;

    private Animator _ani;
    
    // ISO 모드에서 사용하는 발사 방향
    private Vector3 _isoShootDirection = Vector3.forward;
    private bool _isISOMode = false;

    private void Awake()
    {
        _ani = GetComponent<Animator>();
    }
    
    private void Start()
    {
        // 마우스 커서 설정은 CameraManager에서 처리하므로 여기서는 제거
    }
    
    // ISO 모드에서 발사 방향 설정을 위한 메서드 (CameraRotate에서 호출)
    public void SetISOShootDirection(Vector3 direction)
    {
        _isoShootDirection = direction;
    }
    
    private void FireBoom()
    {
        UI_Manager.Instance.RemoveBoom();
        _currentBoomCount = UI_Manager.Instance.GetBoomCount();
        BombPool.Instance.Create(FirePosition.transform.position, ThrowPower * _buttonDownTimer);
    }
    
    private void Update()
    {
        // 현재 카메라 모드 확인
        _isISOMode = CameraManager.Instance.CameraType == CameraType.ISO;
        
        // 수류탄 발사 처리
        ProcessBombFire();
        
        // 총 발사 처리
        ProcessGunFire();
        
        // 재장전 처리
        if(Input.GetKeyDown(KeyCode.R))
        {
            if (_fireCurrentCount < FireMaxCount)
            {
                UI_Manager.Instance.Reload();
                _fireCurrentCount = FireMaxCount;
            }
        }
    }
    
    private void ProcessBombFire()
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
    }
    
    private void ProcessGunFire()
    {
        if (Input.GetMouseButton(0))
        {
            if(UI_Manager.Instance.IsReloading == true) return;
            _fireTimer -= Time.deltaTime;
            if(_fireTimer <= 0.0f && _fireCurrentCount > 0)
            {
                _fireCurrentCount--;
                _ani.SetTrigger("SHOOT");
                
                if (_isISOMode)
                {
                    // ISO 모드에서는 CameraRotate에서 계산한 방향으로 발사
                    FireBulletISO();
                }
                else
                {
                    // FPS/TPS 모드에서는 카메라 방향으로 발사
                    FireBulletFPSTPS();
                }
                
                UI_Manager.Instance.SetBulletCount(_fireCurrentCount);
                _fireTimer = FireCoolTime;
            }
        }
    }
    
    private void FireBulletFPSTPS()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo = new RaycastHit();
        bool isHit = Physics.Raycast(ray, out hitInfo);
        
        if (isHit) 
        {
            if (hitInfo.collider.gameObject == this.gameObject) return;
            BulletEffect.transform.position = hitInfo.point;
            BulletEffect.transform.forward = hitInfo.normal;
            BulletEffect.Play();
            Vector3 bulletdir = hitInfo.point - BulletFirePosition.transform.position;
            BulletTrailPool.Instance.Fire(BulletFirePosition.transform.position, bulletdir);


            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = hitInfo.collider.gameObject.GetComponent<Enemy>();
                Damage damage;
                damage.Value = 10;
                damage.From = this.gameObject;
                enemy.TakeDamage(damage);
            }
            if (hitInfo.collider.gameObject.CompareTag("Explosive"))
            {
                Barrel barrel = hitInfo.collider.gameObject.GetComponent<Barrel>();
                Damage damage;
                damage.Value = 10;
                damage.From = this.gameObject;
                Vector3 dir = hitInfo.point - damage.From.transform.position;
                barrel.TakeDamage(hitInfo.point, dir, damage);
            }
        }
        else
        {
            BulletTrailPool.Instance.Fire(BulletFirePosition.transform.position, Camera.main.transform.forward);
        }
    }
    
    private void FireBulletISO()
    {
        // ISO 모드에서는 조정된 방향으로 발사
        Ray ray = new Ray(BulletFirePosition.transform.position, _isoShootDirection);
        RaycastHit hitInfo = new RaycastHit();
        
        // 총알 궤적 표시
        BulletTrailPool.Instance.Fire(BulletFirePosition.transform.position, _isoShootDirection);
        
        bool isHit = Physics.Raycast(ray, out hitInfo);
        if (isHit) 
        {
            BulletEffect.transform.position = hitInfo.point;
            BulletEffect.transform.forward = hitInfo.normal;
            BulletEffect.Play();

            if(hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = hitInfo.collider.gameObject.GetComponent<Enemy>();
                Damage damage;
                damage.Value = 10;
                damage.From = this.gameObject;
                enemy.TakeDamage(damage);
            }
        }
    }
}
