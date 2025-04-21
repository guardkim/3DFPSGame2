using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float RotationSpeed = 360f;

    private float _rotationX = 0;

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");

        _rotationX += mouseX * RotationSpeed * Time.deltaTime;

        transform.eulerAngles = new Vector3(0, _rotationX, 0);
    }
}
