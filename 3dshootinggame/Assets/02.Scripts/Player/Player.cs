using UnityEngine;

public class Player : MonoBehaviour
{
    public float PlayerHP = 1000.0f;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void TakeDamage(Damage damage)
    {
        PlayerHP -= damage.Value;
    }
}
