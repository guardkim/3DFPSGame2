using System.Collections.Generic;
using UnityEngine;

public class ParticlePoolManager : Singleton<ParticlePoolManager>
{
    public List<ParticlePool> pools;
    private Dictionary<string, Queue<ParticleSystem>> poolDictionary;

    protected override void Awake()
    {
        base.Awake();
        poolDictionary = new Dictionary<string, Queue<ParticleSystem>>();  

        foreach (var pool in pools)
        {
            var queue = new Queue<ParticleSystem>();                    
            for (int i = 0; i < pool.size; i++)
            {
                var ps = Instantiate(pool.prefab, transform);
                ps.gameObject.SetActive(false);

                // 정지 시 콜백 설정
                var main = ps.main;
                main.stopAction = ParticleSystemStopAction.Callback;
                ps.gameObject.AddComponent<ReturnToPool>().Init(this, pool.tag);

                queue.Enqueue(ps);
            }
            poolDictionary.Add(pool.tag, queue);
        }
    }

    // 풀에서 꺼내 위치 이동 후 재생
    public ParticleSystem Spawn(string tag, Vector3 pos)
    {
        if (!poolDictionary.ContainsKey(tag)) return null;
        var queue = poolDictionary[tag];
        if (queue.Count == 0)
        {
            Debug.LogWarning($"Pool '{tag}' exhausted");
            return null;
        }
        var ps = queue.Dequeue();
        ps.transform.position = pos;
        ps.gameObject.SetActive(true);
        ps.Play();                                            
        return ps;
    }

    // 재생 완료 시 ReturnToPool에서 호출
    public void ReturnToPool(string tag, ParticleSystem ps)
    {
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.gameObject.SetActive(false);
        poolDictionary[tag].Enqueue(ps);
    }
}
