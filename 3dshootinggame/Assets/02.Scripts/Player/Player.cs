using UnityEngine;
using UnityEngine.UI;

public enum PlayerMode
{
    Gun = 0,
    Sword = 1,
    Grenade = 2,
}
public class Player : MonoBehaviour, IDamageable
{
    public float PlayerHP = 300.0f;
    public float _maxHP = 300.0f;
    public Slider HPBar;
    public GameObject Gun;
    public GameObject Sword;
    public GameObject Grenade;
    public PlayerMode CurrentMode = PlayerMode.Gun;
    private Animator _ani;
    void Start()
    {
        _maxHP = PlayerHP;
        _ani = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentMode = PlayerMode.Gun;
            
            _ani.SetLayerWeight(1, 1.0f);
            _ani.SetLayerWeight(3, 0.0f);
            _ani.SetBool("IsSword", false);
            UI_Manager.Instance.SelectWeapon(0);
            Gun.SetActive(true);
            Sword.SetActive(false);
            Grenade.SetActive(false);

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentMode = PlayerMode.Sword;
            

            _ani.SetLayerWeight(1, 0.0f);
            _ani.SetLayerWeight(3, 1.0f);
            _ani.SetBool("IsSword", true);
            UI_Manager.Instance.SelectWeapon(1);
            Gun.SetActive(false);
            Sword.SetActive(true);
            Grenade.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CurrentMode = PlayerMode.Grenade;
            
            _ani.SetLayerWeight(1, 0.0f);
            _ani.SetLayerWeight(3, 1.0f);
            _ani.SetBool("IsSword", true);

            UI_Manager.Instance.SelectWeapon(2);
            Gun.SetActive(false);
            Sword.SetActive(false);
            Grenade.SetActive(true);
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
