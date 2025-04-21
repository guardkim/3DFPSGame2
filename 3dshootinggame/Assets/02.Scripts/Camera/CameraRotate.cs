using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    // ī�޶� ȸ�� ��ũ��Ʈ
    // ��ǥ : ���콺�� �����ϸ� ī�޶� �� �������� ȸ����Ű�� �ʹ�.
    
    public float RotationSpeed = 90f;

    // ī�޶� ������ 0���������� �����Ѵٰ� ������ �����.
    private float _rotationX = 0;
    private float _rotationY = 0;

    void Update()
    {
        // 1. ���콺 �Է��� �޴´�. (���콺�� Ŀ���� ������ ����)
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 2. ���콺 �Է����κ��� ȸ����ų ������ �����.
        // ȸ���� �縸ŭ �������ѳ�����
        _rotationX += mouseX * RotationSpeed * Time.deltaTime;
        _rotationY += -mouseY * RotationSpeed * Time.deltaTime;
        _rotationY = Mathf.Clamp(_rotationY, -45f, 60f);

        // 3. ȸ�� �������� ȸ����Ų��.
        transform.eulerAngles = new Vector3(_rotationY, _rotationX, 0);
    }
}
