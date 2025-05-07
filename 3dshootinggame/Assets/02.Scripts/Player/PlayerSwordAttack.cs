using UnityEngine;

public class PlayerSwordAttack : MonoBehaviour
{
    public ParticleSystem ParticlePrefab;
    private void Awake()
    {
        ParticlePrefab.Stop();
    }
    public void StartTrail()
    {
        ParticlePrefab.Clear();
        ParticlePrefab.Play();
    }
    public void StopTrail()
    {
        ParticlePrefab.Stop();
    }

}
