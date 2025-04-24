using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float RotationSpeed = 360f;

    private float _rotationX = 0;

    private void Update()
    {
        // ISO 모드에서는 플레이어 회전은 CameraRotate에서 처리되므로 여기서는 동작하지 않음
        if (CameraManager.Instance != null && CameraManager.Instance.CameraType == CameraType.ISO)
            return;
            
        float mouseX = Input.GetAxis("Mouse X");

        _rotationX += mouseX * RotationSpeed * Time.deltaTime;

        transform.eulerAngles = new Vector3(0, _rotationX, 0);
    }
}
