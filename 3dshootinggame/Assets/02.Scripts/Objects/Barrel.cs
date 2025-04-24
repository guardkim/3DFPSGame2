using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    // TakeDamage 당하면 체력 감소 + 체력이 다하면 폭발 이펙트,
    // 폭발 시 주위 적 & 플레이어에게 데미지
    // 폭발하면 랜덤하게 날라가고 n초 후에 사라짐.

    public float BarrelHP = 30.0f;
    private Rigidbody _rb;
    private bool _isDamaged = false;
    private Vector3 _pendingPoint;
    private Vector3 _pendingForce;

    private bool _isExplode = false;
    private float _explosionForce = 10.0f;
    private float _explosionRadius = 30.0f;
    private Vector3 _flyDir;
    private LayerMask _damageMask;



    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _damageMask = LayerMask.GetMask("Enemy", "Player");
    }
    private void FixedUpdate()
    {
        if (_isDamaged && _isExplode == false)
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
    public void TakeDamage(Vector3 hitPoint, Vector3 hitDir, Damage damage)
    {
        BarrelHP -= damage.Value;
        if (BarrelHP <= 0.0f)
        {
            Explode();
        }
        else
        {
            _pendingPoint = hitPoint;
            _pendingForce = hitDir.normalized;
            _isDamaged = true;
        }
    }
    private void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _explosionRadius, _damageMask);
        _isExplode = true;
        ParticlePoolManager.Instance.Spawn("Explosion", this.gameObject.transform.position);
        FlyAway();
        foreach (Collider hit in hitColliders)
        {
            Debug.Log($"{hit.gameObject}");
            if (hit.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = hit.gameObject.GetComponent<Enemy>();
                Damage damage;
                damage.Value = 10;
                damage.From = this.gameObject;
                enemy.TakeDamage(damage);
            }
            if (hit.gameObject.CompareTag("Player"))
            {
                Player player = hit.gameObject.GetComponent<Player>();
                Damage damage;
                damage.Value = 10;
                damage.From = this.gameObject;
                player.TakeDamage(damage);
            }
        }
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
