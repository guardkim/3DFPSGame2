using System.Collections.Generic;
using UnityEngine;
public enum EEnemyType
{
    Patrol,
    Trace
}
public class EnemyPool : Singleton<EnemyPool>
{
    public List<Enemy> EnemyPrefabs;
    public List<GameObject> EnemySpawners;
    public List<Enemy> _enemies;
    public int PoolSize = 10;

    private Vector3 offset;

    void Start()
    {
        int enemyPrefabCount = EnemyPrefabs.Count;
        _enemies = new List<Enemy>(PoolSize * enemyPrefabCount);

        foreach (Enemy enemyPrefab in EnemyPrefabs)
        {
            for (int i = 0; i < PoolSize * enemyPrefabCount; i++)
            {
                Enemy enemy = Instantiate(enemyPrefab, transform);
                _enemies.Add(enemy);
                enemy.gameObject.transform.SetParent(transform);
                enemy.gameObject.SetActive(false);
            }
        }
    }
    public Enemy Create(EEnemyType enemyType, Vector3 position)
    {
        foreach (Enemy enemy in _enemies)
        {
            if (enemy.EnemyType == enemyType &&
                enemy.gameObject.activeInHierarchy == false)
            {
                offset.x = Random.Range(0.0f, 5.0f);
                offset.y = 1.0f;
                offset.z = Random.Range(0.0f, 5.0f);
                enemy.transform.position = position + offset;
                enemy.gameObject.SetActive(true);

                return enemy;
            }
        }
        return null;
    }

}
