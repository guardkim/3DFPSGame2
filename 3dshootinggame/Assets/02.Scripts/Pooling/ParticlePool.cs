using UnityEngine;

[System.Serializable]
public class ParticlePool
{
    public string tag;       // 풀 식별자
    public ParticleSystem prefab;  // 풀링할 파티클 프리팹
    public int size;      // 초기 풀 크기
}
