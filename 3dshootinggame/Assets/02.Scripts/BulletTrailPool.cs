using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrailPool : Singleton<BulletTrailPool>
{
    public BulletTrail BulletTrailPrefab;
    private List<BulletTrail> _bulletTrails;
    public int PoolSize = 50;
    private void Awake()
    {
        _bulletTrails = new List<BulletTrail>(PoolSize);
        for( int i = 0; i < PoolSize; i++)
        {
            BulletTrail bulletTrail = Instantiate(BulletTrailPrefab);
            bulletTrail.gameObject.SetActive(false);
            bulletTrail.gameObject.transform.SetParent(transform);
            _bulletTrails.Add(bulletTrail);
        }
    }
    public void Fire(Vector3 position, Vector3 dir)
    {
        foreach (BulletTrail bulletTrail in _bulletTrails)
        {
            if (bulletTrail.gameObject.activeInHierarchy == false)
            {
                bulletTrail.transform.position = position;
                bulletTrail.Fire(dir);
                return;
            }
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
