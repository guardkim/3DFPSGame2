using System;
using UnityEngine;

public class UI_Popup : MonoBehaviour
{
    // 콜백 함수 : 어떤 함수를 기억해놨다가 특정 시점(특정 작업이 완료된 후) 호출하는 함수
    private Action _closeCallback;
    public void Open(Action closeCallback = null) // 닫힐 때 호출할 함수를 등록
    {
        _closeCallback = closeCallback;
        gameObject.SetActive(true);
    }
    public void Close()
    {
        _closeCallback?.Invoke(); // 닫힐 때 호출할 함수가 있다면 호출
        gameObject.SetActive(false);
        // 옵션 팝업일 경우에는 GameManager의 Countinue 까지 호출해줘야함
    }

    

}
