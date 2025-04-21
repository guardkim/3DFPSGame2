using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // 메인카메라를 Player의 자식으로 넣는 것 보단 스크립트로 제어하자.
    // 메인카메라를 이용할 때가 많기 때문, 또한 보간 기법을 넣을 수 없음

    public Transform Target;

    private void Update()
    {
        // 보간 smoothing 기법
        transform.position = Target.position;
    }

}
