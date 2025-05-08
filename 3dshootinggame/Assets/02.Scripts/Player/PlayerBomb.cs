using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerBomb : MonoBehaviour
{
    public GameObject BombPrefab;
    public GameObject FirePosition;
    public float ThrowPower = 30f;

    private int _currentBoomCount = 3;
    private float _buttonDownTimer = 0.0f;

    void Start()
    {
        
    }

    void Update()
    {
        ProcessBombFire();
    }
    private void ProcessBombFire()
    {
        if (Input.GetKey(KeyCode.G))
        {
            if (_currentBoomCount <= 0) return;
            _buttonDownTimer += Time.deltaTime;
            if (_buttonDownTimer >= 3.0f) _buttonDownTimer = 3.0f;
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            if (_currentBoomCount <= 0) return;
            FireBomb();
            _buttonDownTimer = 0.0f;
        }
    }
    private void FireBomb()
    {
        UI_Manager.Instance.RemoveBoom();
        _currentBoomCount = UI_Manager.Instance.GetBoomCount();
        BombPool.Instance.Create(FirePosition.transform.position, ThrowPower * _buttonDownTimer);

    }
}
