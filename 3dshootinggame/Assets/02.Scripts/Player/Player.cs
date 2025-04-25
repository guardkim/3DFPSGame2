using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDamageable
{
    public float PlayerHP = 300.0f;
    private float _maxHP = 300.0f;
    public Slider HPBar;
    void Start()
    {
        _maxHP = PlayerHP;
    }

    void Update()
    {
        
    }
    public void TakeDamage(Damage damage)
    {
        PlayerHP -= damage.Value;
        UI_Manager.Instance.BloodFade();
        HPBar.value = (PlayerHP / _maxHP);
        if (PlayerHP < 0) OnDeath();
    }
    private void OnDeath()
    {
        GameManager.Instance.EndGame();
    }
}
