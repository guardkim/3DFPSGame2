using UnityEditor.PackageManager;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    // 목표 : 마우스의 오른쪽 버튼을 누르면 시선이 바라보는 방향으로 수류탄을 던지고 싶다.
    public GameObject ExplosionEffectPrefab;
    public SphereCollider Collider;

    private bool _isBomb = false;

    private void Awake()
    {
        Collider = GetComponent<SphereCollider>();
        Collider.enabled = false;
    }
    public void Fire(float force)
    {
        Rigidbody bombRigidbody = GetComponent<Rigidbody>();
        bombRigidbody.AddForce(Camera.main.transform.forward * force
            , ForceMode.Impulse);
        bombRigidbody.AddTorque(Vector3.one);
        Collider.enabled = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject effectObject = Instantiate(ExplosionEffectPrefab);
        effectObject.transform.position = collision.contacts[0].point;
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (collision.collider.TryGetComponent<IDamageable>(out damageable))
        {
            Damage damage = default;
            damage.Value = 10;
            damage.From = this.gameObject;
            damageable.TakeDamage(damage);
        }
        CameraManager.Instance.ShakeCamera(1.0f, 1.0f);

        Debug.Log($"{collision.gameObject.name} Hit!");
        gameObject.SetActive(false);
    }
}
