using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // 일정 시간
    public float IntervalTime = 1.0f;

    // 현재 시간
    private float _currentTimer = 0.0f;
    void Start()
    {
        
    }

    void Update()
    {
        // 시간이 흐르다가
        _currentTimer += Time.deltaTime;

        // 만약 스폰 타임이 된다면
        if (_currentTimer >= IntervalTime)
        {
            _currentTimer = 0.0f;

            IntervalTime = Random.Range(0.6f, 1.5f);

            // 스폰을 한다.
            int percentage = Random.Range(1, 10);
            if (percentage < 6)
            {
                EnemyPool.Instance.Create(EEnemyType.Trace, this.transform.position);
            }
            else if (percentage < 9)
            {
                EnemyPool.Instance.Create(EEnemyType.Patrol, this.transform.position);
            }
        }
    }
}
