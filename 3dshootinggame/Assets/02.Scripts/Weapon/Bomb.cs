using UnityEngine;

public class Bomb : MonoBehaviour
{
    // 목표 : 마우스의 오른쪽 버튼을 누르면 시선이 바라보는 방향으로 수류탄을 던지고 싶다.
    public GameObject ExplosionEffectPrefab;

    public void Fire(float force)
    {
        Rigidbody bombRigidbody = GetComponent<Rigidbody>();
        bombRigidbody.AddForce(Camera.main.transform.forward * force
            , ForceMode.Impulse);
        bombRigidbody.AddTorque(Vector3.one);
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject effectObject = Instantiate(ExplosionEffectPrefab);
        effectObject.transform.position = collision.contacts[0].point;
        gameObject.SetActive(false);
    }
}
