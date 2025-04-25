using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

public class Barrel : MonoBehaviour, IDamageable
{
    // TakeDamage 당하면 체력 감소 + 체력이 다하면 폭발 이펙트,
    // 폭발 시 주위 적 & 플레이어에게 데미지
    // 폭발하면 랜덤하게 날라가고 n초 후에 사라짐.

    public float BarrelHP = 30.0f;
    public int Damage = 30;
    private Rigidbody _rb;
    private bool _isDamaged = false;
    private Vector3 _pendingPoint;
    private Vector3 _pendingForce;

    private bool _isExplode = false;
    private bool _isDead = false;
    private float _explosionForce = 10.0f;
    private float _explosionRadius = 30.0f;
    private Vector3 _flyDir;
    private LayerMask _damageMask;



    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _damageMask = LayerMask.GetMask("Enemy", "Player", "Barrel");
    }
    private void FixedUpdate()
    {
        if (_isDamaged && _isDead == false)
        {
            // 물리 프레임마다 ForceMode.Impulse로 순간 힘을 가함
            _rb.AddForceAtPosition(_pendingForce, _pendingPoint, ForceMode.Impulse);  // :contentReference[oaicite:2]{index=2}

            // 한 번만 적용하고 초기화
            _isDamaged = false;
        }
        if(_isExplode)
        {
            _rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);
            _rb.AddForce(_flyDir * _explosionForce, ForceMode.Impulse);
            _isExplode = false;
        }
    }
    public void TakeDamage(Damage damage)
    {
        BarrelHP -= damage.Value;
        if (BarrelHP <= 0.0f)
        {
            Explode();
        }
        else
        {
            _pendingPoint = damage.hitPoint;
            _pendingForce = damage.hitDir.normalized;
            _isDamaged = true;
        }
    }
    private void Explode()
    {
        if (_isDead == true) return;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _explosionRadius, _damageMask);
        _isExplode = true;
        _isDead = true;
        ParticlePoolManager.Instance.Spawn("Explosion", this.gameObject.transform.position);
        FlyAway();
        foreach (Collider hit in hitColliders)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (hit.TryGetComponent<IDamageable>(out damageable))
            {
                Damage damage = default;
                damage.Value = Damage;
                damage.From = this.gameObject;
                damageable.TakeDamage(damage);
            }
        }
        //LayerMask layer = LayerMask.NameToLayer("Barrel");
        //Collider[] barrels = Physics.OverlapSphere(transform.position, _explosionRadius, layer);

        //foreach (Collider barrel in barrels)
        //{
        //    if (barrel.TryGetComponent(out Barrel _barrel))
        //    {
        //        Debug.Log("barrel hits barrel");
        //        _barrel.Explode();
        //    }
        //}
        StartCoroutine(Death());
    }
    IEnumerator Death()
    {
        int rand = Random.Range(3, 8);
        yield return new WaitForSeconds(rand);

        Destroy(gameObject);
    }
    private void FlyAway()
    {
        float x = Random.Range(0f, 1f);
        float y = Random.Range(0f, 1f);
        float z = Random.Range(0f, 1f);

        _flyDir = new Vector3(x, y, z);
    }
}
