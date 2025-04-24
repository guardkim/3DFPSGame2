using UnityEngine;

public class ReturnToPool : MonoBehaviour
{
    private ParticlePoolManager manager;
    private string poolTag;
    private ParticleSystem ps;

    public void Init(ParticlePoolManager mgr, string tag)
    {
        manager = mgr;
        poolTag = tag;
        ps = GetComponent<ParticleSystem>();
    }

    private void OnParticleSystemStopped()
    {
        // 파티클 완료 후 자동 반환 
        manager.ReturnToPool(poolTag, ps);
    }
}
