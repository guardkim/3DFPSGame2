using System.Collections;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    private TrailRenderer _bulletTrail;
    public bool IsFired = false;

    private Vector3 _dir;
    private float _timer = 2.0f;

    private float _bulletSpeed = 10000.0f;

    private void TrailStop()
    {
        gameObject.SetActive(false);
        _bulletTrail.enabled = false;
        _bulletTrail.emitting = false;
        _bulletTrail.Clear();
    }
    private void TrailPlay()
    {
        gameObject.SetActive(true);
        _bulletTrail.Clear();
        _bulletTrail.enabled = true;
        _bulletTrail.emitting = true;
    }
    private void Awake()
    {
        _bulletTrail = GetComponent<TrailRenderer>();
        TrailStop();
    }
    public void Fire(Vector3 dir)
    {
        _dir = dir;
        IsFired = true;
        TrailPlay();
    }

    private void FixedUpdate()
    {
        if (IsFired == false) return;
        _timer -= Time.deltaTime;
        transform.Translate(_dir * _bulletSpeed * Time.deltaTime);
        if (_timer <= 0.0f)
        {
            IsFired = false;
            TrailStop();
            _timer = 2.0f;
        }
    }
}
