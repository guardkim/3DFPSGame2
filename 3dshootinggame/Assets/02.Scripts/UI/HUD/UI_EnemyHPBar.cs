using UnityEngine;
using UnityEngine.UI;

public class UI_EnemyHPBar : MonoBehaviour
{
    public Slider HPBar;

    public void RefreshHPBar(float value)
    {
        HPBar.value = value;
    }
    void Start()
    {
        HPBar = GetComponent<Slider>();
        HPBar.value = 1.0f;
    }

    void Update()
    {
        // BillBoard
        transform.forward = Camera.main.transform.forward;
    }
}
