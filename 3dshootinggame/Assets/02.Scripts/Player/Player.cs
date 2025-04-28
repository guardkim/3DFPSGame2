using UnityEngine;
using UnityEngine.UI;

public enum PlayerMode
{
    Gun = 0,
    Sword = 1
}
public class Player : MonoBehaviour, IDamageable
{
    public float PlayerHP = 300.0f;
    public float _maxHP = 300.0f;
    public Slider HPBar;
    public GameObject Gun;
    public GameObject Sword;
    public PlayerMode CurrentMode = PlayerMode.Gun;
    void Start()
    {
        _maxHP = PlayerHP;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentMode = PlayerMode.Gun;
            Gun.SetActive(true);
            Sword.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentMode = PlayerMode.Sword;
            Gun.SetActive(false);
            Sword.SetActive(true);
        }
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
