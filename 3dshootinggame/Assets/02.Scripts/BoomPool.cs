using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class BombPool : MonoBehaviour
{

    public List<Bomb> BombPrefab;
    public int PoolSize = 30;
    private List<Bomb> _bombs;
    public static BombPool Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        int bombPrefabCount = BombPrefab.Count;
        _bombs = new List<Bomb>(PoolSize * bombPrefabCount);

        foreach (Bomb bombPrefab in BombPrefab)
        {
            for (int i = 0; i < PoolSize; i++)
            {
                Bomb bomb = Instantiate(bombPrefab, transform);
                _bombs.Add(bomb);
                bomb.gameObject.transform.SetParent(transform);
                bomb.gameObject.SetActive(false);
            }
        }
    }

    public Bomb Create(Vector3 position, float force)
    {
        foreach (Bomb bomb in _bombs)
        {
            if (bomb.gameObject.activeInHierarchy == false)
            {
                bomb.transform.position = position;
                bomb.gameObject.SetActive(true);
                bomb.Fire(force);
                return bomb;
            }
        }
        return null;
    }
}
