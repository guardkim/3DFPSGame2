using System.Threading;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public GameObject BulletFirePosition;
    public ParticleSystem BulletEffect;

    public float FireCoolTime = 0.1f;
    public int FireMaxCount = 50;

    private float _fireTimer = 0.1f;
    private int _fireCurrentCount = 50;
    private Player _player;
    private Animator _ani;
    
    // ISO 모드에서 사용하는 발사 방향
    private Vector3 _isoShootDirection = Vector3.forward;
    private bool _isISOMode = false;
    private LayerMask _damageMask;
    private float _radius = 5f;
    private float _angle = 90f;
    private int _damage = 20;
    private bool _isZoom = false;

    private void Awake()
    {
        _ani = GetComponentInChildren<Animator>();
        _player = GetComponent<Player>();
        _damageMask = LayerMask.GetMask("Enemy","Barrel");

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
    

    
    private void Update()
    {
        // 현재 카메라 모드 확인
        _isISOMode = CameraManager.Instance.CameraType == CameraType.ISO;
        
        // 총 발사 처리
        ProcessGunFire();

        // 줌 처리
        if (Input.GetMouseButtonDown(1))
        {
            if (_player.CurrentMode != PlayerMode.Gun) return;
            if (CameraManager.Instance.CameraType != CameraType.FPS) return;
            _isZoom = !_isZoom;
            if (_isZoom)
            {
                UI_Manager.Instance.ZoomIn();
            }
            else
            {
                
                UI_Manager.Instance.ZoomOut();
            }
        }
        // 재장전 처리
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (_fireCurrentCount < FireMaxCount)
            {
                UI_Manager.Instance.Reload();
                _fireCurrentCount = FireMaxCount;
            }
        }
    }
    
    
    
    private void ProcessGunFire()
    {
        if (Input.GetMouseButton(0) && _player.CurrentMode == PlayerMode.Gun)
        {
            if(UI_Manager.Instance.IsReloading == true) return;
            _fireTimer -= Time.deltaTime;
            if(_fireTimer <= 0.0f && _fireCurrentCount > 0)
            {
                _fireCurrentCount--;
                _ani.SetTrigger("Shoot");
                
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
        if (Input.GetMouseButtonDown(0) && _player.CurrentMode == PlayerMode.Sword)
        {
            Debug.Log("Press Left Click ");
            _ani.SetTrigger("SwordAttack");
        }
    }
    public void SwordAttackTiming()
    {
        Collider[] hitColliders = new Collider[10];
        int count = Physics.OverlapSphereNonAlloc(transform.position, 3.0f, hitColliders, _damageMask);
        Vector3 forward = transform.forward;
        float cosThreshold = Mathf.Cos(Mathf.Deg2Rad * _angle / 0.5f);
        for (int i = 0; i < count; i++)
        {
            Transform target = hitColliders[i].transform;
            Vector3 toTarget = (target.position - transform.position).normalized;

            // 부채꼴(콘) 각도 체크
            if (Vector3.Dot(forward, toTarget) >= cosThreshold)
            {
                Enemy enemy = hitColliders[i].GetComponent<Enemy>();
                if (enemy != null)
                {
                    Damage damage = default;
                    damage.Value = 10;
                    damage.From = this.gameObject;
                    enemy.TakeDamage(damage);
                }
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radius);

        Vector3 left = Quaternion.Euler(0, -_angle * 0.5f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, _angle * 0.5f, 0) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + left * _radius);
        Gizmos.DrawLine(transform.position, transform.position + right * _radius);
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

            IDamageable damageable = hitInfo.collider.GetComponent<IDamageable>();
            if (hitInfo.collider.TryGetComponent<IDamageable>(out damageable))
            {
                Damage damage;
                damage.Value = 10;
                damage.From = this.gameObject;
                damage.hitPoint = hitInfo.point;
                damage.hitDir = hitInfo.point - damage.From.transform.position;
                damageable.TakeDamage(damage);
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

            IDamageable damageable = hitInfo.collider.GetComponent<IDamageable>();
            if (hitInfo.collider.TryGetComponent<IDamageable>(out damageable))
            {
                Damage damage;
                damage.Value = 10;
                damage.From = this.gameObject;
                damage.hitPoint = hitInfo.point;
                damage.hitDir = hitInfo.point - damage.From.transform.position;
                damageable.TakeDamage(damage);
            }
        }
    }
}
