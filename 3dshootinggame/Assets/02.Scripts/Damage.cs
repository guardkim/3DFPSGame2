using UnityEngine;

public struct Damage
{
    public int Value;
    public GameObject From;
    public Vector3 hitPoint;
    public Vector3 hitDir;

    public Damage(int value, GameObject from, Vector3 hitPoint = default, Vector3 hitDir = default)
    {
        Value = value;
        From = from;
        this.hitPoint = hitPoint;
        this.hitDir = hitDir;
    }

}
