using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // ����ī�޶� Player�� �ڽ����� �ִ� �� ���� ��ũ��Ʈ�� ��������.
    // ����ī�޶� �̿��� ���� ���� ����, ���� ���� ����� ���� �� ����

    public Transform Target;

    private void Update()
    {
        // ���� smoothing ���
        transform.position = Target.position;
    }

}
