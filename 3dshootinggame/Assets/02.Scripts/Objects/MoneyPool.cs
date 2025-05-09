using System.Collections.Generic;
using UnityEngine;

public class MoneyPool : Singleton<MoneyPool>
{
    public Money MoneyPrefab;
    public int PoolSize = 1000;
    private List<Money> _moneys;

    private float _offset = 5.0f;

    private void Awake()
    {

        _moneys = new List<Money>(PoolSize);

        for (int i = 0; i < PoolSize; i++)
        {
            Money money = Instantiate(MoneyPrefab, transform);
            money.gameObject.transform.SetParent(transform);
            money.gameObject.SetActive(false);
            money.Init();
            _moneys.Add(money);
        }
    }

    public List<Money> Create(Vector3 position, int count)
    {
        List<Money> moneyList = new List<Money>(count);
        int createCount = 0;
        Debug.Log($"CreateCount : {count}");
        foreach (Money money in _moneys)
        {
            if (money.gameObject.activeInHierarchy == false)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-_offset, _offset),
                    Random.Range(0.0f, _offset),
                    Random.Range(-_offset, _offset)
                );
                money.transform.position = position + randomOffset;
                money.gameObject.SetActive(true);
                money.Spawn();
                moneyList.Add(money);
                createCount++;

                if (createCount == count)
                {
                    return moneyList;
                }
            }
        }
        return null;
    }
}
