using UnityEngine;

public class Bomb : MonoBehaviour
{
    // ��ǥ : ���콺�� ������ ��ư�� ������ �ü��� �ٶ󺸴� �������� ����ź�� ������ �ʹ�.
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
